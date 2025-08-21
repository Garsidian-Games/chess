using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour {
  #region Constants

  #endregion

  #region Internal

  #endregion

  #region Fields

  #endregion

  #region Events

  [HideInInspector] public UnityEvent OnReady;

  #endregion

  #region Properties

  public bool IsReady { get; private set; }

  #endregion

  #region Methods

  private void ReadyUp() {
    if (IsReady) return;
    IsReady = true;
    OnReady.Invoke();
  }

  #endregion

  #region Coroutines

  #endregion

  #region Handlers

  #endregion

  #region Lifecycle

  private void Start() {
    ReadyUp();
  }

  #endregion
}
