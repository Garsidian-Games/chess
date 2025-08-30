using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using UnityEngine.UI;

public class SideSwitcher : MonoBehaviour {
  #region Constants

  #endregion

  #region Internal

  [System.Serializable] public class SideTypeEvent : UnityEvent<SideType> { } 

  #endregion

  #region Fields

  [SerializeField] private Button white;

  [SerializeField] private Button black;

  #endregion

  #region Events

  [HideInInspector] public SideTypeEvent OnSwitchClicked;

  #endregion

  #region Properties

  #endregion

  #region Methods

  public void Sync(SideType sideType) {
    white.gameObject.SetActive(sideType == SideType.White);
    black.gameObject.SetActive(sideType == SideType.Black);
  }

  #endregion

  #region Coroutines

  #endregion

  #region Handlers

  private void HandleWhiteClicked() => OnSwitchClicked.Invoke(SideType.White);

  private void HandleBlackClicked() => OnSwitchClicked.Invoke(SideType.Black);

  #endregion

  #region Lifecycle

  private void Start() {
    white.onClick.AddListener(HandleWhiteClicked);
    black.onClick.AddListener(HandleBlackClicked);
  }

  #endregion
}
