using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MenuWindow : MonoBehaviour {
  #region Constants

  #endregion

  #region Internal

  #endregion

  #region Fields

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

  public void Hide() => gameObject.SetActive(false);

  public void Show() => gameObject.SetActive(true);

  #endregion

  #region Coroutines

  #endregion

  #region Handlers

  #endregion

  #region Lifecycle

  private void Start() {
    exportGame.onClick.AddListener(OnExportGame.Invoke);
    importGame.onClick.AddListener(OnImportGame.Invoke);
    newGame.onClick.AddListener(OnNewGame.Invoke);
  }

  #endregion
}
