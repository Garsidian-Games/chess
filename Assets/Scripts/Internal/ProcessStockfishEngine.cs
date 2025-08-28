using System;
using System.Diagnostics;
using System.IO;
using UnityEngine;

public class ProcessStockfishEngine : IStockfishEngine {
  private Process process;
  private StreamWriter input;
  private StreamReader output;
  private bool debugLogging = false; // Toggle debugging easily

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

  public ProcessStockfishEngine(string pathToBinary) {
    if (debugLogging)
      UnityEngine.Debug.Log($"[Stockfish] Launching engine at: {pathToBinary}");

    process = new Process();
    process.StartInfo.FileName = pathToBinary;
    process.StartInfo.UseShellExecute = false;
    process.StartInfo.RedirectStandardInput = true;
    process.StartInfo.RedirectStandardOutput = true;
    process.StartInfo.CreateNoWindow = true;

    try {
      process.Start();
    } catch (Exception e) {
      UnityEngine.Debug.LogError($"[Stockfish] Failed to start process! {e.Message}");
      throw;
    }

    input = process.StandardInput;
    output = process.StandardOutput;

    if (debugLogging) UnityEngine.Debug.Log("[Stockfish] Engine started. Initializing UCI...");

    // Initialize Stockfish in UCI mode
    SendCommand("uci");
    SendCommand("isready");
    WaitFor("readyok");

    if (debugLogging) UnityEngine.Debug.Log("[Stockfish] Engine initialized and ready!");
  }

  public void Update() {
    string line = ReadLine();
    if (string.IsNullOrEmpty(line)) return;

    if (debugLogging) UnityEngine.Debug.Log("[Stockfish] " + line);

    // Parse the actual move from "bestmove"
    if (line.StartsWith("bestmove")) {
      string[] parts = line.Split(' ');
      bestMove = parts[1];
    }
  }

  private void SendCommand(string command) {
    if (debugLogging)
      UnityEngine.Debug.Log($"[Stockfish →] {command}");

    input.WriteLine(command);
    input.Flush();
  }

  private void WaitFor(string expected) {
    string line;
    while ((line = output.ReadLine()) != null) {
      if (debugLogging)
        UnityEngine.Debug.Log($"[Stockfish ←] {line}");

      if (line.Contains(expected)) break;
    }
  }

  /// <summary>
  /// Start Stockfish analysis at the given depth using FEN.
  /// </summary>
  public void StartSearch(string fen, int depth) {
    bestMove = null;

    if (debugLogging) {
      UnityEngine.Debug.Log($"[Stockfish] Starting search...");
      UnityEngine.Debug.Log($"[Stockfish] FEN: {fen}");
      UnityEngine.Debug.Log($"[Stockfish] Depth: {depth}");
    }

    SendCommand($"position fen {fen}");
    SendCommand($"go depth {depth}");
  }

  /// <summary>
  /// Read a single line of output from Stockfish.
  /// </summary>
  public string ReadLine() {
    string line = output.ReadLine();

    if (debugLogging && !string.IsNullOrEmpty(line))
      UnityEngine.Debug.Log($"[Stockfish ←] {line}");

    return line;
  }

  public void Dispose() {
    try {
      if (debugLogging)
        UnityEngine.Debug.Log("[Stockfish] Shutting down engine...");

      SendCommand("quit");
      process.Close();

      if (debugLogging)
        UnityEngine.Debug.Log("[Stockfish] Engine closed successfully.");
    } catch (Exception e) {
      UnityEngine.Debug.LogError($"[Stockfish] Error closing process: {e.Message}");
    }
  }
}
