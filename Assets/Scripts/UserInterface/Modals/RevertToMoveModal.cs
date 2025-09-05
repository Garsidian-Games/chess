using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class RevertToMoveModal : UIModal {
  #region Constants

  #endregion

  #region Internal

  #endregion

  #region Fields

  [Header("Actions")]
  [SerializeField] private Button resume;
  [SerializeField] private Button revert;

  #endregion

  #region Events

  [HideInInspector] public UnityEvent OnResume;
  [HideInInspector] public UnityEvent OnRevert;

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
    revert.onClick.AddListener(OnRevert.Invoke);
  }

  #endregion
}
