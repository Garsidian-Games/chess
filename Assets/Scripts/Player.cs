using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Audio;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Linq;

public class Player : MonoBehaviour {
  #region Constants

  public const int HintSearchDepth = 18;

  #endregion

  #region Internal

  private enum Mode {
    Standard,
    CoverageClick,
    MoveClick,
  }

  [System.Serializable] public class PieceEvent : UnityEvent<Piece> { }
  [System.Serializable] public class SideEvent : UnityEvent<SideType> { }

  [System.Serializable]
  public class Music {
    public AudioResource Theme;
    public AudioResource Game;
  }

  [System.Serializable]
  public class Sound {
    [Tooltip("Played when clearing the clicked square")]
    public AudioResource Blur;
    [Tooltip("Played when a move captures an opponent's piece")]
    public AudioResource Capture;
    [Tooltip("Played when clicking on a square a piece")]
    public AudioResource Click;
    [Tooltip("Played when a move results in check")]
    public AudioResource Check;
    [Tooltip("Played when a piece is moved by dropping a dragged piece on a valid square")]
    public AudioResource DragDrop;
    [Tooltip("Played when picking up a dragged piece")]
    public AudioResource DragPickup;
    [Tooltip("Played when releasing a dragged piece on an invalid square")]
    public AudioResource DragReset;
    [Tooltip("Played when clicking on a square with no pieces or coverage")]
    public AudioResource Empty;
    [Tooltip("Played when clicking on a square with only coverage or double clicking on a square with a piece")]
    public AudioResource Focus;
    [Tooltip("Played when something is done wrong")]
    public AudioResource Invalid;
    [Tooltip("Played when player wins")]
    public AudioResource Mate;
    [Tooltip("Played when the game ends in a draw")]
    public AudioResource Draw;
    [Tooltip("Played when a piece is moved by a click")]
    public AudioResource Move;
    [Tooltip("Played when a pawn is being promoted")]
    public AudioResource Promotion;
    [Tooltip("Played when clicking on or dragging a pieces with no moves or coverage")]
    public AudioResource Stuck;
  }

  [System.Serializable]
  public class BorderColor {
    public Color Inspected;
    public Color Opponent;
    public Color Player;
  }

  #endregion

  #region Fields

  [Header("Audio")]
  [SerializeField] private Music music;
  [SerializeField] private Sound sound;

  [Header("Settings")]
  [SerializeField] private Color targetColor;
  [SerializeField] private BorderColor borderColor;

  [Header("Configuration")]
  [SerializeField] private string prefPlayAsBlack = "PlayAsBlack";
  [SerializeField] private float doubleClickTimeframe = 0.5f;

  private SideType side;

  private GameController gameController;
  private UIController uiController;

  private float? clickedAt;
  private float? clearedAt;
  private bool wasDoubleClicked;

  private Move awaitingPromotion;
  private Move[] draggedMoves;

  private SquareStateProvider squareStateProvider;

  private Mode mode;

  #endregion

  #region Events

  [HideInInspector] public SideEvent OnSideChanged;
  [HideInInspector] public UnityEvent OnBeforePromotion;
  [HideInInspector] public PieceEvent OnDragStart;
  [HideInInspector] public UnityEvent OnDragEnd;

  #endregion

  #region Properties

  public static Player Instance => GameObject.FindWithTag("Player").GetComponent<Player>();

  public bool IsWhite => SideType == SideType.White;

  public bool IsBlack => SideType == SideType.Black;

  public bool IsTurnToMove => gameController.GameManager.GameState.BoardState.SideToMove == SideType && !gameController.GameManager.GameState.IsMate;

  public SideType SideType {
    get => side;
    set {
      if (side == value) return;

      side = value;
      if (side == SideType.Black) PlayerPrefs.SetInt(prefPlayAsBlack, 1);
      else PlayerPrefs.DeleteKey(prefPlayAsBlack);

      squareStateProvider.Clear();

      OnSideChanged.Invoke(side);
      SyncCoverage();
    }
  }

  public Square Clicked { get; private set; }

  public Piece Dragged { get; private set; }

  public Color TargetColor => targetColor;

  public Color BorderColorInspected => borderColor.Inspected;

  public Color BorderColorOpponent => borderColor.Opponent;

  public Color BorderColorPlayer => borderColor.Player;

  #endregion

  #region Methods

  public Color BorderColorFor(SideType sideType) {
    return sideType == SideType ? borderColor.Player : borderColor.Opponent;
  }

  public void PlayGameMusic() {
    gameController.AudioManager.PlayMusic(music.Game);
  }

  public void ChoosePromotion(PieceType pieceType) {
    Assert.IsTrue(Piece.PromotionTypes.Contains(pieceType), string.Format("{0} is not a valid promotion type!", pieceType));
    Assert.IsNotNull(awaitingPromotion, "Not awaiting promotion!");

    var move = awaitingPromotion;
    awaitingPromotion = null;

    gameController.GameManager.Make(move, pieceType);
    gameController.AudioManager.PlaySound(
      gameController.GameManager.GameState.IsDraw ? sound.Draw :
      gameController.GameManager.GameState.IsMate ? sound.Mate :
      gameController.GameManager.GameState.InCheck ? sound.Check :
      move.IsCapture ? sound.Capture : sound.Move
    );
  }

  public bool ToggleHint(Move move) {
    squareStateProvider.Hint = move;
    return squareStateProvider.Hint == move;
  }

  private void SyncCoverage() {
    squareStateProvider.Hint = null;
  }

  private void ClearDragged() {
    if (Dragged == null) return;
    Dragged = null;
    draggedMoves = null;

    foreach (var square in uiController.Board.Squares) {
      square.ScreenVisible = false;
      square.BorderVisible = false;
      square.TremblePiece = false;
      square.HighlightVisible = false;
      square.ResetPieceBorderColor();
    }

    OnDragEnd.Invoke();
  }

  private void ClearClicked() {
    if (Clicked == null) return;
    Clicked = null;
    clickedAt = null;
    clearedAt = Time.time;
    squareStateProvider.Reset();
  }

  private void Click(Piece piece, Square square) {
    bool playerPiece = piece.SideType == SideType;
    var _borderColor = playerPiece ? borderColor.Player : borderColor.Opponent;
    var moves = gameController.GameManager.GameState.MovesFor(piece, square);
    var hasMoves = moves.Count() > 0;

    if (!playerPiece || !hasMoves) {
      var coverages = gameController.GameManager.GameState.BoardState.CoverageMap[piece, square];
      var hasCoverage = coverages.Count() > 0;

      if (!hasCoverage) {
        gameController.AudioManager.PlaySound(sound.Stuck);
        return;
      }

      clickedAt = Time.time;
      clearedAt = null;
      gameController.AudioManager.PlaySound(sound.Focus);
      Clicked = square;
      squareStateProvider.Click(Clicked, SquareStateProvider.ProviderMode.Coverages);
      return;
    }

    gameController.AudioManager.PlaySound(sound.Click);

    clickedAt = Time.time;
    clearedAt = null;
    Clicked = square;
    squareStateProvider.Click(Clicked, SquareStateProvider.ProviderMode.Moves);
  }

  private void Click(Square square) {
    var coverages = gameController.GameManager.GameState.BoardState.CoverageMap.To(square);
    var hasCoverage = coverages.Count() > 0;
    //Debug.Log(hasCoverage ? sound.Focus : sound.Empty);
    gameController.AudioManager.PlaySound(hasCoverage ? sound.Focus : sound.Empty);
    if (!hasCoverage) {
      return;
    }
    clickedAt = Time.time;
    clearedAt = null;
    Clicked = square;
    Clicked.BorderColor = borderColor.Inspected;
    foreach (var coverage in coverages) {
      coverage.From.PieceBorderColor = borderColor.Inspected;
      coverage.From.PulsePiece = true;
    }
  }

  private void AcceptHint() {
    if (squareStateProvider.Hint == null) return;
    ClearClicked();
    Make(squareStateProvider.Hint, sound.Move);
    Debug.Log("Accept Hint!");
  }

  private bool ClickToMove(Square square) {
    bool pieceOnSquare = gameController.GameManager.GameState.BoardState.IsPieceOn(Clicked);
    if (!pieceOnSquare) return false;

    var piece = gameController.GameManager.GameState.BoardState[Clicked];
    if (piece.SideType != SideType) return false;

    var move = gameController.GameManager.GameState.MovesFor(piece, Clicked).FirstOrDefault(move => move.To == square);
    if (move == null) return false;

    ClearClicked();
    Make(move, sound.Move);

    return true;
  }

  private void Make(Move move, AudioResource moveSound) {
    if (move.IsPromotion) {
      // pre-promotion preview
      move.From.Piece = null;
      move.To.Piece = move.Piece;

      gameController.AudioManager.PlaySound(sound.Promotion);
      awaitingPromotion = move;
      OnBeforePromotion.Invoke();
      return;
    }

    gameController.GameManager.Make(move);
    gameController.AudioManager.PlaySound(
      gameController.GameManager.GameState.IsDraw ? sound.Draw :
      gameController.GameManager.GameState.IsMate ? sound.Mate :
      gameController.GameManager.GameState.InCheck ? sound.Check :
      move.IsCapture ? sound.Capture : moveSound
    );
  }

  #endregion

  #region Coroutines

  #endregion

  #region Handlers

  private void HandleGameReady() {
    SideType = PlayerPrefs.HasKey(prefPlayAsBlack) ? SideType.Black : SideType.White;
    SyncCoverage();
    gameController.AudioManager.PlayMusic(music.Theme);
  }

  private void HandleSquareClicked(Square square) {
    if (Dragged != null) return;

    if (!IsTurnToMove) {
      gameController.AudioManager.PlaySound(sound.Invalid);
      return;
    }

    if (squareStateProvider.Hint != null && squareStateProvider.Hint.To == square) {
      AcceptHint();
      return;
    }

    bool clickedAgain = square == Clicked;
    bool pieceOnSquare = gameController.GameManager.GameState.BoardState.IsPieceOn(square);
    bool isDoubleClick = clickedAt.HasValue && Time.time < (clickedAt.Value + doubleClickTimeframe);
    bool isDoubleClickFromClear = clearedAt.HasValue && Time.time < (clearedAt.Value + doubleClickTimeframe);

    if (Clicked != null && !clickedAgain) {
      if (ClickToMove(square)) return;
    }

    ClearClicked();

    if ((isDoubleClickFromClear || isDoubleClick) && pieceOnSquare) {
      wasDoubleClicked = true;
      Click(square);
      clickedAt = null;
      return;
    }

    if (clickedAgain && !wasDoubleClicked) {
      gameController.AudioManager.PlaySound(sound.Blur);
      return;
    }

    wasDoubleClicked = false;
    if (pieceOnSquare) Click(gameController.GameManager.GameState.BoardState[square], square);
    else Click(square);
  }

  private void HandleSquareDragBegan(Square square) {
    ClearClicked();

    if (!IsTurnToMove || !gameController.GameManager.GameState.BoardState.IsPieceOn(square)) {
      gameController.AudioManager.PlaySound(sound.Invalid);
      return;
    }

    var piece = gameController.GameManager.GameState.BoardState[square];
    if (piece.SideType != SideType) {
      gameController.AudioManager.PlaySound(sound.Invalid);
      return;
    }

    var moves = gameController.GameManager.GameState.MovesFor(piece, square);
    if (moves.Count() == 0) {
      gameController.AudioManager.PlaySound(sound.Stuck);
      return;
    }

    Dragged = piece;
    draggedMoves = moves;
    gameController.AudioManager.PlaySound(sound.DragPickup);
    OnDragStart.Invoke(Dragged);

    foreach (var s in uiController.Board.Squares) {
      bool hasMove = moves.Any(move => move.To == s);
      bool isHint = squareStateProvider.Hint != null && squareStateProvider.Hint.To == s;

      s.ScreenVisible = !hasMove;
      s.TremblePiece = hasMove;
      s.HighlightVisible = isHint;
      if (isHint) s.BorderColor = borderColor.Inspected;
    }
  }

  private void HandleSquareDragEnded(Square square) {
    if (Dragged == null) return;
    gameController.AudioManager.PlaySound(sound.DragReset);
    ClearDragged();
  }

  private void HandleSquareDropped(Square square) {
    if (Dragged == null) return;

    var move = draggedMoves.FirstOrDefault(move => move.To == square);
    if (move == null) return;

    ClearDragged();
    Make(move, sound.DragDrop);
  }

  private void HandleSquareEntered(Square square) {
    if (Dragged == null) return;
    if (!draggedMoves.Any(move => move.To == square)) return;
    bool isHint = squareStateProvider.Hint != null && squareStateProvider.Hint.To == square;

    square.BorderColor = isHint ? borderColor.Inspected : borderColor.Player;
    if (gameController.GameManager.GameState.BoardState.IsPieceOn(square))
      square.PieceBorderColor = borderColor.Opponent;
  }

  private void HandleSquareExited(Square square) {
    if (Dragged == null) return;

    square.BorderVisible = false;
    square.ResetPieceBorderColor();
  }

  private void HandleMoved(Move _) {
    gameController.AudioManager.PlayMusic(gameController.GameManager.GameState.GameOver ? music.Theme : music.Game);
    SyncCoverage();
  }

  private void HandleUndone() {
    SyncCoverage();
  }

  private void HandleImported() {
    SyncCoverage();
  }

  #endregion

  #region Lifecycle

  private void Update() {
#if UNITY_EDITOR
    if (Input.GetKeyDown(KeyCode.P)) {
      string filename = string.Format("{0}/screenshots/chess_{1}.png", Application.persistentDataPath, System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
      ScreenCapture.CaptureScreenshot(filename);
      Debug.LogFormat("Captured screenshot {0}", filename);
    }
#endif
  }

  private void Start() {
    if (gameController.IsReady) HandleGameReady();
    else gameController.OnReady.AddListener(HandleGameReady);

    uiController.Board.OnSquareClicked.AddListener(HandleSquareClicked);
    uiController.Board.OnSquareDragBegan.AddListener(HandleSquareDragBegan);
    uiController.Board.OnSquareDragEnded.AddListener(HandleSquareDragEnded);
    uiController.Board.OnSquareDropped.AddListener(HandleSquareDropped);
    uiController.Board.OnSquareEntered.AddListener(HandleSquareEntered);
    uiController.Board.OnSquareExited.AddListener(HandleSquareExited);

    gameController.GameManager.OnMoved.AddListener(HandleMoved);
    gameController.GameManager.OnUndone.AddListener(HandleUndone);
    gameController.GameManager.OnImported.AddListener(HandleImported);
  }

  private void Awake() {
    gameController = GameController.Instance;
    uiController = UIController.Instance;
    squareStateProvider = new(gameController, this);
  }

  #endregion
}
