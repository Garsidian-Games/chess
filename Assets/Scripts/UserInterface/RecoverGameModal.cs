using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class RecoverGameModal : MonoBehaviour {
  #region Constants

  #endregion

  #region Internal

  #endregion

  #region Fields

  [SerializeField] private Button clear;

  [SerializeField] private Button load;

  #endregion

  #region Events

  [HideInInspector] public UnityEvent OnCleared;

  [HideInInspector] public UnityEvent OnLoaded;

  #endregion

  #region Properties

  #endregion

  #region Methods

  public void Show() => gameObject.SetActive(true);

  public void Hide() => gameObject.SetActive(false);

  #endregion

  #region Coroutines

  #endregion

  #region Handlers

  #endregion

  #region Lifecycle

  private void Start() {
    clear.onClick.AddListener(OnCleared.Invoke);
    load.onClick.AddListener(OnLoaded.Invoke);
  }

  #endregion
}
