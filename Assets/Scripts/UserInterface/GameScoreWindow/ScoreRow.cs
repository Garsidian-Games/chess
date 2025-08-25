using UnityEngine;
using TMPro;

public class ScoreRow : MonoBehaviour {
  #region Constants

  #endregion

  #region Internal

  #endregion

  #region Fields

  [SerializeField] private TextMeshProUGUI number;
  [SerializeField] private TextMeshProUGUI white;
  [SerializeField] private TextMeshProUGUI black;

  #endregion

  #region Events

  #endregion

  #region Properties

  public string NumberText {
    get => number.text;
    set => number.text = value;
  }

  public string WhiteText {
    get => white.text;
    set => white.text = value;
  }

  public string BlackText {
    get => black.text;
    set => black.text = value;
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

  #endregion

  #region Lifecycle

  private void Start() {
    NumberText = $"{transform.GetSiblingIndex() + 1}.";
  }

  #endregion
}
