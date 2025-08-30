using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class ScoreRow : MonoBehaviour {
  #region Constants

  #endregion

  #region Internal

  [System.Serializable] public class ScoreEvent : UnityEvent<int, SideType> { }

  [System.Serializable]
  public class SideButton {
    public Button button;
    public TextMeshProUGUI text;
  }

  #endregion

  #region Fields

  [SerializeField] private TextMeshProUGUI number;
  [SerializeField] private SideButton white;
  [SerializeField] private SideButton black;

  private int index;

  #endregion

  #region Events

  [HideInInspector] public ScoreEvent OnScoreClicked;

  #endregion

  #region Properties

  public string NumberText {
    get => number.text;
    set => number.text = value;
  }

  public string WhiteText {
    get => white.text.text;
    set => white.text.text = value;
  }

  public string BlackText {
    get => black.text.text;
    set => black.text.text = value;
  }

  #endregion

  #region Methods

  public void Reset() {
    WhiteText = string.Empty;
    BlackText = string.Empty;
  }

  public void Hide() {
    gameObject.SetActive(false);
    Reset();
  }

  public void Show() {
    gameObject.SetActive(true);
  }

  #endregion

  #region Coroutines

  #endregion

  #region Handlers

  private void HandleWhiteClicked() {
    OnScoreClicked.Invoke(index, SideType.White);
  }

  private void HandleBlackClicked() {
    OnScoreClicked.Invoke(index, SideType.Black);
  }

  #endregion

  #region Lifecycle

  private void Start() {
    index = transform.GetSiblingIndex() + 1;
    white.button.onClick.AddListener(HandleWhiteClicked);
    black.button.onClick.AddListener(HandleBlackClicked);
    NumberText = $"{index}.";
  }

  #endregion
}
