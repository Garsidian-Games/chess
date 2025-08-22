using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour {
  #region Constants

  private const float MinDb = -80f;

  private const float Threshold = 0.01f;

  #endregion

  #region Internal

  #endregion

  #region Fields

  [Header("References")]
  [SerializeField] private AudioMixer audioMixer;
  [SerializeField] private AudioSource musicPlayer;
  [SerializeField] private AudioSource soundPlayer;

  [Header("Settings")]
  [SerializeField] private AudioChannel music;
  [SerializeField] private AudioChannel sound;

  private readonly Queue<AudioSource> soundsIdle = new();

  #endregion

  #region Events

  #endregion

  #region Properties

  public bool MusicMuted {
    get => music.IsMuted;
    set => music.IsMuted = value;
  }

  public float MusicVolume {
    get => music.Volume;
    set => music.Volume = value;
  }

  public bool SoundMuted {
    get => sound.IsMuted;
    set => sound.IsMuted = value;
  }

  public float SoundVolume {
    get => sound.Volume;
    set => sound.Volume = value;
  }

  #endregion

  #region Methods

  public void PlayMusic(AudioResource resource) {
    musicPlayer.Stop();
    musicPlayer.resource = resource;
    musicPlayer.Play();
  }

  public void PlaySound(AudioResource resource) {
    AudioSource source = soundsIdle.Count > 0 ? soundsIdle.Dequeue() : Instantiate(soundPlayer, transform).GetComponent<AudioSource>();
    StartCoroutine(PlaySoundOn(resource, source));
  }

  #endregion

  #region Coroutines

  private IEnumerator PlaySoundOn(AudioResource resource, AudioSource source) {
    source.resource = resource;
    source.Play();
    while (source.isPlaying) yield return 0f;
    soundsIdle.Enqueue(source);
  } 

  #endregion

  #region Lifecycle

  private void Start() {
    music.Initialize();
    sound.Initialize();
  }

  #endregion
}
