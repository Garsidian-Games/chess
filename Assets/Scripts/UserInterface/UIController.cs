using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
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
  public class Sound {
    public AudioResource tap;
    public AudioResource show;
    public AudioResource hide;
  }

  [System.Serializable]
  public class Permanent {
    public OpponentsTurn opponentsTurn;
    public PlayersTurn playersTurn;
    public SideStatus sideStatusWhite;
    public SideStatus sideStatusBlack;
    public ReadyCheckModal readyCheckModal;
    public Button menuButton;
    public Button settingsButton;
  }

  [System.Serializable]
  public class Modal {
    public RecoverGameModal recoverGame;
    public ResetGameModal resetGame;
    public PickPromotionModal pickPromotion;
    public GameOverModal gameOver;
  }

  [System.Serializable]
  public class Window {
    public SettingsWindow settings;
    public MenuWindow menu;
    public ImportGameWindow importGame;
    public ExportGameWindow exportGame;
    public GameScoreWindow gameScore;
  }

  [System.Serializable]
  public class Screen {
    public LoadingScreen loading;
  }

  #endregion

  #region Fields

  [Header("Settings")]
  [SerializeField] private BorderColor borderColor;
  [SerializeField] private Sound sound;

  [Header("References")]
  [SerializeField] private Board board;
  [SerializeField] private DraggedPiece draggedPiece;
  [SerializeField] private Permanent permanent;
  [SerializeField] private Modal modal;
  [SerializeField] private Window window;
  [SerializeField] private Screen screen;

  [Header("Configuration")]
  [SerializeField] private string prefMoveTimer = "UseMoveTimer";

  private GameController gameController;

  private Player player;

  private Opponent opponent;

  private PortableGameNotation savedGame;

  private readonly Dictionary<GameState, Move> hints = new();

  private bool gettingHint;

  private bool useMoveTimer;

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
    bool isDraw = gameController.GameManager.GameState.IsDraw;
    bool canUndo = gameController.GameManager.CanUndo;

    window.menu.CanResetGame = !isRoot;
    window.menu.CanExportGame = canUndo;

    permanent.readyCheckModal.Display(isRoot && !player.IsTurnToMove);
    permanent.opponentsTurn.Display(!isMate && !player.IsTurnToMove);
    permanent.playersTurn.Display(!isMate && player.IsTurnToMove, canUndo, !isRoot);

    var captured = gameController.GameManager.GameState.BoardState.Captured;
    permanent.sideStatusWhite.Display(captured);
    permanent.sideStatusBlack.Display(captured);

    if (gameController.GameManager.GameState.BoardState.SideToMove == SideType.White) {
      permanent.sideStatusBlack.ResetBorderColor();
      permanent.sideStatusWhite.BorderColor = player.IsWhite ? borderColor.Player : borderColor.Opponent;
    } else {
      permanent.sideStatusWhite.ResetBorderColor();
      permanent.sideStatusBlack.BorderColor = player.IsWhite ? borderColor.Opponent : borderColor.Player;
    }

    if (isMate) modal.gameOver.Show(isDraw, gameController.GameManager.GameState.BoardState.SideToMove);
    else {
      permanent.sideStatusWhite.InCheck = gameController.GameManager.GameState.BoardState.WhiteInCheck;
      permanent.sideStatusBlack.InCheck = gameController.GameManager.GameState.BoardState.BlackInCheck;
    }

    permanent.sideStatusWhite.Title = player.SideType == SideType.White ? "Player" : "Opponent";
    permanent.sideStatusWhite.ForPlayer = player.IsWhite;
    permanent.sideStatusWhite.TurnToMove = gameController.GameManager.GameState.BoardState.SideToMove == SideType.White;

    permanent.sideStatusBlack.Title = player.SideType == SideType.White ? "Opponent" : "Player";
    permanent.sideStatusBlack.ForPlayer = player.IsBlack;
    permanent.sideStatusBlack.TurnToMove = gameController.GameManager.GameState.BoardState.SideToMove == SideType.Black;

    window.settings.Sync(player.SideType, useMoveTimer, opponent.DepthStep);
    window.settings.GameInProgress = !isRoot;

    SyncMoveTimer();
  }

  private void SyncMoveTimer() {
    permanent.sideStatusWhite.UseMoveTimer = useMoveTimer;
    permanent.sideStatusBlack.UseMoveTimer = useMoveTimer;
  }

  private void PlaySound(AudioResource resource) => gameController.AudioManager.PlaySound(resource);

  private void EnableOpponent() {
    opponent.IsUnlocked = true;
    permanent.opponentsTurn.IsThinking = true;
  }

  #endregion

  #region Coroutines

  #endregion

  #region Handlers

  private void HandlePlayerSideChanged(SideType sideType) {
    Sync();
  }

  private void HandlePlayerBeforePromotion() {
    modal.pickPromotion.Show(player.SideType);
  }

  private void HandleMoved(Move move) {
    EnableOpponent();
    Sync();
  }

  private void HandleGameReady() {
    if (gameController.GameManager.HasSavedGame)
      savedGame = new(board, gameController.GameManager.White, gameController.GameManager.Black, gameController.GameManager.SavedGame);

    Sync();

    if (savedGame != null && savedGame.IsValid && !savedGame.EndsInCheckmate) {
      permanent.readyCheckModal.Display(false);
      modal.recoverGame.Show();
    }

    screen.loading.Hide();
  }

  private void HandleUndo() {
    modal.gameOver.Hide();
    PlaySound(sound.tap);
    gameController.GameManager.Undo();
  }

  private void HandleHint() {
    if (gettingHint) return;
    if (!player.IsTurnToMove) return;

    PlaySound(sound.tap);
    if (hints.ContainsKey(gameController.GameManager.GameState)) player.ToggleHint(hints[gameController.GameManager.GameState]);
    else {
      gettingHint = true;
      permanent.playersTurn.HintBusy = true;
      gameController.StockfishMananger.StartSearch(gameController.GameManager.GameState.ToFEN(), Player.HintSearchDepth);
    }
  }

  private void HandleSideSwitched(SideType sideType) {
    player.SideType = sideType switch {
      SideType.White => SideType.Black,
      SideType.Black => SideType.White,
      _ => throw new System.ArgumentException(string.Format("{0} is not a valid SideType", sideType)),
    };
  }

  private void HandleDepthStepChanged(int depthStep) {
    opponent.DepthStep = depthStep;
  }

  private void HandleResetGame() {
    gameController.GameManager.ClearSave();
    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
  }

  private void HandleShowScore() {
    PlaySound(sound.tap);
    window.gameScore.Show(gameController.GameManager.History);
  }

  private void HandlePlayerIsReady() {
    permanent.readyCheckModal.Hide();
    permanent.opponentsTurn.IsThinking = true;
    opponent.IsUnlocked = true;
    player.PlayGameMusic();
  }

  private void HandleUndone() {
    window.gameScore.Hide();
    modal.gameOver.Hide();
    EnableOpponent();
    Sync();
  }

  private void HandleImported() {
    Sync();
  }

  private void HandleDragStart(Piece piece) {
    draggedPiece.Piece = piece;
  }

  private void HandleDragEnd() {
    draggedPiece.Piece = null;
  }

  private void HandleExportGame() {
    window.exportGame.Show(gameController.GameManager.ToString());
  }

  private void HandleGameExportCopied(string pgn) {
    PlaySound(sound.tap);
    GUIUtility.systemCopyBuffer = pgn;
  }

  private void HandleGameImported(PortableGameNotation pgn) {
    PlaySound(sound.tap);
    gameController.GameManager.Import(pgn);
    window.importGame.Hide();
    window.menu.Hide();
    Sync();
    if (!pgn.EndsInCheckmate) permanent.readyCheckModal.Display(!player.IsTurnToMove);
  }

  private void HandleClearSave() {
    savedGame = null;
    gameController.GameManager.ClearSave();
    modal.recoverGame.Hide();
    permanent.readyCheckModal.Display(gameController.GameManager.GameState.BoardState.IsRoot && !player.IsTurnToMove);
  }

  private void HandleLoadSave() {
    gameController.GameManager.RecoverSavedTimers(out float white, out float black);
    gameController.GameManager.Import(savedGame, white, black);
    savedGame = null;
    modal.recoverGame.Hide();
    Sync();
    permanent.readyCheckModal.Display(!player.IsTurnToMove);
    if (player.IsTurnToMove) player.PlayGameMusic();
  }

  private void HandleDisplayToggleShown() => PlaySound(sound.show);

  private void HandleDisplayToggleHidden() => PlaySound(sound.hide);

  private void HandleTimeToggled(bool value) {
    if (value) PlayerPrefs.DeleteKey(prefMoveTimer);
    else PlayerPrefs.SetInt(prefMoveTimer, 1);
    useMoveTimer = PlayerPrefs.HasKey(prefMoveTimer);
    SyncMoveTimer();
  }

  private void HandleScoreClicked(int index, SideType sideType) => gameController.GameManager.RevertTo(index, sideType);

  #endregion

  #region Binders

  #region Primary

  private void Bind(GameController gameController) {
    if (gameController.IsReady) HandleGameReady();
    else gameController.OnReady.AddListener(HandleGameReady);

    gameController.GameManager.OnMoved.AddListener(HandleMoved);
    gameController.GameManager.OnUndone.AddListener(HandleUndone);
    gameController.GameManager.OnImported.AddListener(HandleImported);
  }

  private void Bind(Board board) {
    if (board.IsReady) ReadyUp();
    else board.OnReady.AddListener(ReadyUp);
  }

  private void Bind(Player player) {
    player.OnSideChanged.AddListener(HandlePlayerSideChanged);
    player.OnBeforePromotion.AddListener(HandlePlayerBeforePromotion);
    player.OnDragStart.AddListener(HandleDragStart);
    player.OnDragEnd.AddListener(HandleDragEnd);
  }

  #endregion

  #region UI

  private void Bind(Permanent gui) {
    gui.menuButton.onClick.AddListener(window.menu.Show);
    gui.settingsButton.onClick.AddListener(window.settings.Show);

    Bind(gui.playersTurn);
    Bind(gui.readyCheckModal);
  }

  private void Bind(Modal modal) {
    Bind(modal.recoverGame);
    Bind(modal.resetGame);
    Bind(modal.pickPromotion);
    Bind(modal.gameOver);
  }

  private void Bind(Window window) {
    Bind(window.settings);
    Bind(window.menu);
    Bind(window.importGame);
    Bind(window.exportGame);
    Bind(window.gameScore);
  }

  #endregion

  #region GUI

  private void Bind(PlayersTurn playersTurn) {
    playersTurn.OnUndo.AddListener(HandleUndo);
    playersTurn.OnHint.AddListener(HandleHint);
    playersTurn.OnShowScore.AddListener(HandleShowScore);
  }

  private void Bind(ReadyCheckModal readyCheckModal) {
    // Don't bind the normal modal sound handlers - the beginning of the game is noisy enough without them
    // Bind(readyCheckModal as UIModal);
    readyCheckModal.OnReady.AddListener(HandlePlayerIsReady);
  }

  #endregion

  #region Modal

  private void Bind(PickPromotionModal pickPromotionModal) {
    // Don't bind the normal modal sound handlers - the player already handles sounds for this modal
    // Bind(pickPromotionModal as UIModal);
    pickPromotionModal.OnPicked.AddListener(player.ChoosePromotion);
  }

  private void Bind(GameOverModal gameOverModal) {
    // Don't bind the normal modal sound handlers - the player already handles sounds for this modal
    // Bind(gameOverModal as UIModal);
    gameOverModal.OnUndo.AddListener(HandleUndo);
    gameOverModal.OnShowScore.AddListener(HandleShowScore);
    gameOverModal.OnNewGame.AddListener(HandleResetGame);
  }

  private void Bind(RecoverGameModal recoverGameModal) {
    // bind only the hide handler - otherwise an annoying window popup plays at app load
    recoverGameModal.OnHidden.AddListener(HandleDisplayToggleHidden);
    recoverGameModal.OnCleared.AddListener(HandleClearSave);
    recoverGameModal.OnLoaded.AddListener(HandleLoadSave);
  }

  private void Bind(BaseResetGameModal baseResetGameModal) {
    // reset triggers a scene reload so only a sound handler for resume is required
    baseResetGameModal.OnHidden.AddListener(HandleDisplayToggleHidden);
    baseResetGameModal.OnReset.AddListener(HandleResetGame);
    baseResetGameModal.OnResume.AddListener(baseResetGameModal.Hide);
  }

  #endregion

  #region Window

  private void Bind(MenuWindow menuWindow) {
    Bind(menuWindow as UIWindow);
    menuWindow.OnNewGame.AddListener(modal.resetGame.Show);
    menuWindow.OnImportGame.AddListener(window.importGame.Show);
    menuWindow.OnExportGame.AddListener(HandleExportGame);
  }

  private void Bind(SettingsWindow settingsWindow) {
    Bind(settingsWindow as UIWindow);
    settingsWindow.OnSideSwitched.AddListener(HandleSideSwitched);
    settingsWindow.OnDepthStepChanged.AddListener(HandleDepthStepChanged);
    settingsWindow.OnTimeToggled.AddListener(HandleTimeToggled);
    settingsWindow.OnReset.AddListener(HandleResetGame);
    settingsWindow.OnInnerModalClosed.AddListener(HandleDisplayToggleHidden);
  }

  private void Bind(ImportGameWindow importGameWindow) {
    Bind(importGameWindow as UIWindow);
    importGameWindow.OnImported.AddListener(HandleGameImported);
    importGameWindow.Configure(board, gameController.GameManager.White, gameController.GameManager.Black);
  }

  private void Bind(ExportGameWindow exportGameWindow) {
    Bind(exportGameWindow as UIWindow);
    exportGameWindow.OnCopied.AddListener(HandleGameExportCopied);
  }

  private void Bind(GameScoreWindow gameScoreWindow) {
    Bind(gameScoreWindow as UIWindow);
    gameScoreWindow.OnScoreClicked.AddListener(HandleScoreClicked);
  }

  #endregion

  #region Common

  private void Bind(UIModal modal) => Bind(modal as DisplayToggle);

  private void Bind(UIWindow window) => Bind(window as DisplayToggle);

  private void Bind(DisplayToggle displayToggle) {
    displayToggle.OnShown.AddListener(HandleDisplayToggleShown);
    displayToggle.OnHidden.AddListener(HandleDisplayToggleHidden);
  }

  #endregion

  #endregion

  #region Lifecycle

  private void Update() {
    if (!gettingHint) return;
    if (!gameController.StockfishMananger.HasBestMove) return;
    var hint = gameController.GameManager.GameState.ConvertUciToMove(gameController.StockfishMananger.BestMove, player.SideType);
    gettingHint = false;
    permanent.playersTurn.HintBusy = false;
    hints[gameController.GameManager.GameState] = hint;
    player.ToggleHint(hint);
  }

  private void LateUpdate() {
    permanent.sideStatusWhite.ElapsedTime = gameController.GameManager.WhiteTimer;
    permanent.sideStatusBlack.ElapsedTime = gameController.GameManager.BlackTimer;
  }

  private void Start() {
    screen.loading.Show();

    useMoveTimer = PlayerPrefs.HasKey(prefMoveTimer);
    SyncMoveTimer();

    Bind(board);
    Bind(gameController);
    Bind(player);
    Bind(permanent);
    Bind(modal);
    Bind(window);
  }

  private void Awake() {
    gameController = GameController.Instance;
    opponent = Opponent.Instance;
    player = Player.Instance;
  }

  #endregion
}
