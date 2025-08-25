using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Opponent : MonoBehaviour {
  #region Constants

  #endregion

  #region Internal

  #endregion

  #region Fields

  [SerializeField] private float thinkingTime = 3f;
  [SerializeField] private int movesPerFrame = 128;

  private Coroutine thinking;

  private bool isUnlocked;

  private GameController gameController;
  private Player player;

  private readonly Dictionary<GameState, Dictionary<Move, int>> scores = new();

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

  private void StartThinking() {
    Debug.Log("StartThinking");
    thinking = StartCoroutine(Think());
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
