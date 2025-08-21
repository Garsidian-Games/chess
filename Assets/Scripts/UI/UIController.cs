using UnityEngine;

public class UIController : MonoBehaviour {
  #region Constants

  #endregion

  #region Internal

  #endregion

  #region Fields

  [Header("References")]
  [SerializeField] private GameObject loadingScreen;

  #endregion

  #region Events

  #endregion

  #region Properties

  #endregion

  #region Methods

  #endregion

  #region Coroutines

  #endregion

  #region Handlers

  #endregion

  #region Lifecycle

  private void Awake() {
    loadingScreen.SetActive(true);
  }

  #endregion
}
