using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameOverModal : MonoBehaviour {
  #region Constants

  #endregion

  #region Internal

  #endregion

  #region Fields

  [SerializeField] private GameObject whiteWins;
  [SerializeField] private GameObject blackWins;
  [SerializeField] private Button showScore;

  #endregion

  #region Events

  [HideInInspector] public UnityEvent OnShowScore;

  #endregion

  #region Properties

  #endregion

  #region Methods

  public void Show(SideType loser) {
    gameObject.SetActive(true);
    whiteWins.SetActive(loser == SideType.Black);
    blackWins.SetActive(loser == SideType.White);
  }

  #endregion

  #region Coroutines

  #endregion

  #region Handlers

  #endregion

  #region Lifecycle

  private void Start() {
    showScore.onClick.AddListener(OnShowScore.Invoke);
  }

  #endregion
}
