#if UNITY_IOS && !UNITY_EDITOR
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;

public class NativeStockfishEngineIOS : IStockfishEngine {
#region Constants

  private const string filePrefix = "file://";

  // Throttle polling if needed (seconds). 0 = every frame.
  //private const float POLL_INTERVAL = 0f;

#endregion

#region Internal

  [DllImport("__Internal")] private static extern void _stockfish_start();
  [DllImport("__Internal")] private static extern void _stockfish_stop();
  [DllImport("__Internal")] private static extern void _stockfish_send(string cmd);
  [DllImport("__Internal")] static extern int _stockfish_read_copy([Out] byte[] dst, int maxLen);

#endregion

#region Fields

  private static readonly ConcurrentQueue<string> output = new();
  private static bool engineRunning;
  //private static float nextPollTime;

  static readonly byte[] _buf = new byte[4096];

  private bool debugLogging = true;

#endregion

#region Events

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

#region Methods

  public void Dispose() {
    if (!engineRunning) return;

    if (debugLogging) Debug.Log("[Stockfish] Shutting down engine...");

    _stockfish_stop();
    engineRunning = false;

    // drain any stragglers
    while (PollCopy(out _)) { }
    while (output.TryDequeue(out _)) { }

    if (debugLogging) Debug.Log("[Stockfish] Engine closed successfully.");
  }

  public void Update() {
    DrainPollIntoQueue();

    string line = ReadLine();
    if (string.IsNullOrEmpty(line)) return;

    // Parse the actual move from "bestmove"
    if (line.StartsWith("bestmove")) {
      string[] parts = line.Split(' ');
      bestMove = parts[1];
    }
  }

  private void SendCommand(string command) {
    if (debugLogging) Debug.Log($"[Stockfish →] {command}");
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
    //if (POLL_INTERVAL > 0f && Time.realtimeSinceStartup < nextPollTime) return;

    const int kMaxPerPump = 64;
    int n = 0;
    while (n++ < kMaxPerPump && PollCopy(out var s)) output.Enqueue(s);

    //if (POLL_INTERVAL > 0f) nextPollTime = Time.realtimeSinceStartup + POLL_INTERVAL;
  }

  private static bool PollCopy(out string s) {
    int n = _stockfish_read_copy(_buf, _buf.Length);
    s = n > 0 ? System.Text.Encoding.ASCII.GetString(_buf, 0, n) : null;
    return s != null;
  }

  private string ReadLine() {
    if (TryReadLine(out string line)) {
      if (debugLogging) Debug.Log($"[Stockfish ←] {line}");
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

#region Coroutines

#endregion

#region Handlers

#endregion

#region Constructor

  public NativeStockfishEngineIOS() {
    if (engineRunning) return;
    if (debugLogging) Debug.Log($"[Stockfish] Launching native engine for iOS");

    _stockfish_start();
    engineRunning = true;

    if (debugLogging) Debug.Log("[Stockfish] Engine started!");

    SendCommand("uci");
    SendCommand("isready");
    WaitFor("readyok");

    if (debugLogging) Debug.Log("[Stockfish] Engine initialized and ready!");
  }

#endregion
}
#endif