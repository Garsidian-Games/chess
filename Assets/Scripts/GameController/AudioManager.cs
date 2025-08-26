using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour {
  #region Constants

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

  private readonly List<AudioResource> playing = new();
  private readonly Queue<AudioSource> idle = new();

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
    if (musicPlayer.resource == resource) return;

    musicPlayer.Stop();
    musicPlayer.resource = resource;
    musicPlayer.Play();
  }

  public void PlaySound(AudioResource resource, bool unique = false) {
    if (unique && playing.Contains(resource)) return;
    //Debug.Log(resource);
    AudioSource source = idle.Count > 0 ? idle.Dequeue() : Instantiate(soundPlayer, transform).GetComponent<AudioSource>();
    StartCoroutine(PlaySoundOn(resource, source, unique));
  }

  #endregion

  #region Coroutines

  private IEnumerator PlaySoundOn(AudioResource resource, AudioSource source, bool unique) {
    if (unique) playing.Add(resource);
    source.resource = resource;
    source.Play();
    while (source.isPlaying) yield return 0f;
    idle.Enqueue(source);
    if (unique) playing.Remove(resource);
  } 

  #endregion

  #region Lifecycle

  private void Start() {
    music.Initialize();
    sound.Initialize();
  }

  #endregion
}
