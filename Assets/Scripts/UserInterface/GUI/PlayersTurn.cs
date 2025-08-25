using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PlayersTurn : MonoBehaviour {
  #region Constants

  #endregion

  #region Internal

  #endregion

  #region Fields

  [SerializeField] private Button undo;

  [SerializeField] private Button score;

  #endregion

  #region Events

  [HideInInspector] public UnityEvent OnUndo;

  [HideInInspector] public UnityEvent OnShowScore;

  #endregion

  #region Properties

  #endregion

  #region Methods

  public void Display(bool isVisible, bool canUndo, bool hasScore) {
    gameObject.SetActive(isVisible);
    undo.interactable = canUndo;
    score.interactable = hasScore;
  }

  #endregion

  #region Coroutines

  #endregion

  #region Handlers

  #endregion

  #region Lifecycle

  private void Start() {
    undo.onClick.AddListener(OnUndo.Invoke);
    score.onClick.AddListener(OnShowScore.Invoke);
  }

  #endregion
}
