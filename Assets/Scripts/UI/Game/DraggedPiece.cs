using UnityEngine;
using UnityEngine.UI;

public class DraggedPiece : MonoBehaviour {
  #region Constants

  #endregion

  #region Internal

  #endregion

  #region Fields

  [SerializeField] private Image icon;

  [SerializeField] private Image border;

  private Sprite defaultIcon;

  private Sprite defaultBorder;

  private Canvas canvas;

  private RectTransform rectTransform;

  private Piece piece;

  #endregion

  #region Events

  #endregion

  #region Properties

  public Piece Piece {
    get => piece;
    set {
      piece = value;
      icon.sprite = piece == null ? defaultIcon : piece.Icon;
      border.sprite = piece == null ? defaultBorder : piece.Border;
    }
  }

  #endregion

  #region Methods

  #endregion

  #region Coroutines

  #endregion

  #region Handlers

  #endregion

  #region Lifecycle

  private void Update() {
    Vector2 localPoint;
    RectTransformUtility.ScreenPointToLocalPointInRectangle(
        canvas.transform as RectTransform,
        Input.mousePosition,
        canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
        out localPoint
    );

    rectTransform.localPosition = localPoint;
  }

  private void Awake() {
    canvas = GetComponentInParent<Canvas>();
    rectTransform = GetComponent<RectTransform>();

    defaultIcon = icon.sprite;
    defaultBorder = border.sprite;
  }

  #endregion
}
