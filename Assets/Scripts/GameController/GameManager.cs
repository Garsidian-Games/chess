using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Linq;

public class GameManager : MonoBehaviour {
  #region Constants

  #endregion

  #region Internal

  [System.Serializable] public class MoveEvent : UnityEvent<Move> { }

  #endregion

  #region Fields

  [SerializeField] private Side black;
  [SerializeField] private Side white;

  private UIController uiController;

  #endregion

  #region Events

  [HideInInspector] public UnityEvent OnReady;
  [HideInInspector] public MoveEvent OnMoved;
  [HideInInspector] public UnityEvent OnUndone;

  #endregion

  #region Properties

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
    GameState = GameState.Previous.Previous;
    uiController.Board.Render(GameState);
    OnUndone.Invoke();
  }

  public void Make(Move move, PieceType promotion = PieceType.None) {
    GameState = GameState.MakeMove(move, promotion);
    uiController.Board.Render(GameState);
    OnMoved.Invoke(move);
  }

  private void ReadyUp() {
    if (IsReady) return;
    if (!uiController.IsReady) return;

    GameState = new(uiController.Board, white, black);
    uiController.Board.Render(GameState);

    IsReady = true;
    OnReady.Invoke();
  }

  #endregion

  #region Coroutines

  #endregion

  #region Handlers

  #endregion

  #region Lifecycle

  private void Start() {
    if (uiController.IsReady) ReadyUp();
    else uiController.OnReady.AddListener(ReadyUp);
  }

  private void Awake() {
    uiController = UIController.Instance;
  }

  #endregion
}
