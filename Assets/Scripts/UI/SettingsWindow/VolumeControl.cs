using UnityEngine;
using UnityEngine.UI;

public class VolumeControl : MonoBehaviour {
  #region Constants

  #endregion

  #region Internal

  #endregion

  #region Fields

  [SerializeField] private Button on;

  [SerializeField] private Button off;

  [SerializeField] private AudioChannel audioChannel;

  #endregion

  #region Properties

  #endregion

  #region Methods

  public void Toggle() => audioChannel.IsMuted = !audioChannel.IsMuted;

  private void Sync(float volume) {
    on.gameObject.SetActive(volume > 0f);
    off.gameObject.SetActive(volume == 0f);
  }

  #endregion

  #region Coroutines

  #endregion

  #region Handlers

  #endregion

  #region Lifecycle

  private void Start() {
    on.onClick.AddListener(() => audioChannel.IsMuted = true);
    off.onClick.AddListener(() => audioChannel.IsMuted = false);
    audioChannel.OnVolumeChanged.AddListener(Sync);
    Sync(audioChannel.Volume);
  }

  #endregion
}
