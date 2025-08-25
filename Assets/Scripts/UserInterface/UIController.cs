using UnityEngine;
using UnityEngine.Events;

public class UIController : MonoBehaviour {
  #region Constants

  #endregion

  #region Internal

  [System.Serializable]
  public class GUI {
    public OpponentsTurn opponentsTurn;
    public PlayersTurn playersTurn;
    public ReadyCheckModal readyCheckModal;
  }

  #endregion

  #region Fields

  [Header("References")]
  [SerializeField] private Board board;
  [SerializeField] private GUI gui;
  [SerializeField] private PickPromotionModal pickPromotionModal;
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
    gui.readyCheckModal.Display(isRoot && !player.IsTurnToMove);
    gui.opponentsTurn.Display(!player.IsTurnToMove);
    gui.playersTurn.Display(player.IsTurnToMove, gameController.GameManager.CanUndo, !isRoot);
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
    Debug.LogError("HandleShowScore Not Implemented");
  }

  private void HandlePlayerIsReady() {
    opponent.IsUnlocked = true;
  }

  private void HandleUndone() {
    Sync();
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
    pickPromotionModal.OnPicked.AddListener(player.ChoosePromotion);
    pickPromotionModal.Sync(player.SideType);

    gameController.GameManager.OnMoved.AddListener(HandleMoved);
    gameController.GameManager.OnUndone.AddListener(HandleUndone);

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
