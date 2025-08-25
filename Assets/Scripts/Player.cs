using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Audio;
using UnityEngine.Events;
using System.Linq;

public class Player : MonoBehaviour {
  #region Constants

  public static float[] CoverageOpacity = new float[] { 0f, 0.2f, 0.4f, 0.6f };

  #endregion

  #region Internal

  [System.Serializable] public class SideEvent : UnityEvent<SideType> { }

  [System.Serializable]
  public class Music {
    public AudioResource Theme;
    public AudioResource Game;
  }

  [System.Serializable]
  public class Sound {
    public AudioResource Tap;
    public AudioResource Focus;
    public AudioResource Blur;
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
  [SerializeField] private BorderColor borderColor;

  [Header("Configuration")]
  [SerializeField] private string prefPlayAsBlack = "PlayAsBlack";
  [SerializeField] private float doubleClickTimeframe = 0.5f;

  private SideType side;

  private GameController gameController;
  private UIController uiController;

  private float? clickedAt;
  private bool wasDoubleClicked;

  private Move awaitingPromotion;

  #endregion

  #region Events

  [HideInInspector] public SideEvent OnSideChanged;
  [HideInInspector] public UnityEvent OnBeforePromotion;

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

      OnSideChanged.Invoke(side);
      SyncCoverage();
    }
  }

  public Square Clicked { get; private set; }

  #endregion

  #region Methods

  public void ChoosePromotion(PieceType pieceType) {
    Assert.IsTrue(Piece.PromotionTypes.Contains(pieceType), string.Format("{0} is not a valid promotion type!"));
    Assert.IsNotNull(awaitingPromotion, "Not awaiting promotion!");

    var move = awaitingPromotion;
    awaitingPromotion = null;

    gameController.GameManager.Make(move, pieceType);
  }

  private static float CoverageOpacityFor(int count) => CoverageOpacity[Mathf.Min(count, CoverageOpacity.Length - 1)];

  private void SyncCoverage() {
    foreach (var square in gameController.GameManager.GameState.BoardState.Squares) {
      var coverages = gameController.GameManager.GameState.BoardState.CoverageMap[square];
      var whiteCount = coverages.Count(c => c.Piece.IsWhite);
      var blackCount = coverages.Count(c => c.Piece.IsBlack);

      square.PlayerCoverageOpacity = CoverageOpacityFor(IsWhite ? whiteCount : blackCount);
      square.OpponentCoverageOpacity = CoverageOpacityFor(IsWhite ? blackCount : whiteCount);
    }
  }

  private void ClearClicked() {
    if (Clicked == null) return;
    Clicked = null;
    clickedAt = null;
    foreach (var square in uiController.Board.Squares) {
      square.WobblePiece = false;
      square.BorderVisible = false;
      square.ResetPieceBorderColor();
    }
    SyncCoverage();
  }

  private void Click(Piece piece, Square square) {
    bool playerPiece = piece.SideType == SideType;
    var _borderColor = playerPiece ? borderColor.Player : borderColor.Opponent;
    var moves = gameController.GameManager.GameState.MovesFor(piece, square);
    var hasMoves = moves.Count() > 0;

    if (!hasMoves) {
      var coverages = gameController.GameManager.GameState.BoardState.CoverageMap[piece, square];
      var hasCoverage = coverages.Count() > 0;

      if (!hasCoverage) {
        gameController.AudioManager.PlaySound(sound.Tap);
        return;
      }

      gameController.AudioManager.PlaySound(sound.Focus);
      Clicked = square;
      Clicked.PieceBorderColor = _borderColor;
      Clicked.WobblePiece = true;
      foreach (var coverage in coverages) coverage.To.BorderColor = _borderColor;
      return;
    }

    gameController.AudioManager.PlaySound(sound.Focus);

    clickedAt = Time.time;
    Clicked = square;
    Clicked.PieceBorderColor = _borderColor;
    foreach (var move in moves) move.To.BorderColor = _borderColor;
  }

  private void Click(Square square) {
    var coverages = gameController.GameManager.GameState.BoardState.CoverageMap[square];
    var hasCoverage = coverages.Count() > 0;

    gameController.AudioManager.PlaySound(hasCoverage ? sound.Focus : sound.Tap);
    if (!hasCoverage) return;
    clickedAt = Time.time;
    Clicked = square;
    Clicked.BorderColor = borderColor.Inspected;
    foreach (var coverage in coverages) {
      coverage.From.PieceBorderColor = borderColor.Inspected;
      coverage.From.WobblePiece = true;
    }
  }

  private bool ClickToMove(Square square) {
    bool pieceOnSquare = gameController.GameManager.GameState.BoardState.IsPieceOn(Clicked);
    if (!pieceOnSquare) return false;

    var piece = gameController.GameManager.GameState.BoardState[Clicked];
    if (piece.SideType != SideType) return false;

    var move = gameController.GameManager.GameState.MovesFor(piece, Clicked).FirstOrDefault(move => move.To == square);
    if (move == null) return false;

    ClearClicked();
    Make(move);

    return true;
  }

  private void Make(Move move) {
    if (move.IsPromotion) {
      awaitingPromotion = move;
      OnBeforePromotion.Invoke();
      return;
    }

    gameController.GameManager.Make(move);
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
    if (!IsTurnToMove) return;

    bool clickedAgain = square == Clicked;
    bool pieceOnSquare = gameController.GameManager.GameState.BoardState.IsPieceOn(square);
    bool isDoubleClick = clickedAt.HasValue && Time.time < (clickedAt.Value + doubleClickTimeframe);

    if (Clicked != null && !clickedAgain) {
      if (ClickToMove(square)) return;
    }

    ClearClicked();

    if (isDoubleClick && pieceOnSquare) {
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
    Debug.LogFormat("Drag Began {0}", square);
  }

  private void HandleSquareDragEnded(Square square) {
    Debug.LogFormat("Drag Ended {0}", square);
  }

  private void HandleSquareDropped(Square square) {
    Debug.LogFormat("Dropped {0}", square);
  }

  private void HandleSquareEntered(Square square) {
    Debug.LogFormat("Entered {0}", square);
  }

  private void HandleSquareExited(Square square) {
    Debug.LogFormat("Exited {0}", square);
  }

  private void HandleMoved(Move _) {
    gameController.AudioManager.PlayMusic(music.Game);
    SyncCoverage();
  }

  private void HandleUndone() {
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
  }

  private void Awake() {
    gameController = GameController.Instance;
    uiController = UIController.Instance;
  }

  #endregion
}
