# stockfish/src/build_ios.sh
#!/usr/bin/env bash
set -euo pipefail

CXX="$(xcrun --sdk iphoneos -f clang++)"
SDK="$(xcrun --sdk iphoneos --show-sdk-path)"
MIN_IOS="${MIN_IOS:-12.0}"

CXXFLAGS=(
  -isysroot "$SDK"
  -arch arm64
  -miphoneos-version-min="$MIN_IOS"
  -std=c++17
  -stdlib=libc++
  -O3 -DNDEBUG
  -DUSE_POPCNT=1 -DUSE_NEON=1 -DNO_PREFETCH=1
)

OUT=build_ios
rm -rf "$OUT" && mkdir -p "$OUT"

echo "CXX: $CXX"
echo "SDK: $SDK"
echo "CXXFLAGS: ${CXXFLAGS[*]}"

shopt -s nullglob

# Top-level sources (exclude CLI)
for f in *.cpp; do
  case "$f" in main.cpp) echo "Skip $f"; continue;; esac
  echo "Compiling $f"
  "$CXX" "${CXXFLAGS[@]}" -c "$f" -o "$OUT/${f%.cpp}.o"
done

# NNUE core
if compgen -G "nnue/*.cpp" > /dev/null; then
  mkdir -p "$OUT/nnue"
  for f in nnue/*.cpp; do
    echo "Compiling $f"
    "$CXX" "${CXXFLAGS[@]}" -c "$f" -o "$OUT/nnue/$(basename "${f%.cpp}").o"
  done
fi

# NNUE features (this fixes the make_index undefineds)
if compgen -G "nnue/features/*.cpp" > /dev/null; then
  mkdir -p "$OUT/nnue_features"
  for f in nnue/features/*.cpp; do
    echo "Compiling $f"
    "$CXX" "${CXXFLAGS[@]}" -c "$f" -o "$OUT/nnue_features/$(basename "${f%.cpp}").o"
  done
fi

# Optional: Syzygy (only if you actually want TB probing)
if compgen -G "syzygy/*.cpp" > /dev/null; then
  mkdir -p "$OUT/syzygy"
  for f in syzygy/*.cpp; do
    echo "Compiling $f"
    "$CXX" "${CXXFLAGS[@]}" -c "$f" -o "$OUT/syzygy/$(basename "${f%.cpp}").o"
  done
fi

echo "Archiving libstockfish.a"
echo $(find "$OUT" -name '*.o' -print)
libtool -static -o "$OUT/libstockfish.a" $(find "$OUT" -name '*.o' -print)

echo "Built: $OUT/libstockfish.a"
nm -arch arm64 -gU "$OUT/libstockfish.a" | c++filt | grep -i "UCI::loop" || true


