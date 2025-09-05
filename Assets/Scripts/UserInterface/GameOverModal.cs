using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameOverModal : UIModal {
  #region Constants

  #endregion

  #region Internal

  [System.Serializable]
  public class Draw {
    public GameObject fiftyMoveRule;
    public GameObject threefoldRepetition;
    public GameObject insufficientMaterial;
    public GameObject stalemate;

    public void Reset() {
      fiftyMoveRule.SetActive(false);
      threefoldRepetition.SetActive(false);
      insufficientMaterial.SetActive(false);
      stalemate.SetActive(false);
    }
  }

  #endregion

  #region Fields

  [SerializeField] private GameObject whiteWins;
  [SerializeField] private GameObject blackWins;
  [SerializeField] private Draw draw;
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

  //public void Show(bool isDraw, SideType? loser) {
  public void Show(GameState gameState) {
    bool isDraw = gameState.IsDraw;
    SideType? loser = isDraw ? null : gameState.BoardState.SideToMove;

    draw.Reset();

    if (gameState.FiftyMoveRule) draw.fiftyMoveRule.SetActive(true);
    else if (gameState.ThreefoldRepetition) draw.threefoldRepetition.SetActive(true);
    else if (gameState.InsufficientMaterial) draw.insufficientMaterial.SetActive(true);
    else if (gameState.IsStalemate) draw.stalemate.SetActive(true);
    else if (isDraw) throw new System.InvalidOperationException("Unhandled draw state!!");

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
