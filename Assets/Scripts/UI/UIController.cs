using UnityEngine;
using UnityEngine.Events;

public class UIController : MonoBehaviour {
  #region Constants

  #endregion

  #region Internal

  #endregion

  #region Fields

  [Header("References")]
  [SerializeField] private Board board;
  [SerializeField] private GameObject loadingScreen;

  private GameController gameController;

  #endregion

  #region Events

  [HideInInspector] public UnityEvent OnReady;

  #endregion

  #region Properties

  public static UIController Instance => GameObject.FindWithTag("UIController").GetComponent<UIController>();

  public bool IsReady { get; private set; }

  #endregion

  #region Methods

  private void ReadyUp() {
    if (IsReady) return;
    if (!board.IsReady) return;

    IsReady = true;
    OnReady.Invoke();
  }

  #endregion

  #region Coroutines

  #endregion

  #region Handlers

  private void HandleGameReady() {
    loadingScreen.SetActive(false);
  }

  #endregion

  #region Lifecycle

  private void Start() {
    if (gameController.IsReady) HandleGameReady();
    else gameController.OnReady.AddListener(HandleGameReady);

    if (board.IsReady) ReadyUp();
    else board.OnReady.AddListener(ReadyUp);
  }

  private void Awake() {
    loadingScreen.SetActive(true);

    gameController = GameController.Instance;
  }

  #endregion
}
