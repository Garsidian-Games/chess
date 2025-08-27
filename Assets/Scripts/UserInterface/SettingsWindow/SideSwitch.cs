using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using UnityEngine.UI;

public class SideSwitch : MonoBehaviour {
  #region Constants

  #endregion

  #region Internal

  #endregion

  #region Fields

  [SerializeField] private Button white;

  [SerializeField] private Button black;

  private Player player;

  private GameController gameController;

  private SideType? buffered;

  #endregion

  #region Events

  public UnityEvent OnConfirmationRequired;

  #endregion

  #region Properties

  #endregion

  #region Methods

  private void Sync(SideType sideType) {
    white.gameObject.SetActive(sideType == SideType.White);
    black.gameObject.SetActive(sideType == SideType.Black);
  }

  private void TryChangeSide(SideType sideType) {
    buffered = sideType;
    if (gameController.GameManager.GameState.BoardState.IsRoot) {
      player.SideType = buffered.Value;
      buffered = null;
    } else OnConfirmationRequired.Invoke();
  }

  #endregion

  #region Coroutines

  #endregion

  #region Handlers

  public void SwitchAndReset() {
    gameController.GameManager.ClearSave();
    player.SideType = buffered.Value;
    buffered = null;
    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
  }

  private void HandleWhiteClicked() => TryChangeSide(SideType.Black);

  private void HandleBlackClicked() => TryChangeSide(SideType.White);

  #endregion

  #region Lifecycle

  private void Start() {
    white.onClick.AddListener(HandleWhiteClicked);
    black.onClick.AddListener(HandleBlackClicked);
    player.OnSideChanged.AddListener(Sync);
    Sync(player.SideType);
  }

  private void Awake() {
    player = Player.Instance;
    gameController = GameController.Instance;
  }

  #endregion
}
