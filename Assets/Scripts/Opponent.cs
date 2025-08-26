using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Opponent : MonoBehaviour {
  #region Constants

  private readonly string[] fool = new string[] { "f3", "g4" };

  private readonly string[] foolsMates = new string[] { "e5", "h4" };

  private readonly string[] enPassant = new string[] { "a5", "h5", "b5", "g5", "c5", "f5", "d5", "e5" };

  #endregion

  #region Internal

  public enum Mode {
    AI,
    Fool,
    FoolsMate,
    EnPassant,
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
  [SerializeField] private float thinkingTime = 3f;
  [SerializeField] private int movesPerFrame = 128;

  private Coroutine thinking;

  private bool isUnlocked;

  private GameController gameController;
  private Player player;

  private readonly Dictionary<GameState, Dictionary<Move, int>> scores = new();

  private int moveCounter;

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

  private static Move EndingOnSquare(Move[] moves, string squareName) {
    var to = moves.Where(move => move.To.name == squareName);
    if (to.Count() == 1) return to.First();

    var pieceType = squareName.Length == 2 ? PieceType.Pawn : Piece.TypeFrom(squareName[0]);
    return to.FirstOrDefault(move => move.Piece.PieceType == pieceType);
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
      Mode.Fool => fool,
      Mode.FoolsMate => foolsMates,
      Mode.EnPassant => enPassant,
      _ => throw new System.ArgumentException(string.Format("{0} is not a valid mode!", mode)),
    };
  }

  private void StopThinking() {
    Debug.Log("StopThinking");
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

  private int Score(GameState gameState, Move move) {
    if (!scores.ContainsKey(gameState)) scores[gameState] = new();
    if (!scores[gameState].ContainsKey(move)) scores[gameState][move] = CalculateScoreFor(gameState, move);
    return scores[gameState][move];
  }

  private int CalculateScoreFor(GameState gameState, Move move) {
    return Random.Range(int.MinValue, int.MaxValue);
  }

  #endregion

  #region Coroutines

  public IEnumerator Think() {
    float elapsed = 0f;

    GameState gameState = gameController.GameManager.GameState;
    Queue<Move> moves = new(gameState.MovesFor(SideType));

    int? bestScore = null;
    List<Move> bestMoves = null;

    while (elapsed < thinkingTime) {
      elapsed += Time.deltaTime;

      int count = 0;
      while (moves.Count > 0) {
        if (count >= movesPerFrame) break;

        var move = moves.Dequeue();
        var score = Score(gameState, move);

        if (!bestScore.HasValue || bestScore.Value < score) {
          bestScore = score;
          bestMoves = new() { move };
        } else if (bestScore.Value == score) bestMoves.Add(move);

        count++;
      }

      if (moves.Count > 0) yield return new WaitForEndOfFrame();
    }

    Make(bestMoves[Random.Range(0, bestMoves.Count)]);

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
  }

  #endregion
}
