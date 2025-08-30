using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameOverModal : UIModal {
  #region Constants

  #endregion

  #region Internal

  #endregion

  #region Fields

  [SerializeField] private GameObject whiteWins;
  [SerializeField] private GameObject blackWins;
  [SerializeField] private GameObject draw;
  [SerializeField] private Button undo;
  [SerializeField] private Button showScore;
  [SerializeField] private Button newGame;

  #endregion

  #region Events

  [HideInInspector] public UnityEvent OnUndo;
  [HideInInspector] public UnityEvent OnShowScore;
  [HideInInspector] public UnityEvent OnNewGame;

  #endregion

  #region Properties

  #endregion

  #region Methods

  public void Show(bool isDraw, SideType? loser) {
    draw.SetActive(isDraw);
    whiteWins.SetActive(!isDraw && loser == SideType.Black);
    blackWins.SetActive(!isDraw && loser == SideType.White);
    ChangeVisibility(true);
  }

  #endregion

  #region Coroutines

  #endregion

  #region Handlers

  #endregion

  #region Lifecycle

  private void Start() {
    undo.onClick.AddListener(OnUndo.Invoke);
    showScore.onClick.AddListener(OnShowScore.Invoke);
    newGame.onClick.AddListener(OnNewGame.Invoke);
  }

  #endregion
}
