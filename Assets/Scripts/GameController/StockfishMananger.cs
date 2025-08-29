using UnityEngine;
using UnityEngine.Events;

public class StockfishMananger : MonoBehaviour {
  #region Constants

  #endregion

  #region Internal

  #endregion

  #region Fields

  [SerializeField] private int searchDepth = 15;

  private IStockfishEngine engine;

  private string bestMove;

  #endregion

  #region Events

  [HideInInspector] public UnityEvent OnReady;

  #endregion

  #region Properties

  public bool IsReady { get; private set; }

  public bool IsBusy { get; private set; }

  public bool HasBestMove => bestMove != null;

  public string BestMove {
    get {
      if (bestMove == null) return null;
      var _bestMove = bestMove;
      bestMove = null;
      return _bestMove;
    }
  }

  #endregion

  #region Methods

  public void StartSearch(string fen) {
    if (IsBusy) throw new System.InvalidOperationException("Search is already running!");
    IsBusy = true;
    //Debug.Log($"[StockfishManager] Analyzing FEN: {fen}");
    engine.StartSearch(fen, searchDepth);
  }

  private static IStockfishEngine CreateEngine() {
#if UNITY_IOS && !UNITY_EDITOR
    return new NativeStockfishEngineIOS();
#elif UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
    string stockfishPath = Application.dataPath + "/Editor/Stockfish/Mac/stockfish/stockfish-macos-m1-apple-silicon";
    return new ProcessStockfishEngine(stockfishPath);
#endif
  }

  private void ReadyUp() {
    if (IsReady) return;
    if (engine == null) return;

    IsReady = true;
    OnReady.Invoke();
  }

  #endregion

  #region Coroutines

  #endregion

  #region Handlers

  #endregion

  #region Lifecycle

  private void OnDestroy() {
    engine.Dispose();
  }

  private void Update() {
    if (engine == null) {
      engine = CreateEngine();
      ReadyUp();
    }

    if (!IsBusy) return;
    if (engine.HasBestMove) {
      //Debug.Log("[StockfishManager] Best move found!");
      IsBusy = false;
      bestMove = engine.BestMove;
    } else engine.Update();
  }

  #endregion
}
