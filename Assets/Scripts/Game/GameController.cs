using UnityEngine;

[RequireComponent(typeof(AudioManager))]
[RequireComponent(typeof(GameManager))]
public class GameController : MonoBehaviour {
  #region Constants

  #endregion

  #region Internal

  #endregion

  #region Fields

  #endregion

  #region Events

  #endregion

  #region Properties

  public static GameController Instance => GameObject.FindWithTag("GameController").GetComponent<GameController>();

  public AudioManager AudioManager { get; private set; }

  public GameManager GameManager { get; private set; }

  #endregion

  #region Methods

  #endregion

  #region Coroutines

  #endregion

  #region Handlers

  #endregion

  #region Lifecycle

  private void Awake() {
    AudioManager = GetComponent<AudioManager>();
    GameManager = GetComponent<GameManager>();
  }

  #endregion
}
