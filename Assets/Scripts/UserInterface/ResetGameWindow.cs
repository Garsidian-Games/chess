using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ResetGameWindow : UIWindow {
  #region Constants

  #endregion

  #region Internal

  #endregion

  #region Fields

  [Header("Reset Game References")]
  [SerializeField] private Button reset;

  #endregion

  #region Events

  [HideInInspector] public UnityEvent OnReset;

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

  protected override void Start() {
    reset.onClick.AddListener(OnReset.Invoke);
  }

  #endregion
}
