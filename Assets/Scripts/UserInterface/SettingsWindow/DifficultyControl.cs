using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class DifficultyControl : MonoBehaviour {
  #region Constants

  #endregion

  #region Internal

  [System.Serializable] public class DepthStepEvent : UnityEvent<int> { }

  [System.Serializable]
  public class Difficulty {
    public string text;
    public Color color;
  }

  #endregion

  #region Fields

  [SerializeField] private Slider slider;
  [SerializeField] private TextMeshProUGUI text;
  [SerializeField] private Difficulty[] difficulties;

  private bool syncing;

  #endregion

  #region Events

  [HideInInspector] public DepthStepEvent OnDepthStepChanged;

  #endregion

  #region Properties

  #endregion

  #region Methods

  public void Sync(int depthStep) {
    syncing = true;
    slider.value = depthStep;
    SyncText(depthStep);
    syncing = false;
  }

  private void SyncText(int depthStep) {
    var label = difficulties[depthStep - 1];
    text.text = label.text;
    text.color = label.color;
  }

  #endregion

  #region Coroutines

  #endregion

  #region Handlers

  private void HandleSliderChanged(float value) {
    if (syncing) return;
    int depthStep = Mathf.FloorToInt(value);
    SyncText(depthStep);
    OnDepthStepChanged.Invoke(depthStep);
  }

  #endregion

  #region Lifecycle

  private void Awake() {
    slider.minValue = Opponent.DepthStepMin;
    slider.maxValue = Opponent.DepthStepCount;
    slider.wholeNumbers = true;
    slider.onValueChanged.AddListener(HandleSliderChanged);
  }

  #endregion
}
