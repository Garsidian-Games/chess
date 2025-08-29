using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections.Generic;

public class UIController : MonoBehaviour {
  #region Constants

  #endregion

  #region Internal

  [System.Serializable]
  public class BorderColor {
    public Color Opponent;
    public Color Player;
  }

  [System.Serializable]
  public class GUI {
    public Button menuButton;
    public Button settingsButton;
    public OpponentsTurn opponentsTurn;
    public PlayersTurn playersTurn;
    public ReadyCheckModal readyCheckModal;
    public SideStatus sideStatusWhite;
    public SideStatus sideStatusBlack;
  }

  #endregion

  #region Fields

  [Header("Settings")]
  [SerializeField] private BorderColor borderColor;

  [Header("References")]
  [SerializeField] private Board board;
  [SerializeField] private DraggedPiece draggedPiece;
  [SerializeField] private GUI gui;
  [SerializeField] private MenuWindow menuWindow;
  [SerializeField] private GameObject resetGameWindow;
  [SerializeField] private RecoverGameModal recoverGameModal;
  [SerializeField] private ImportGameWindow importGameWindow;
  [SerializeField] private ExportGameWindow exportGameWindow;
  [SerializeField] private PickPromotionModal pickPromotionModal;
  [SerializeField] private GameScoreWindow gameScoreWindow;
  [SerializeField] private SettingsWindow settingsWindow;
  [SerializeField] private GameOverModal gameOverModal;
  [SerializeField] private GameObject loadingScreen;

  private GameController gameController;

  private Player player;

  private Opponent opponent;

  private PortableGameNotation savedGame;

  private readonly Dictionary<GameState, Move> hints = new();

  private bool gettingHint;

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
    bool canUndo = gameController.GameManager.CanUndo;

    menuWindow.CanResetGame = !isRoot;
    menuWindow.CanExportGame = canUndo;

    gui.readyCheckModal.Display(isRoot && !player.IsTurnToMove);
    gui.opponentsTurn.Display(!isMate && !player.IsTurnToMove);
    gui.playersTurn.Display(!isMate && player.IsTurnToMove, canUndo, !isRoot);

    var captured = gameController.GameManager.GameState.BoardState.Captured;
    gui.sideStatusWhite.Display(captured);
    gui.sideStatusBlack.Display(captured);

    if (gameController.GameManager.GameState.BoardState.SideToMove == SideType.White) {
      gui.sideStatusBlack.ResetBorderColor();
      gui.sideStatusWhite.BorderColor = player.IsWhite ? borderColor.Player : borderColor.Opponent;
    } else {
      gui.sideStatusWhite.ResetBorderColor();
      gui.sideStatusBlack.BorderColor = player.IsWhite ? borderColor.Opponent : borderColor.Player;
    }

    if (isMate) gameOverModal.Show(gameController.GameManager.GameState.BoardState.SideToMove);
    else {
      gui.sideStatusWhite.InCheck = gameController.GameManager.GameState.BoardState.WhiteInCheck;
      gui.sideStatusBlack.InCheck = gameController.GameManager.GameState.BoardState.BlackInCheck;
    }

    gui.sideStatusWhite.Title = player.SideType == SideType.White ? "Player" : "Opponent";
    gui.sideStatusBlack.Title = player.SideType == SideType.White ? "Opponent" : "Player";
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
    Debug.Log(gameController.GameManager.HasSavedGame);
    if (gameController.GameManager.HasSavedGame)
      savedGame = new(board, gameController.GameManager.White, gameController.GameManager.Black, gameController.GameManager.SavedGame);
    //Debug.Log(savedGame);
    Sync();
    //Debug.Log(savedGame.IsValid);

    if (savedGame != null && savedGame.IsValid && !savedGame.EndsInCheckmate) {
      gui.readyCheckModal.Display(false);
      recoverGameModal.Show();
    }

    loadingScreen.SetActive(false);
  }

  private void HandleUndo() {
    gameController.GameManager.Undo();
  }

  private void HandleHint() {
    if (gettingHint) return;
    if (!player.IsTurnToMove) return;

    if (hints.ContainsKey(gameController.GameManager.GameState)) player.ToggleHint(hints[gameController.GameManager.GameState]);
    else {
      gettingHint = true;
      gui.playersTurn.HintBusy = true;
      gameController.StockfishMananger.StartSearch(gameController.GameManager.GameState.ToFEN());
    }
  }

  private void HandleShowScore() {
    gameScoreWindow.Show(gameController.GameManager.History);
  }

  private void HandlePlayerIsReady() {
    gui.opponentsTurn.IsThinking = true;
    opponent.IsUnlocked = true;
    player.PlayGameMusic();
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

  private void HandleResetGame() {
    resetGameWindow.SetActive(true);
  }

  private void HandleExportGame() {
    exportGameWindow.Show(gameController.GameManager.ToString());
  }

  private void HandleGameImported(PortableGameNotation pgn) {
    gameController.GameManager.Import(pgn);
    importGameWindow.Hide();
    menuWindow.Hide();
    Sync();
    gui.readyCheckModal.Display(!player.IsTurnToMove);
  }

  private void HandleClearSave() {
    savedGame = null;
    gameController.GameManager.ClearSave();
    recoverGameModal.Hide();
    gui.readyCheckModal.Display(gameController.GameManager.GameState.BoardState.IsRoot && !player.IsTurnToMove);
  }

  private void HandleLoadSave() {
    gameController.GameManager.RecoverSavedTimers(out float white, out float black);
    gameController.GameManager.Import(savedGame, white, black);
    savedGame = null;
    recoverGameModal.Hide();
    Sync();
    gui.readyCheckModal.Display(!player.IsTurnToMove);
    if (player.IsTurnToMove) player.PlayGameMusic();
  }

  #endregion

  #region Lifecycle

  private void Update() {
    if (!gettingHint) return;
    if (!gameController.StockfishMananger.HasBestMove) return;
    var hint = gameController.GameManager.GameState.ConvertUciToMove(gameController.StockfishMananger.BestMove, player.SideType);
    gettingHint = false;
    gui.playersTurn.HintBusy = false;
    hints[gameController.GameManager.GameState] = hint;
    player.ToggleHint(hint);
  }

  private void LateUpdate() {
    gui.sideStatusWhite.ElapsedTime = gameController.GameManager.WhiteTimer;
    gui.sideStatusBlack.ElapsedTime = gameController.GameManager.BlackTimer;
  }

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

    gui.menuButton.onClick.AddListener(menuWindow.Show);
    gui.settingsButton.onClick.AddListener(settingsWindow.Show);
    gui.playersTurn.OnUndo.AddListener(HandleUndo);
    gui.playersTurn.OnHint.AddListener(HandleHint);
    gui.playersTurn.OnShowScore.AddListener(HandleShowScore);
    gui.readyCheckModal.OnReady.AddListener(HandlePlayerIsReady);

    gameOverModal.OnShowScore.AddListener(HandleShowScore);

    menuWindow.OnNewGame.AddListener(HandleResetGame);
    menuWindow.OnImportGame.AddListener(importGameWindow.Show);
    menuWindow.OnExportGame.AddListener(HandleExportGame);

    importGameWindow.OnImported.AddListener(HandleGameImported);
    importGameWindow.Configure(board, gameController.GameManager.White, gameController.GameManager.Black);

    recoverGameModal.OnCleared.AddListener(HandleClearSave);
    recoverGameModal.OnLoaded.AddListener(HandleLoadSave);
  }

  private void Awake() {
    loadingScreen.SetActive(true);

    gameController = GameController.Instance;
    opponent = Opponent.Instance;
    player = Player.Instance;
  }

  #endregion
}
