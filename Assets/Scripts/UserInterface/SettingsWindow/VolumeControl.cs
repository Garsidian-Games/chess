using UnityEngine;
using UnityEngine.UI;

public class VolumeControl : MonoBehaviour {
  #region Constants

  #endregion

  #region Internal

  #endregion

  #region Fields

  [SerializeField] private AudioChannel audioChannel;

  [SerializeField] private Button on;

  [SerializeField] private Button off;

  [SerializeField] private Slider slider;

  private SliderSound sliderSound;

  private bool syncing;

  #endregion

  #region Properties

  #endregion

  #region Methods

  public void Toggle() => audioChannel.IsMuted = !audioChannel.IsMuted;

  private void Sync(float volume) {
    on.gameObject.SetActive(volume > 0f);
    off.gameObject.SetActive(volume == 0f);

    syncing = true;
    if (sliderSound != null) sliderSound.enabled = false;
    slider.normalizedValue = volume;
    if (sliderSound != null) sliderSound.enabled = true;
    syncing = false;
  }

  #endregion

  #region Coroutines

  #endregion

  #region Handlers

  private void HandleSliderChanged(float value) {
    if (syncing) return;
    audioChannel.Volume = value;
  }

  #endregion

  #region Lifecycle

  private void Start() {
    slider.onValueChanged.AddListener(HandleSliderChanged);
    on.onClick.AddListener(() => audioChannel.IsMuted = true);
    off.onClick.AddListener(() => audioChannel.IsMuted = false);
    audioChannel.OnVolumeChanged.AddListener(Sync);
    Sync(audioChannel.Volume);
  }

  private void Awake() {
    sliderSound = slider.GetComponent<SliderSound>();
  }

  #endregion
}
