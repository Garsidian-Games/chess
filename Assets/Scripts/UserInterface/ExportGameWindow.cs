using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class ExportGameWindow : UIWindow {
  #region Constants

  #endregion

  #region Internal

  [System.Serializable] public class StringEvent : UnityEvent<string> { }

  #endregion

  #region Fields

  [Header("References")]
  [SerializeField] private TMP_InputField input;
  [SerializeField] private TextMeshProUGUI message;
  [SerializeField] private Button copy;

  private string pgn;

  #endregion

  #region Events

  [HideInInspector] public StringEvent OnCopied;

  #endregion

  #region Properties

  #endregion

  #region Methods

  public override void Toggle() => throw new System.InvalidOperationException();

  public override void Show() => throw new System.InvalidOperationException();

  public void Show(string pgn) {
    message.enabled = false;
    this.pgn = pgn;
    input.text = pgn;
    ChangeVisibility(true);
  }

  #endregion

  #region Coroutines

  #endregion

  #region Handlers

  private void HandleCopyClicked() {
    OnCopied.Invoke(pgn);
    message.enabled = true;
  }

  #endregion

  #region Lifecycle

  protected override void Start() {
    copy.onClick.AddListener(HandleCopyClicked);
  }

  #endregion
}
