using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public sealed class MenuWindow : UIWindow {
  #region Constants

  #endregion

  #region Internal

  #endregion

  #region Fields

  [Header("Options")]
  [SerializeField] private Button exportGame;
  [SerializeField] private Button importGame;
  [SerializeField] private Button newGame;

  #endregion

  #region Events

  [HideInInspector] public UnityEvent OnExportGame;
  [HideInInspector] public UnityEvent OnImportGame;
  [HideInInspector] public UnityEvent OnNewGame;

  #endregion

  #region Properties

  public bool CanResetGame {
    get => newGame.interactable;
    set => newGame.interactable = value;
  }

  public bool CanExportGame {
    get => exportGame.interactable;
    set => exportGame.interactable = value;
  }

  #endregion

  #region Methods

  #endregion

  #region Coroutines

  #endregion

  #region Handlers

  #endregion

  #region Lifecycle

  protected override void Start() {
    base.Start();

    exportGame.onClick.AddListener(OnExportGame.Invoke);
    importGame.onClick.AddListener(OnImportGame.Invoke);
    newGame.onClick.AddListener(OnNewGame.Invoke);
  }

  #endregion
}
