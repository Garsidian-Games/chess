using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public sealed class ImportGameWindow : UIWindow {
  #region Constants

  #endregion

  #region Internal

  [System.Serializable] public class PortableGameNotationEvent : UnityEvent<PortableGameNotation> { }

  [System.Serializable]
  public class MessageText {
    public string prompt = "Please enter the PGN you wish to import.";
    public string errorInvalid = "Error: Entered PGN is invalid!";
    public string warningCheckmate = "Warning: Entered PGN ends in checkmate!";
    public string success = "PGN is valid.";
  }

  [System.Serializable]
  public class MessageColor {
    public Color success;
    public Color standard;
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
  [SerializeField] private Button import;
  [SerializeField] private Button importAnyway;

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

  #endregion

  #region Methods

  public void Configure(Board board, Side white, Side black) {
    this.board = board;
    this.white = white;
    this.black = black;
  }

  public void SetMessage(string text, Color color) {
    message.text = text;
    message.color = color;
  }

  protected override void OnShow() {
    input.text = string.Empty;
    import.interactable = false;
    SetMessage(messageText.prompt, messageColor.standard);
    base.OnShow();
  }

  private void Setup() {
    import.gameObject.SetActive(true);
    importAnyway.gameObject.SetActive(false);

    import.interactable = false;

    if (string.IsNullOrWhiteSpace(input.text)) {
      SetMessage(messageText.prompt, messageColor.standard);
      return;
    }

    pgn = new(board, white, black, input.text);

    if (!pgn.IsValid) {
      SetMessage(messageText.errorInvalid, messageColor.error);
      return;
    }

    if (pgn.EndsInCheckmate) {
      import.gameObject.SetActive(false);
      importAnyway.gameObject.SetActive(true);
      SetMessage(messageText.warningCheckmate, messageColor.warning);
      return;
    }

    SetMessage(messageText.success, messageColor.success);
    import.interactable = true;
  }

  #endregion

  #region Coroutines

  #endregion

  #region Handlers

  private void HandleImportClicked() {
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

  protected override void Start() {
    base.Start();
    import.onClick.AddListener(HandleImportClicked);
    importAnyway.onClick.AddListener(HandleImportClicked);
    input.onValueChanged.AddListener(HandleInputValueChanged);
  }

  #endregion
}
