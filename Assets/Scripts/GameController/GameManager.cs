using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Linq;

public class GameManager : MonoBehaviour {
  #region Constants

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

  #endregion

  #region Properties

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

  #endregion

  #region Methods

  public void Undo() {
    timers.Remove(GameState);

    GameState = GameState.Previous.Previous;
    uiController.Board.Render(GameState);

    gameTimer = timers[GameState];
    turnTimer = 0f;

    OnUndone.Invoke();
  }

  public void Make(Move move, PieceType promotion = PieceType.None) {
    var timer = new GameTimer(timers[GameState], turnTimer, GameState.BoardState.SideToMove);

    GameState = GameState.MakeMove(move, promotion);
    uiController.Board.Render(GameState);

    gameTimer = timer;
    timers[GameState] = timer;
    turnTimer = 0f;

    OnMoved.Invoke(move);
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
