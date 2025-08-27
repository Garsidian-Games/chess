using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class ImportGameWindow : MonoBehaviour {
  #region Constants

  #endregion

  #region Internal

  [System.Serializable] public class PortableGameNotationEvent : UnityEvent<PortableGameNotation> { }

  [System.Serializable]
  public class MessageText {
    public string prompt = "Please enter the PGN you wish to import.";
    public string errorInvalid = "Entered PGN is invalid!";
    public string errorCheckmate = "Entered PGN ends in checkmate!";
    public string success = "PGN is valid.";
  }

  [System.Serializable]
  public class MessageColor {
    public Color success;
    public Color warning;
    public Color error;
  }

  #endregion

  #region Fields

  [Header("Settings")]
  [SerializeField] private MessageText messageText;
  [SerializeField] private MessageColor messageColor;

  [Header("References")]
  [SerializeField] private TMP_InputField input;
  [SerializeField] private TextMeshProUGUI message;
  [SerializeField] private Button button;

  private Board board;
  private Side white;
  private Side black;
  private PortableGameNotation pgn;

  private bool dirty;

  #endregion

  #region Events

  [HideInInspector] public PortableGameNotationEvent OnImported;

  #endregion

  #region Properties

  public void Configure(Board board, Side white, Side black) {
    this.board = board;
    this.white = white;
    this.black = black;
  }

  #endregion

  #region Methods

  public void Hide() => gameObject.SetActive(false);

  public void Show() {
    gameObject.SetActive(true);
    input.text = string.Empty;
    button.interactable = false;
    SetMessage(messageText.prompt, messageColor.warning);
  }

  public void SetMessage(string text, Color color) {
    message.text = text;
    message.color = color;
  }

  private void Setup() {
    button.interactable = false;

    if (string.IsNullOrWhiteSpace(input.text)) {
      SetMessage(messageText.prompt, messageColor.warning);
      return;
    }

    pgn = new(board, white, black, input.text);

    if (!pgn.IsValid) {
      SetMessage(messageText.errorInvalid, messageColor.error);
      return;
    }

    if (pgn.EndsInCheckmate) {
      SetMessage(messageText.errorCheckmate, messageColor.error);
      return;
    }

    SetMessage(messageText.success, messageColor.success);
    button.interactable = true;
  }

  #endregion

  #region Coroutines

  #endregion

  #region Handlers

  private void HandleButtonClicked() {
    OnImported.Invoke(pgn);
  }

  private void HandleInputValueChanged(string value) {
    dirty = true;
  }

  #endregion

  #region Lifecycle

  private void LateUpdate() {
    if (dirty) {
      dirty = false;
      Setup();
    }
  }

  private void Start() {
    button.onClick.AddListener(HandleButtonClicked);
    input.onValueChanged.AddListener(HandleInputValueChanged);
  }

  #endregion
}
