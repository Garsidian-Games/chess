using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "Audio Channel", menuName = "Audio/AudioChannel")]
public class AudioChannel : ScriptableObject {
  #region Constants

  private const float MinDb = -80f;

  private const float Threshold = 0.01f;

  #endregion

  #region Internal

  [System.Serializable] public class VolumeChangeEvent : UnityEvent<float> { }

  [System.Serializable] public class VolumeToggleEvent : UnityEvent<bool> { }

  #endregion

  #region Fields

  [SerializeField] private AudioMixer audioMixer;

  [SerializeField] [Range(0f, 1f)] private float defaultVolume = 0.5f;

  private float volume;

  private string volumeParam;

  private float? volumeWhenMuted;

  #endregion

  #region Events

  [HideInInspector] public VolumeChangeEvent OnVolumeChanged;

  #endregion

  #region Properties

  public float Volume {
    get {
      if (!PlayerPrefs.HasKey(volumeParam)) PlayerPrefs.SetFloat(volumeParam, defaultVolume);
      return Mathf.Clamp01(PlayerPrefs.GetFloat(volumeParam));
    }

    set {
      volume = Mathf.Clamp01(value);
      ApplyVolume(volumeParam, volume);
      PlayerPrefs.SetFloat(volumeParam, volume);
      OnVolumeChanged.Invoke(Volume);
    }
  }

  public bool IsMuted {
    get => Volume == 0f;
    set {
      if (value == IsMuted) return;
      if (value && Volume > Threshold) volumeWhenMuted = Volume;
      Volume = value ? 0f : volumeWhenMuted ?? defaultVolume;
      if (!value) volumeWhenMuted = null;
    }
  }

  public string VolumeParam {
    get {
      if (string.IsNullOrEmpty(volumeParam)) volumeParam = $"{name}Volume";
      return volumeParam;
    }
  }

  #endregion

  #region Methods

  public void Initialize() {
    ApplyVolume(VolumeParam, Volume);
  }

  private void ApplyVolume(string param, float volume) {
    audioMixer.SetFloat(param, (volume <= Threshold) ? MinDb : Mathf.Log10(volume) * 20f);
  }

  #endregion

  #region Coroutines

  #endregion

  #region Handlers

  #endregion

  #region Lifecycle

  #endregion
}
