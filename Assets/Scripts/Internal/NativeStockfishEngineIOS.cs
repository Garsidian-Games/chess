using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class NativeStockfishEngineIOS : IStockfishEngine {
  #region Constants

  #endregion

  #region Internal

  [DllImport("__Internal")] static extern void sf_init();
  [DllImport("__Internal")] static extern void sf_send(string uci);
  [DllImport("__Internal")] static extern int sf_is_ready();
  [DllImport("__Internal")] static extern void sf_clear_ready();
  [DllImport("__Internal")] static extern int sf_has_bestmove();
  [DllImport("__Internal")] static extern IntPtr sf_take_bestmove();

  #endregion

  #region Fields

  private bool debugLogging = false;

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
    if (debugLogging) Debug.Log("[Stockfish] Shutting down engine...");

    sf_send("quit");
    sf_clear_ready();

    if (debugLogging) Debug.Log("[Stockfish] Engine closed successfully.");
  }

  public void Update() {
    if (sf_has_bestmove() == 0) return;
    bestMove = Marshal.PtrToStringAnsi(sf_take_bestmove());
  }

  public void StartSearch(string fen, int depth) {
    sf_send($"position fen {fen}");
    sf_send($"go depth {depth}");
  }

  #endregion

  #region Coroutines

  #endregion

  #region Handlers

  #endregion

  #region Constructor

  public NativeStockfishEngineIOS() {
    if (debugLogging) Debug.Log($"[Stockfish] Launching native iOS engine");
    sf_init();

    if (debugLogging) Debug.Log("[Stockfish] Engine started. Initializing UCI...");

    sf_send("uci");
    sf_send("isready");

    while (sf_is_ready() == 0) {}

    if (debugLogging) Debug.Log("[Stockfish] Engine initialized and ready!");
  }

  #endregion
}
