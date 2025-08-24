using UnityEngine;
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

  #endregion

  #region Events

  #endregion

  #region Properties

  #endregion

  #region Methods

  private void Sync(SideType side) {
    white.gameObject.SetActive(side == SideType.White);
    black.gameObject.SetActive(side == SideType.Black);
  }

  #endregion

  #region Coroutines

  #endregion

  #region Handlers

  private void HandleWhiteClicked() => player.Side = SideType.Black;

  private void HandleBlackClicked() => player.Side = SideType.White;

  #endregion

  #region Lifecycle

  private void Start() {
    white.onClick.AddListener(HandleWhiteClicked);
    black.onClick.AddListener(HandleBlackClicked);
    player.OnSideChanged.AddListener(Sync);
    Sync(player.Side);
  }

  private void Awake() {
    player = Player.Instance;
  }

  #endregion
}
