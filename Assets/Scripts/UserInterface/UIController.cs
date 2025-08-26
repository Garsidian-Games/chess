using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIController : MonoBehaviour {
  #region Constants

  #endregion

  #region Internal

  [System.Serializable]
  public class GUI {
    public Button settingsButton;
    public OpponentsTurn opponentsTurn;
    public PlayersTurn playersTurn;
    public ReadyCheckModal readyCheckModal;
  }

  #endregion

  #region Fields

  [Header("References")]
  [SerializeField] private Board board;
  [SerializeField] private DraggedPiece draggedPiece;
  [SerializeField] private GUI gui;
  [SerializeField] private PickPromotionModal pickPromotionModal;
  [SerializeField] private GameScoreWindow gameScoreWindow;
  [SerializeField] private SettingsWindow settingsWindow;
  [SerializeField] private GameObject loadingScreen;

  private GameController gameController;

  private Player player;

  private Opponent opponent;

  #endregion

  #region Events

  [HideInInspector] public UnityEvent OnReady;

  #endregion

  #region Properties

  public static UIController Instance => GameObject.FindWithTag("UIController").GetComponent<UIController>();

  public bool IsReady { get; private set; }

  public Board Board => board;

  #endregion

  #region Methods

  private void ReadyUp() {
    if (IsReady) return;
    if (!board.IsReady) return;

    IsReady = true;
    OnReady.Invoke();
  }

  private void Sync() {
    bool isRoot = gameController.GameManager.GameState.BoardState.IsRoot;
    bool isMate = gameController.GameManager.GameState.IsMate;
    gui.readyCheckModal.Display(isRoot && !player.IsTurnToMove);
    gui.opponentsTurn.Display(!isMate && !player.IsTurnToMove);
    gui.playersTurn.Display(!isMate && player.IsTurnToMove, gameController.GameManager.CanUndo, !isRoot);
  }

  #endregion

  #region Coroutines

  #endregion

  #region Handlers

  private void HandlePlayerSideChanged(SideType sideType) {
    pickPromotionModal.Sync(sideType);
    Sync();
  }

  private void HandleMoved(Move move) {
    gui.opponentsTurn.IsThinking = true;
    Sync();
  }

  private void HandleGameReady() {
    Sync();
    loadingScreen.SetActive(false);
  }

  private void HandleUndo() {
    gameController.GameManager.Undo();
  }

  private void HandleShowScore() {
    gameScoreWindow.Show(gameController.GameManager.History);
  }

  private void HandlePlayerIsReady() {
    opponent.IsUnlocked = true;
  }

  private void HandleUndone() {
    Sync();
  }

  private void HandleDragStart(Piece piece) {
    draggedPiece.Piece = piece;
  }

  private void HandleDragEnd() {
    draggedPiece.Piece = null;
  }

  #endregion

  #region Lifecycle

  private void Start() {
    if (gameController.IsReady) HandleGameReady();
    else gameController.OnReady.AddListener(HandleGameReady);

    if (board.IsReady) ReadyUp();
    else board.OnReady.AddListener(ReadyUp);

    player.OnSideChanged.AddListener(HandlePlayerSideChanged);
    player.OnBeforePromotion.AddListener(pickPromotionModal.Show);
    player.OnDragStart.AddListener(HandleDragStart);
    player.OnDragEnd.AddListener(HandleDragEnd);
    pickPromotionModal.OnPicked.AddListener(player.ChoosePromotion);
    pickPromotionModal.Sync(player.SideType);

    gameController.GameManager.OnMoved.AddListener(HandleMoved);
    gameController.GameManager.OnUndone.AddListener(HandleUndone);

    gui.settingsButton.onClick.AddListener(settingsWindow.Show);
    gui.playersTurn.OnUndo.AddListener(HandleUndo);
    gui.playersTurn.OnShowScore.AddListener(HandleShowScore);
    gui.readyCheckModal.OnReady.AddListener(HandlePlayerIsReady);
  }

  private void Awake() {
    loadingScreen.SetActive(true);

    gameController = GameController.Instance;
    opponent = Opponent.Instance;
    player = Player.Instance;
  }

  #endregion
}
