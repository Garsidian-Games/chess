using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PickPromotionModal : MonoBehaviour {
  #region Constants

  #endregion

  #region Internal

  [System.Serializable] public class PieceTypeEvent : UnityEvent<PieceType> { }

  [System.Serializable]
  public class PieceImage {
    public Image queen;
    public Image knight;
    public Image bishop;
    public Image rook;
  }

  #endregion

  #region Fields

  [SerializeField] private PieceImage white;

  [SerializeField] private PieceImage black;

  #endregion

  #region Events

  [HideInInspector] public PieceTypeEvent OnPicked;

  #endregion

  #region Properties

  #endregion

  #region Methods

  public void Show() => gameObject.SetActive(true);

  public void Sync(SideType sideType) {
    white.queen.enabled = sideType == SideType.White;
    white.knight.enabled = sideType == SideType.White;
    white.bishop.enabled = sideType == SideType.White;
    white.rook.enabled = sideType == SideType.White;

    black.queen.enabled = sideType == SideType.Black;
    black.knight.enabled = sideType == SideType.Black;
    black.bishop.enabled = sideType == SideType.Black;
    black.rook.enabled = sideType == SideType.Black;
  }

  public void Queen() => Pick(PieceType.Queen);

  public void Knight() => Pick(PieceType.Knight);

  public void Bishop() => Pick(PieceType.Bishop);

  public void Rook() => Pick(PieceType.Rook);

  private void Pick(PieceType pieceType) {
    gameObject.SetActive(false);
    OnPicked.Invoke(pieceType);
  }

  #endregion

  #region Coroutines

  #endregion

  #region Handlers

  #endregion

  #region Lifecycle

  #endregion
}
