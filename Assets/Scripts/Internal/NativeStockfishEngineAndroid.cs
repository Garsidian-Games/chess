#if UNITY_ANDROID && !UNITY_EDITOR
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

public class NativeStockfishEngineAndroid : IStockfishEngine, IDisposable {
#region Constants

  // On Android, StreamingAssets paths look like: jar:file://...!/assets
  // We’re not using this right now (EvalFile is commented out), but keeping for parity.
  private const string JarPrefix = "jar:file://";
  private const string FilePrefix = "file://";

  //private const float POLL_INTERVAL = 0f;

#endregion

#region Internal (native bridge)

  // lib name must match your .so base name: libstockfish_unity.so
  private const string LIB = "stockfish_unity";

  [DllImport(LIB)] private static extern void _stockfish_start();
  [DllImport(LIB)] private static extern void _stockfish_stop();
  [DllImport(LIB)] private static extern void _stockfish_send(string cmd);
  [DllImport(LIB)] private static extern int _stockfish_read_copy([Out] byte[] dst, int maxLen);

#endregion

#region Fields

  private static readonly ConcurrentQueue<string> output = new();
  private static bool engineRunning;
  //private static float nextPollTime;

  private static readonly byte[] _buf = new byte[4096];

  private bool debugLogging = true;

#endregion

#region Properties

  private string bestMove;

  public bool HasBestMove => !string.IsNullOrEmpty(bestMove);
  public string BestMove {
    get {
      if (!HasBestMove) return null;
      var _bestMove = bestMove;
      bestMove = null;
      return _bestMove;
    }
  }

#endregion

#region Lifecycle

  public void Dispose() {
    if (!engineRunning) return;

    if (debugLogging) Debug.Log("[Stockfish/Android] Shutting down engine...");

    _stockfish_stop();
    engineRunning = false;

    // drain any stragglers
    while (PollCopy(out _)) { }
    while (output.TryDequeue(out _)) { }

    if (debugLogging) Debug.Log("[Stockfish/Android] Engine closed successfully.");
  }

#endregion

#region Update / IO

  public void Update() {
    DrainPollIntoQueue();

    string line = ReadLine();
    if (string.IsNullOrEmpty(line)) return;

    if (line.StartsWith("bestmove")) {
      string[] parts = line.Split(' ');
      if (parts.Length > 1) bestMove = parts[1];
    }
  }

  private void SendCommand(string command) {
    if (debugLogging) Debug.Log($"[Stockfish/Android →] {command}");
    _stockfish_send(command ?? string.Empty);
  }

  public void StartSearch(string fen, int depth) {
    SendCommand($"position fen {fen}");
    SendCommand($"go depth {depth}");
  }

  private bool TryReadLine(out string line) {
    DrainPollIntoQueue();
    return output.TryDequeue(out line);
  }

  private void DrainPollIntoQueue() {
    if (!engineRunning) return;
    // if (POLL_INTERVAL > 0f && Time.realtimeSinceStartup < nextPollTime) return;

    const int kMaxPerPump = 64;
    int n = 0;
    while (n++ < kMaxPerPump && PollCopy(out var s)) output.Enqueue(s);

    // if (POLL_INTERVAL > 0f) nextPollTime = Time.realtimeSinceStartup + POLL_INTERVAL;
  }

  private static bool PollCopy(out string s) {
    int n = _stockfish_read_copy(_buf, _buf.Length);
    s = n > 0 ? Encoding.ASCII.GetString(_buf, 0, n) : null;
    return s != null;
  }

  private string ReadLine() {
    if (TryReadLine(out string line)) {
      if (debugLogging) Debug.Log($"[Stockfish/Android ←] {line}");
      return line;
    }
    return null;
  }

  private void WaitFor(string expected) {
    string line;
    while ((line = ReadLine()) != null) {
      if (line.Contains(expected)) break;
    }
  }

#endregion

#region Constructor

  public NativeStockfishEngineAndroid() {
    if (engineRunning) return;
    if (debugLogging) Debug.Log("[Stockfish/Android] Launching native engine");

    _stockfish_start();
    engineRunning = true;

    if (debugLogging) Debug.Log("[Stockfish/Android] Engine started!");

    SendCommand("uci");
    SendCommand("isready");
    WaitFor("readyok");

    if (debugLogging) Debug.Log("[Stockfish/Android] Engine initialized and ready!");
  }

#endregion

#region Helpers (optional: StreamingAssets → absolute file path)

  // If/when you enable EvalFile on Android, StreamingAssets are inside the APK (jar),
  // so you must extract to a real file first. Here’s a synchronous helper you can
  // adapt or replace with your own pipeline.
  private static string ResolveStreamingAssetAbsolutePath(string relative) {
    // If it’s already an absolute path, return as-is.
    if (Path.IsPathRooted(relative)) return relative;

    var sa = Application.streamingAssetsPath;
    var combined = sa.EndsWith("/") ? (sa + relative) : (sa + "/" + relative);

    // Case 1: Unpacked (rare on Android dev devices), starts with file://
    if (combined.StartsWith(FilePrefix, StringComparison.OrdinalIgnoreCase)) {
      return combined.Substring(FilePrefix.Length);
    }

    // Case 2: Inside APK jar → copy to persistentDataPath
    // NOTE: For production, do this asynchronously with UnityWebRequest.
    // Here’s a minimal synchronous fallback using Unity’s WWW (deprecated) replaced
    // with UnityWebRequest in a coroutine in real projects. For now, return a target path.
    var dstDir = Application.persistentDataPath;
    var dstPath = Path.Combine(dstDir, relative.Replace("\\", "/"));
    var dstFolder = Path.GetDirectoryName(dstPath);
    if (!Directory.Exists(dstFolder)) Directory.CreateDirectory(dstFolder);

    // Caller should actually copy bytes from StreamingAssets to dstPath before use.
    // Leaving a note so you remember to implement the copy if you enable EvalFile.
    return dstPath;
  }

#endregion
}
#endif