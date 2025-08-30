using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using UnityEngine.UI;

public class TimeToggle : MonoBehaviour {
  #region Constants

  #endregion

  #region Internal

  [System.Serializable] public class BoolEvent : UnityEvent<bool> { } 

  #endregion

  #region Fields

  [SerializeField] private Button on;

  [SerializeField] private Button off;

  #endregion

  #region Events

  [HideInInspector] public BoolEvent OnTimeToggled;

  #endregion

  #region Properties

  #endregion

  #region Methods

  public void Sync(bool isOn) {
    on.gameObject.SetActive(isOn);
    off.gameObject.SetActive(!isOn);
  }

  #endregion

  #region Coroutines

  #endregion

  #region Handlers

  private void HandleOnClicked() {
    OnTimeToggled.Invoke(true);
    Sync(false);
  }

  private void HandleOffClicked() {
    OnTimeToggled.Invoke(false);
    Sync(true);
  }

  #endregion

  #region Lifecycle

  private void Start() {
    on.onClick.AddListener(HandleOnClicked);
    off.onClick.AddListener(HandleOffClicked);
  }

  #endregion
}
