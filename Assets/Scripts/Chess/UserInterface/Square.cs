using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Image))]
public class Square : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler, IDragHandler, IDropHandler {
  #region Constants

  public static Color Black => new(0.99f, 0.99f, 0.99f);

  public static Color White => new(1.00f, 1.00f, 1.00f);

  #endregion

  #region Internal

  [System.Serializable] public class SquareEvent : UnityEvent<Square> { }

  [System.Serializable]
  public class Coverage {
    public Image opponent;
    public Image player;
  }

  [System.Serializable]
  public class Text {
    public TextMeshProUGUI topLeft;
    public TextMeshProUGUI center;
    public TextMeshProUGUI bottomRight;
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
    public Image alertBlocked;
  }

  #endregion

  #region Fields

  [SerializeField] private Coverage coverage;
  [SerializeField] private Image highlight;
  [SerializeField] private Image border;
  [SerializeField] private PieceDisplay pieceDisplay;
  [SerializeField] private Image screen;
  [SerializeField] private Text text;

  private Image image;

  private int? index;

  private Sprite defaultPieceIcon;
  private Sprite defaultPieceBorder;
  private Color defaultPieceBorderColor;

  private Piece piece;

  private SquareState applied;

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

  public Piece Piece {
    get => piece;
    set {
      piece = value;
      pieceDisplay.icon.sprite = piece == null ? defaultPieceIcon : piece.Icon;
      pieceDisplay.border.sprite = piece == null ? defaultPieceBorder : piece.Border;
    }
  }

  #endregion

  #region Methods

  public static char FileIndexToChar(int file) => (char)('a' + file);

  public void Reset() {
    if (applied == null) return;
    Apply(applied);
  }

  public void Apply(SquareState state) {
    applied = state;

    SetImageColorOpacity(coverage.opponent, state.OpponentCoverageOpacity);
    SetImageColorOpacity(coverage.player, state.PlayerCoverageOpacity);
    SetTextColorOpacity(text.center, state.CenterTextOpacity);

    if (state.BorderColor.HasValue) border.color = state.BorderColor.Value;
    pieceDisplay.border.color = state.PieceBorderColor ?? defaultPieceBorderColor;

    highlight.enabled = state.HighlightVisible;
    border.enabled = state.BorderVisible;
    screen.enabled = state.ScreenVisible;
    text.center.enabled = state.TextVisible;

    pieceDisplay.wobble.enabled = state.WobblePiece;
    pieceDisplay.pulse.enabled = state.PulsePiece;
    pieceDisplay.tremble.enabled = state.TremblePiece;
    pieceDisplay.alertGreen.enabled = state.GreenAlert;
    pieceDisplay.alertRed.enabled = state.RedAlert;
    pieceDisplay.alertBlocked.enabled = state.BlockedAlert;
    pieceDisplay.icon.enabled = state.PieceVisible;
    pieceDisplay.border.sprite = state.PieceBorder == null ? (piece == null ? defaultPieceBorder : piece.Border) : state.PieceBorder.Border;
  }

  public override string ToString() => $"{FileChar}{Rank + 1}";

  private void SetImageColorOpacity(Image image, float value) {
    var color = image.color;
    color.a = Mathf.Clamp01(value);
    image.color = color;
  }

  private void SetTextColorOpacity(TextMeshProUGUI text, float value) {
    var color = text.color;
    color.a = Mathf.Clamp01(value);
    text.color = color;
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
    if (File == 0) text.topLeft.text = $"{Rank + 1}";
    text.center.text = name;
    if (Rank == 0) text.bottomRight.text = $"{FileChar}";
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
