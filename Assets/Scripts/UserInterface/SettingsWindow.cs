using UnityEngine;
using UnityEngine.Events;

public sealed class SettingsWindow : UIWindow {
  #region Constants

  #endregion

  #region Internal

  #endregion

  #region Fields

  [Header("Settings References")]
  [SerializeField] private SideSwitcher side;
  [SerializeField] private DifficultyControl difficulty;
  [SerializeField] private VolumeControl sound;
  [SerializeField] private VolumeControl music;
  [SerializeField] private ChangeSideModal changeSideModal;

  private SideType? sideToSwitchTo;

  #endregion

  #region Events

  [HideInInspector] public UnityEvent OnInnerModalClosed;
  [HideInInspector] public SideSwitcher.SideTypeEvent OnSideSwitched;
  [HideInInspector] public DifficultyControl.DepthStepEvent OnDepthStepChanged;
  [HideInInspector] public UnityEvent OnReset;

  #endregion

  #region Properties

  [HideInInspector] public bool GameInProgress;

  #endregion

  #region Methods

  public void Sync(SideType sideType, int depthStep) {
    side.Sync(sideType);
    difficulty.Sync(depthStep);
  }

  #endregion

  #region Coroutines

  #endregion

  #region Handlers

  private void HandleResume() {
    changeSideModal.Hide();
    OnInnerModalClosed.Invoke();
  }

  private void HandleSideSwitchReset() {
    if (!sideToSwitchTo.HasValue) throw new System.InvalidOperationException("Undetermined sideToSwitchTo");
    var sideType = sideToSwitchTo.Value;
    sideToSwitchTo = null;
    OnSideSwitched.Invoke(sideType);
    OnReset.Invoke();
  }

  private void HandleSideSwitch(SideType sideType) {
    if (GameInProgress) {
      sideToSwitchTo = sideType;
      changeSideModal.Show();
    } else OnSideSwitched.Invoke(sideType);
  } 

  #endregion

  #region Lifecycle

  protected override void Start() {
    base.Start();
    side.OnSwitchClicked.AddListener(HandleSideSwitch);
    difficulty.OnDepthStepChanged.AddListener(OnDepthStepChanged.Invoke);
    changeSideModal.OnResume.AddListener(HandleResume);
    changeSideModal.OnReset.AddListener(HandleSideSwitchReset);
  }

  #endregion
}
