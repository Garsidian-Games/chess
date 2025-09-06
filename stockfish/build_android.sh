#!/usr/bin/env bash
set -euo pipefail

# --- Config you can override ---
: "${ANDROID_NDK:?Set ANDROID_NDK=/path/to/android-ndk-r26d}"
API="${API:-24}"
ABI="${ABI:-arm64-v8a}"
STD="${STD:-c++17}"

# --- Resolve toolchain ---
TC_ROOT="$ANDROID_NDK/toolchains/llvm/prebuilt"
HOST_DIR="$(/bin/ls -1 "$TC_ROOT" | head -n1)"
BIN="$TC_ROOT/$HOST_DIR/bin"

case "$ABI" in
  arm64-v8a)   TRIPLE=aarch64-linux-android;   EXTRA_FLAGS="-DUSE_NEON=1";;
  armeabi-v7a) TRIPLE=armv7a-linux-androideabi; EXTRA_FLAGS="-march=armv7-a -mfpu=neon -mfloat-abi=softfp -mthumb -DUSE_NEON=1";;
  x86_64)      TRIPLE=x86_64-linux-android;    EXTRA_FLAGS="";;
  *) echo "Unsupported ABI: $ABI"; exit 1;;
esac

CXX="$BIN/${TRIPLE}${API}-clang++"
AR="$BIN/llvm-ar"
NM="$BIN/llvm-nm"

OUT="build_android/$ABI"
rm -rf "$OUT" && mkdir -p "$OUT"

CXXFLAGS=(
  -std=${STD}
  -fPIC
  -O3 -DNDEBUG
  -fno-exceptions -fno-rtti
  -DUSE_POPCNT=1 -DNO_PREFETCH=1
)

# split EXTRA_FLAGS into array if non-empty
if [[ -n "${EXTRA_FLAGS// /}" ]]; then
  # shellcheck disable=SC2206
  CXXFLAGS+=($EXTRA_FLAGS)
fi

echo "NDK: $ANDROID_NDK"
echo "CXX: $CXX"
echo "ABI: $ABI  API: $API"
echo "CXXFLAGS: ${CXXFLAGS[*]}"

shopt -s nullglob

# --- Compile Stockfish sources (exclude CLI main) ---
for f in *.cpp; do
  case "$f" in main.cpp) echo "Skip $f"; continue;; esac
  echo "Compiling $f"
  "$CXX" "${CXXFLAGS[@]}" -c "$f" -o "$OUT/${f%.cpp}.o"
done

# --- NNUE core ---
if compgen -G "nnue/*.cpp" > /dev/null; then
  mkdir -p "$OUT/nnue"
  for f in nnue/*.cpp; do
    echo "Compiling $f"
    "$CXX" "${CXXFLAGS[@]}" -c "$f" -o "$OUT/nnue/$(basename "${f%.cpp}").o"
  done
fi

# --- NNUE features ---
if compgen -G "nnue/features/*.cpp" > /dev/null; then
  mkdir -p "$OUT/nnue_features"
  for f in nnue/features/*.cpp; do
    echo "Compiling $f"
    "$CXX" "${CXXFLAGS[@]}" -c "$f" -o "$OUT/nnue_features/$(basename "${f%.cpp}").o"
  done
fi

# --- Syzygy (optional) ---
if compgen -G "syzygy/*.cpp" > /dev/null; then
  mkdir -p "$OUT/syzygy"
  for f in syzygy/*.cpp; do
    echo "Compiling $f"
    "$CXX" "${CXXFLAGS[@]}" -c "$f" -o "$OUT/syzygy/$(basename "${f%.cpp}").o"
  done
fi

# --- Your Android bridge (must exist) ---
if [[ ! -f "bridge_android.cpp" ]]; then
  echo "ERROR: bridge_android.cpp not found next to this script."
  exit 1
fi
echo "Compiling bridge_android.cpp"
"$CXX" "${CXXFLAGS[@]}" -c bridge_android.cpp -o "$OUT/bridge_android.o"

# --- Link shared lib (.so) ---
echo "Linking libstockfish_unity.so"
#$CXX -shared -o "$OUT/libstockfish_unity.so" $(find "$OUT" -name '*.o' -print) \
#    -static-libstdc++ -lm -llog
$CXX -shared -o "$OUT/libstockfish_unity.so" $(find "$OUT" -name '*.o') \
     -Wl,-z,max-page-size=16384 -static-libstdc++ -lm -llog

echo "Built: $OUT/libstockfish_unity.so"
$NM -C --defined-only "$OUT/libstockfish_unity.so" | grep -E '_stockfish_(start|stop|send|read_copy)$' || true


