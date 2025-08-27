using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class Square : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler, IDragHandler, IDropHandler {
  #region Constants

  public static Color Black => new(0.99f, 0.99f, 0.99f);

  public static Color White => new(1.00f, 1.00f, 1.00f);

  public static float[] AxisWeight = new float[] { 0.25f, 0.5f, 0.75f, 1f, 1f, 0.75f, 0.5f, 0.25f };

  #endregion

  #region Internal

  [System.Serializable] public class SquareEvent : UnityEvent<Square> { }

  [System.Serializable]
  public class Coverage {
    public Image opponent;
    public Image player;
  }

  [System.Serializable]
  public class PieceDisplay {
    public Wobble wobble;
    public PulseScale pulse;
    public Tremble tremble;
    public Image icon;
    public Image border;
    public Image alertGreen;
    public Image alertRed;
  }

  #endregion

  #region Fields

  [SerializeField] private Coverage coverage;
  [SerializeField] private Image highlight;
  [SerializeField] private Image target;
  [SerializeField] private Image border;
  [SerializeField] private PieceDisplay pieceDisplay;
  [SerializeField] private Image screen;

  private Image image;

  private int? index;
  private float? playerCoverageOpacity;
  private float? opponentCoverageOpacity;

  private Sprite defaultPieceIcon;
  private Sprite defaultPieceBorder;
  private Color defaultPieceBorderColor;

  private Piece piece;

  #endregion

  #region Events

  [HideInInspector] public SquareEvent OnClicked;
  [HideInInspector] public SquareEvent OnDragBegan;
  [HideInInspector] public SquareEvent OnDragEnded;
  [HideInInspector] public SquareEvent OnDropped;
  [HideInInspector] public SquareEvent OnEntered;
  [HideInInspector] public SquareEvent OnExited;

  #endregion

  #region Properties

  public float Weight => AxisWeight[Rank] * AxisWeight[File];

  public int Index {
    get {
      if (!index.HasValue) index = transform.GetSiblingIndex();
      return index.Value;
    }
  }

  public bool IsWhite => ((File + Rank) % 2) != 0;

  public int Rank => Index / 8;

  public int File => Index % 8;

  public char FileChar => FileIndexToChar(File);

  public bool HighlightVisible {
    get => highlight.enabled;
    set => highlight.enabled = value;
  }

  public Color TargetColor {
    get => target.color;
    set {
      target.color = value;
      TargetVisible = true;
    }
  }

  public bool TargetVisible {
    get => target.enabled;
    set => target.enabled = value;
  }

  public Color BorderColor {
    get => border.color;
    set {
      border.color = value;
      BorderVisible = true;
    }
  }

  public bool BorderVisible {
    get => border.enabled;
    set => border.enabled = value;
  }

  public bool ScreenVisible {
    get => screen.enabled;
    set => screen.enabled = value;
  }

  public float OpponentCoverageOpacity {
    get => opponentCoverageOpacity.Value;
    set => SetCoverageOpacity(ref opponentCoverageOpacity, value, coverage.opponent);
  }

  public float PlayerCoverageOpacity {
    get => playerCoverageOpacity.Value;
    set => SetCoverageOpacity(ref playerCoverageOpacity, value, coverage.player);
  }

  public Piece Piece {
    get => piece;
    set {
      piece = value;
      pieceDisplay.icon.sprite = piece == null ? defaultPieceIcon : piece.Icon;
      pieceDisplay.border.sprite = piece == null ? defaultPieceBorder : piece.Border;
      ResetPieceBorderColor();
    }
  }

  public Color PieceBorderColor {
    get => pieceDisplay.border.color;
    set => pieceDisplay.border.color = value;
  }

  public bool WobblePiece {
    get => pieceDisplay.wobble.enabled;
    set => pieceDisplay.wobble.enabled = value;
  }

  public bool PulsePiece {
    get => pieceDisplay.pulse.enabled;
    set => pieceDisplay.pulse.enabled = value;
  }

  public bool TremblePiece {
    get => pieceDisplay.tremble.enabled;
    set => pieceDisplay.tremble.enabled = value;
  }

  public bool GreenAlert {
    get => pieceDisplay.alertGreen.enabled;
    set => pieceDisplay.alertGreen.enabled = value;
  }

  public bool RedAlert {
    get => pieceDisplay.alertRed.enabled;
    set => pieceDisplay.alertRed.enabled = value;
  }

  public bool ShowAlerts {
    get => pieceDisplay.alertGreen.gameObject.activeSelf;
    set {
      pieceDisplay.alertGreen.gameObject.SetActive(value);
      pieceDisplay.alertRed.gameObject.SetActive(value);
    }
  }

  #endregion

  #region Methods

  public static char FileIndexToChar(int file) => (char)('a' + file);

  public void ResetPieceBorderColor() => PieceBorderColor = defaultPieceBorderColor;

  public override string ToString() => $"{FileChar}{Rank + 1}";

  private void SetCoverageOpacity(ref float? opacity, float value, Image image) {
    opacity = Mathf.Clamp01(value);
    var color = image.color;
    color.a = opacity.Value;
    image.color = color;
  }

  #endregion

  #region Coroutines

  #endregion

  #region Handlers

  void IPointerClickHandler.OnPointerClick(PointerEventData eventData) => OnClicked.Invoke(this);

  void IBeginDragHandler.OnBeginDrag(PointerEventData eventData) => OnDragBegan.Invoke(this);

  public void OnDrag(PointerEventData eventData) { }

  void IEndDragHandler.OnEndDrag(PointerEventData eventData) => OnDragEnded.Invoke(this);

  void IDropHandler.OnDrop(PointerEventData eventData) => OnDropped.Invoke(this);

  void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData) => OnEntered.Invoke(this);

  void IPointerExitHandler.OnPointerExit(PointerEventData eventData) => OnExited.Invoke(this);

  #endregion

  #region Lifecycle

  private void Start() {
    image.color = IsWhite ? White : Black;
    HighlightVisible = false;
    BorderVisible = false;
    ScreenVisible = false;
    if (!opponentCoverageOpacity.HasValue) OpponentCoverageOpacity = 0f;
    if (!playerCoverageOpacity.HasValue) PlayerCoverageOpacity = 0f;
  }

  private void Awake() {
    name = ToString();
    image = GetComponent<Image>();
    defaultPieceIcon = pieceDisplay.icon.sprite;
    defaultPieceBorder = pieceDisplay.border.sprite;
    defaultPieceBorderColor = pieceDisplay.border.color;
  }

  #endregion
}
