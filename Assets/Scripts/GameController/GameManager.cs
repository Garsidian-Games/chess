using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class GameManager : MonoBehaviour {
  #region Constants

  public const string SavedGameKey = "SavedGame";
  public const string SavedGameTimerKey_White = "SavedGameTimer_White";
  public const string SavedGameTimerKey_Black = "SavedGameTimer_Black";

  #endregion

  #region Internal

  [System.Serializable] public class MoveEvent : UnityEvent<Move> { }

  [System.Serializable]
  public class GameTimer {
    public float White;
    public float Black;

    public GameTimer(float white, float black) {
      White = white;
      Black = black;
    }

    public GameTimer(GameTimer timer, float turn, SideType sideType) {
      White = timer.White;
      Black = timer.Black;

      switch (sideType) {
        case SideType.White:
          White += turn;
          break;
        case SideType.Black:
          Black += turn;
          break;
      }
    }
  }

  #endregion

  #region Fields

  [SerializeField] private Side black;
  [SerializeField] private Side white;

  private UIController uiController;

  private readonly Dictionary<GameState, GameTimer> timers = new();

  private GameTimer gameTimer;
  private float turnTimer;

  #endregion

  #region Events

  [HideInInspector] public UnityEvent OnReady;
  [HideInInspector] public MoveEvent OnMoved;
  [HideInInspector] public UnityEvent OnUndone;
  [HideInInspector] public UnityEvent OnImported;

  #endregion

  #region Properties

  public Side White => white;

  public Side Black => black;

  public float WhiteTimer {
    get {
      var timer = gameTimer.White;
      if (GameState.BoardState.SideToMove == SideType.White)
        timer += turnTimer;
      return timer;
    }
  }

  public float BlackTimer {
    get {
      var timer = gameTimer.Black;
      if (GameState.BoardState.SideToMove == SideType.Black)
        timer += turnTimer;
      return timer;
    }
  }

  public bool IsReady { get; private set; }

  public GameState GameState { get; private set; }

  public bool CanUndo => !GameState.IsRoot && !GameState.Previous.IsRoot;

  public GameState[] History {
    get {
      List<GameState> states = new();
      GameState state = GameState;
      while (!state.IsRoot) {
        states.Add(state);
        state = state.Previous;
      }
      states.Reverse();
      return states.ToArray();
    }
  }

  public bool HasSavedGame => PlayerPrefs.HasKey(SavedGameKey);

  public string SavedGame => PlayerPrefs.GetString(SavedGameKey);

  #endregion

  #region Methods

  public void ClearSave() {
    //Debug.Log("ClearSave");
    PlayerPrefs.DeleteKey(SavedGameKey);
    PlayerPrefs.DeleteKey(SavedGameTimerKey_White);
    PlayerPrefs.DeleteKey(SavedGameTimerKey_Black);
  }

  public void RecoverSavedTimers(out float white, out float black) {
    white = PlayerPrefs.HasKey(SavedGameTimerKey_White) ? PlayerPrefs.GetFloat(SavedGameTimerKey_White) : 0f;
    black = PlayerPrefs.HasKey(SavedGameTimerKey_Black) ? PlayerPrefs.GetFloat(SavedGameTimerKey_Black) : 0f;
  }

  public override string ToString() {
    var history = History;
    int moveNumber = 1;

    StringBuilder stringBuilder = new();

    for (int i = 0; i < history.Length; i++) {
      if (i % 2 == 0) stringBuilder.Append($"{moveNumber++}.");
      stringBuilder.Append($"{history[i]} ");
    }

    return stringBuilder.ToString();
  }

  public void Import(PortableGameNotation pgn, float white = 0, float black = 0) {
    var gameState = pgn.GameState;
    while (!gameState.IsRoot) {
      GameTimer timer = new(white, black);
      timers[gameState] = timer;
      gameState = gameState.Previous;
    }

    gameTimer = timers[pgn.GameState];
    GameState = pgn.GameState;
    uiController.Board.Render(GameState);
    OnImported.Invoke();
    Save();
  }

  public void Undo() {
    timers.Remove(GameState);

    GameState = GameState.Previous.Previous;
    uiController.Board.Render(GameState);

    gameTimer = timers.ContainsKey(GameState) ? timers[GameState] : new(0f, 0f);
    turnTimer = 0f;

    OnUndone.Invoke();
    Save();
  }

  public void Make(Move move, PieceType promotion = PieceType.None) {
    var timer = new GameTimer(timers[GameState], turnTimer, GameState.BoardState.SideToMove);

    GameState = GameState.MakeMove(move, promotion);
    uiController.Board.Render(GameState);

    gameTimer = timer;
    timers[GameState] = timer;
    turnTimer = 0f;

    OnMoved.Invoke(move);
    Save();
  }

  private void Save() {
    if (GameState.IsMate) ClearSave();
    else {
      //Debug.Log("Save");
      PlayerPrefs.SetString(SavedGameKey, ToString());
      PlayerPrefs.SetFloat(SavedGameTimerKey_White, gameTimer.White);
      PlayerPrefs.SetFloat(SavedGameTimerKey_Black, gameTimer.Black);
      //Debug.Log(PlayerPrefs.GetString(SavedGameKey));
    }
  }

  private void ReadyUp() {
    if (IsReady) return;
    if (!uiController.IsReady) return;

    GameState = new(uiController.Board, white, black);
    uiController.Board.Render(GameState);

    gameTimer = new(0, 0);
    timers[GameState] = gameTimer;
    turnTimer = 0f;

    IsReady = true;
    OnReady.Invoke();
  }

  #endregion

  #region Coroutines

  #endregion

  #region Handlers

  #endregion

  #region Lifecycle

  private void Update() {
    if (GameState.IsRoot) return;
    turnTimer += Time.deltaTime;
  }

  private void Start() {
    if (uiController.IsReady) ReadyUp();
    else uiController.OnReady.AddListener(ReadyUp);
  }

  private void Awake() {
    uiController = UIController.Instance;
  }

  #endregion
}
