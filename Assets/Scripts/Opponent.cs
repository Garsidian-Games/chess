using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Opponent : MonoBehaviour {
  #region Constants

  private readonly string[] enPassant = new string[] { "a5", "h5", "b5", "g5", "c5", "f5", "d5", "e5" };

  private readonly string[] foolWhite = new string[] { "f3", "g4" };

  private readonly string[] foolBlack = new string[] { "e5", "h4" };

  private readonly string[] scholarWhite = new string[] { "e4", "Bc4", "Qh5", "Qf7" };

  private readonly string[] scholarBlack = new string[] { "e5", "Nc6", "Nf6" };

  #endregion

  #region Internal

  public enum Mode {
    AI,
    EnPassant,
    Fool,
    Scholar,
  }

  [System.Serializable]
  public class Sound {
    public AudioResource capture;
    public AudioResource check;
    public AudioResource mate;
    public AudioResource move;
  }

  #endregion

  #region Fields

  [Header("Settings")]
  [SerializeField] private Sound sound;

  [Header("Configuration")]
  [SerializeField] private Mode mode;
  [SerializeField] private int searchDepth = 15;

  private Coroutine thinking;

  private bool isUnlocked;

  private GameController gameController;
  private Player player;

  private int moveCounter;

  private IStockfishEngine engine;

  #endregion

  #region Events

  [HideInInspector] public UnityEvent OnUnlocked;

  #endregion

  #region Properties

  public static Opponent Instance => GameObject.FindWithTag("Opponent").GetComponent<Opponent>();

  public SideType SideType => player.IsWhite ? SideType.Black : SideType.White;

  public bool IsThinking => thinking != null;

  public bool IsUnlocked {
    get => isUnlocked;
    set {
      if (isUnlocked) return;
      if (!value) return;
      isUnlocked = value;
      OnUnlocked.Invoke();
    }
  }

  #endregion

  #region Methods

  private static Move RandomFrom(IEnumerable<Move> moves) => moves.ElementAt(Random.Range(0, moves.Count()));

  private static Move EndingOnSquare(Move[] moves, string annotation) {
    var squareName = annotation[^2..];
    var to = moves.Where(move => move.To.name == squareName);
    if (to.Count() == 1) return to.First();

    var pieceType = annotation.Length == 2 ? PieceType.Pawn : Piece.TypeFrom(annotation[0]);
    return to.First(move => move.Piece.PieceType == pieceType);
  }

  private Move NextFromOrRandom(string[] annotatedMoves) {
    var moves = gameController.GameManager.GameState.MovesFor(SideType);
    if (moveCounter == annotatedMoves.Length) return RandomFrom(moves);
    return EndingOnSquare(moves, annotatedMoves[moveCounter++]);
  }

  private void StartThinking() {
    switch (mode) {
      case Mode.AI:
        thinking = StartCoroutine(Think());
        break;
      default:
        Make(NextFromOrRandom(AnnotatedMovesFor(mode)));
        break;
    }
  }

  private string[] AnnotatedMovesFor(Mode mode) {
    return mode switch {
      Mode.Fool => SideType == SideType.White ? foolWhite : foolBlack,
      Mode.Scholar => SideType == SideType.White ? scholarWhite : scholarBlack,
      Mode.EnPassant => enPassant,
      _ => throw new System.ArgumentException(string.Format("{0} is not a valid mode!", mode)),
    };
  }

  private void StopThinking() {
    if (!IsThinking) return;
    StopCoroutine(thinking);
    thinking = null;
  }

  private void Make(Move move) {
    PieceType promotion = PieceType.None;
    gameController.GameManager.Make(move, promotion);
    gameController.AudioManager.PlaySound(
      gameController.GameManager.GameState.IsMate ? sound.mate :
      gameController.GameManager.GameState.InCheck ? sound.check :
      move.IsCapture ? sound.capture : sound.move
    );
  }

  private Move ConvertUciToMove(string uciMove, GameState gameState) {
    // Extract start & destination squares from UCI string
    int fromFile = uciMove[0] - 'a';
    int fromRank = int.Parse(uciMove[1].ToString()) - 1;
    int toFile = uciMove[2] - 'a';
    int toRank = int.Parse(uciMove[3].ToString()) - 1;

    Debug.LogFormat("{4}: {0}{1} -> {2}{3}", fromFile, fromRank, toFile, toRank, uciMove);
    //foreach (var m in gameState.MovesFor(SideType)) Debug.Log(m);

    return gameState.MovesFor(SideType).FirstOrDefault(move =>
      move.From.File == fromFile && move.From.Rank == fromRank &&
      move.To.File == toFile && move.To.Rank == toRank
    );
  }

  private static IStockfishEngine CreateEngine() {
#if UNITY_IOS && !UNITY_EDITOR
    return new NativeStockfishEngineIOS();
#elif UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
    string stockfishPath = Application.streamingAssetsPath + "/Stockfish/Mac/stockfish/stockfish-macos-m1-apple-silicon";
    return new ProcessStockfishEngine(stockfishPath);
#endif
  }

  #endregion

  #region Coroutines

  private IEnumerator Think() {
    // 2. Get current FEN from your game state
    GameState gameState = gameController.GameManager.GameState;
    string fen = gameState.ToFEN(); // Assuming you have this. If not, I can help write it.

    Debug.Log($"[Stockfish] Analyzing FEN: {fen}");

    // 3. Start Stockfish search at configured depth
    engine.StartSearch(fen, searchDepth);

    // 4. Wait until Stockfish reports "bestmove"
    while (!engine.HasBestMove) {
      engine.Update();
      yield return null;
    }

    string bestMove = engine.BestMove;
    Debug.Log($"[Stockfish] Best Move = {bestMove}");

    // 5. Convert Stockfish UCI move into your Move type
    Move moveToMake = ConvertUciToMove(bestMove, gameState);
    Make(moveToMake);

    // 6. Stop thinking
    StopThinking();
  }

  #endregion

  #region Handlers

  private void HandleMoved(Move _) {
    IsUnlocked = true;
  }

  #endregion

  #region Lifecycle

  private void Update() {
    if (!IsUnlocked) return;
    if (player.IsTurnToMove) return;
    if (gameController.GameManager.GameState.IsMate) return;
    if (IsThinking) return;

    StartThinking();
  }

  private void Start() {
    gameController.GameManager.OnMoved.AddListener(HandleMoved);
  }

  private void Awake() {
    gameController = GameController.Instance;
    player = Player.Instance;
    engine = CreateEngine();
  }

  #endregion
}
