using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class PlayersTurn : MonoBehaviour {
  #region Constants

  private const string BusyText = "...";

  #endregion

  #region Internal

  #endregion

  #region Fields

  [SerializeField] private Button undo;

  [SerializeField] private Button hint;

  [SerializeField] private TextMeshProUGUI hintText;

  [SerializeField] private Button score;

  private string originalHintText;

  #endregion

  #region Events

  [HideInInspector] public UnityEvent OnUndo;

  [HideInInspector] public UnityEvent OnHint;

  [HideInInspector] public UnityEvent OnShowScore;

  #endregion

  #region Properties

  public bool HintBusy {
    get => hintText.text == BusyText;
    set => hintText.text = value ? BusyText : originalHintText;
  }

  #endregion

  #region Methods

  public void Display(bool isVisible, bool canUndo, bool hasScore) {
    gameObject.SetActive(isVisible);
    undo.interactable = canUndo;
    score.interactable = hasScore;
  }

  #endregion

  #region Coroutines

  #endregion

  #region Handlers

  #endregion

  #region Lifecycle

  private void Start() {
    undo.onClick.AddListener(OnUndo.Invoke);
    hint.onClick.AddListener(OnHint.Invoke);
    score.onClick.AddListener(OnShowScore.Invoke);
  }

  private void Awake() {
    originalHintText = hintText.text;
  }

  #endregion
}
