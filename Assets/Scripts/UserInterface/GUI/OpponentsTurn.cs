using UnityEngine;

public class OpponentsTurn : MonoBehaviour {
  #region Constants

  #endregion

  #region Internal

  #endregion

  #region Fields

  [SerializeField] private GameObject thinkingSpinner;

  #endregion

  #region Events

  #endregion

  #region Properties

  public bool IsThinking {
    get => thinkingSpinner.activeSelf;
    set => thinkingSpinner.SetActive(value);
  }

  #endregion

  #region Methods

  public void Display(bool value) => gameObject.SetActive(value);

  #endregion

  #region Coroutines

  #endregion

  #region Handlers

  #endregion

  #region Lifecycle

  #endregion
}
