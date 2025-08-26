using UnityEngine;
using UnityEngine.UI;

public class CapturedPieces : MonoBehaviour {
  #region Constants

  #endregion

  #region Internal

  #endregion

  #region Fields
  [SerializeField] private Image[] images;

  #endregion

  #region Events

  #endregion

  #region Properties

  private Sprite blank;

  #endregion

  #region Methods

  public void Display(Piece[] pieces) {
    for (int i = 0; i < images.Length; i++) images[i].sprite = pieces.Length > i ? pieces[i].Icon : blank;
  }

  #endregion

  #region Coroutines

  #endregion

  #region Handlers

  #endregion

  #region Lifecycle

  private void Awake() {
    blank = images[0].sprite;
  }

  #endregion
}
