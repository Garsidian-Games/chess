using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public abstract class BaseResetGameModal : UIModal {
  #region Constants

  #endregion

  #region Internal

  #endregion

  #region Fields

  [Header("Actions")]
  [SerializeField] private Button resume;
  [SerializeField] private Button reset;

  #endregion

  #region Events

  [HideInInspector] public UnityEvent OnResume;
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

  protected virtual void Start() {
    resume.onClick.AddListener(OnResume.Invoke);
    reset.onClick.AddListener(OnReset.Invoke);
  }

  #endregion
}
