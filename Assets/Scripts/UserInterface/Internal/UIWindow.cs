using UnityEngine;
using UnityEngine.UI;

public abstract class UIWindow : DisplayToggle {
  #region Constants

  #endregion

  #region Internal

  #endregion

  #region Fields

  [Header("Window References")]
  [SerializeField] private Button close;

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

  protected virtual void Start() {
    close.onClick.AddListener(Hide);
  }

  #endregion
}
