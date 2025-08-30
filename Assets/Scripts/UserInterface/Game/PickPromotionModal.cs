using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public sealed class PickPromotionModal : UIModal {
  #region Constants

  #endregion

  #region Internal

  [System.Serializable] public class PieceTypeEvent : UnityEvent<PieceType> { }

  [System.Serializable]
  public class ImageButton {
    public Button button;
    public Image white;
    public Image black;

    public void ShowFor(SideType sideType) {
      white.enabled = sideType == SideType.White;
      black.enabled = sideType == SideType.Black;
    }
  }

  [System.Serializable]
  public class PieceImage {
    public ImageButton queen;
    public ImageButton knight;
    public ImageButton bishop;
    public ImageButton rook;
  }

  #endregion

  #region Fields

  [Header("Actions")]
  [SerializeField] private ImageButton queen;
  [SerializeField] private ImageButton knight;
  [SerializeField] private ImageButton bishop;
  [SerializeField] private ImageButton rook;

  #endregion

  #region Events

  [HideInInspector] public PieceTypeEvent OnPicked;

  #endregion

  #region Properties

  #endregion

  #region Methods

  public override void Toggle() => throw new System.InvalidOperationException();

  public override void Show() => throw new System.InvalidOperationException();

  public void Show(SideType sideType) {
    queen.ShowFor(sideType);
    knight.ShowFor(sideType);
    bishop.ShowFor(sideType);
    rook.ShowFor(sideType);

    ChangeVisibility(true);
  }

  private void Pick(PieceType pieceType) {
    gameObject.SetActive(false);
    OnPicked.Invoke(pieceType);
  }

  #endregion

  #region Coroutines

  #endregion

  #region Handlers

  private void HandleQueenClicked() => Pick(PieceType.Queen);

  private void HandleKnightClicked() => Pick(PieceType.Knight);

  private void HandleBishopClicked() => Pick(PieceType.Bishop);

  private void HandleRookClicked() => Pick(PieceType.Rook);

  #endregion

  #region Lifecycle

  private void Start() {
    queen.button.onClick.AddListener(HandleQueenClicked);
    knight.button.onClick.AddListener(HandleKnightClicked);
    bishop.button.onClick.AddListener(HandleBishopClicked);
    rook.button.onClick.AddListener(HandleRookClicked);
  }

  #endregion
}
