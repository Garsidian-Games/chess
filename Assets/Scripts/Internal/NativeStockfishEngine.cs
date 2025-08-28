// NativeStockfishEngineIOS.cs
using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class NativeStockfishEngineIOS : IStockfishEngine {
#if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")] private static extern void init_stockfish();
    [DllImport("__Internal")] private static extern void send_stockfish_command(string cmd);
    [DllImport("__Internal")] private static extern IntPtr read_stockfish_output();
#endif

  private readonly bool debug = true;

  public NativeStockfishEngineIOS() {
#if UNITY_IOS && !UNITY_EDITOR
        init_stockfish();
        // mimic the process flow:
        send_stockfish_command("uci");
        send_stockfish_command("isready");
        if (debug) Debug.Log("[Stockfish iOS] Initialized.");
#else
    Debug.LogWarning("NativeStockfishEngineIOS constructed outside iOS device runtime.");
#endif
  }

  public void StartSearch(string fen, int depth) {
#if UNITY_IOS && !UNITY_EDITOR
        if (debug) Debug.Log($"[Stockfish iOS →] position fen {fen}");
        send_stockfish_command($"position fen {fen}");
        send_stockfish_command($"go depth {depth}");
#endif
  }

  public string ReadLine() {
#if UNITY_IOS && !UNITY_EDITOR
        var ptr = read_stockfish_output();
        var s = Marshal.PtrToStringAnsi(ptr);
        if (!string.IsNullOrEmpty(s)) Debug.Log($"[Stockfish iOS ←] {s}");
        return s;
#else
    return null;
#endif
  }

  public void Dispose() {
#if UNITY_IOS && !UNITY_EDITOR
        send_stockfish_command("quit");
#endif
  }
}
