using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ReadyCheckModal : MonoBehaviour {
  #region Constants

  #endregion

  #region Internal

  #endregion

  #region Fields

  [SerializeField] private Button button;

  #endregion

  #region Events

  [HideInInspector] public UnityEvent OnReady;

  #endregion

  #region Properties

  #endregion

  #region Methods

  public void Display(bool value) => gameObject.SetActive(value);

  #endregion

  #region Coroutines

  #endregion

  #region Handlers

  private void HandleButtonClick() {
    OnReady.Invoke();
    gameObject.SetActive(false);
  }

  #endregion

  #region Lifecycle

  private void Start() {
    button.onClick.AddListener(HandleButtonClick);
  }

  #endregion
}
