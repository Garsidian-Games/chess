using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour {
  #region Constants

  private const float MinDb = -80f;

  private const float Threshold = 0.01f;

  #endregion

  #region Fields

  [Header("References")]
  [SerializeField] private AudioMixer audioMixer;

  [Header("Settings")]
  [SerializeField] private string paramMusicVolume = "MusicVolume";
  [SerializeField] private string paramSoundVolume = "SoundVolume";
  [SerializeField] [Range(0f, 1f)] private float defaultVolume = 0.5f;

  private float? lastMusicVolume;
  private float? lastSoundVolume;

  #endregion

  #region Properties

  public bool MusicMuted => GetVolume(paramMusicVolume) == 0f;

  public float MusicVolume {
    get => GetVolume(paramMusicVolume);
    set => SetVolume(paramMusicVolume, value);
  }

  public bool SoundMuted => GetVolume(paramSoundVolume) == 0f;

  public float SoundVolume {
    get => GetVolume(paramSoundVolume);
    set => SetVolume(paramSoundVolume, value);
  }

  #endregion

  #region Methods

  public bool ToggleMusic() => Toggle(paramMusicVolume, ref lastMusicVolume);

  public bool ToggleSound() => Toggle(paramSoundVolume, ref lastSoundVolume);

  private bool Toggle(string param, ref float? last) {
    var volume = GetVolume(param);
    if (volume < Threshold) {
      SetVolume(param, last ?? defaultVolume);
      last = null;
      return true;
    } else {
      last = volume;
      SetVolume(param, 0f);
      return false;
    }
  }

  private void SetVolume(string param, float volume) {
    volume = Mathf.Clamp01(volume);
    ApplyVolume(param, volume);
    PlayerPrefs.SetFloat(param, volume);
  }

  private void ApplyVolume(string param, float volume) {
    audioMixer.SetFloat(param, (volume <= Threshold) ? MinDb : Mathf.Log10(volume) * 20f);
  }

  private float GetVolume(string key) {
    if (!PlayerPrefs.HasKey(key)) PlayerPrefs.SetFloat(key, defaultVolume);
    return Mathf.Clamp01(PlayerPrefs.GetFloat(key));
  }

  #endregion

  #region Lifecycle

  private void Start() {
    ApplyVolume(paramSoundVolume, GetVolume(paramSoundVolume));
    ApplyVolume(paramMusicVolume, GetVolume(paramMusicVolume));
  }

  #endregion
}
