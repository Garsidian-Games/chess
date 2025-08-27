using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class ExportGameWindow : MonoBehaviour {
  #region Constants

  #endregion

  #region Internal

  #endregion

  #region Fields

  [Header("References")]
  [SerializeField] private TMP_InputField input;
  [SerializeField] private TextMeshProUGUI message;
  [SerializeField] private Button button;

  private string pgn;

  #endregion

  #region Events

  #endregion

  #region Properties

  #endregion

  #region Methods

  public void Hide() => gameObject.SetActive(false);

  public void Show(string pgn) {
    message.enabled = false;
    this.pgn = pgn;
    gameObject.SetActive(true);
    input.text = pgn;
  }

  #endregion

  #region Coroutines

  #endregion

  #region Handlers

  private void HandleButtonClicked() {
    message.enabled = true;
    GUIUtility.systemCopyBuffer = pgn;
  }

  #endregion

  #region Lifecycle

  private void Start() {
    button.onClick.AddListener(HandleButtonClicked);
  }

  #endregion
}
