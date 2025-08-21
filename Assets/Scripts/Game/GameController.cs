using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(AudioManager))]
public class GameController : MonoBehaviour {
  #region Constants

  #endregion

  #region Internal

  #endregion

  #region Fields

  private UIController uiController;

  #endregion

  #region Events

  [HideInInspector] public UnityEvent OnReady;

  #endregion

  #region Properties

  public static GameController Instance => GameObject.FindWithTag("GameController").GetComponent<GameController>();

  public bool IsReady { get; private set; }

  public AudioManager AudioManager { get; private set; }

  #endregion

  #region Methods

  private void ReadyUp() {
    if (IsReady) return;
    if (!uiController.IsReady) return;

    IsReady = true;
    OnReady.Invoke();
  }

  #endregion

  #region Coroutines

  #endregion

  #region Handlers

  private void HandleUIReady() => ReadyUp();

  #endregion

  #region Lifecycle

  private void Start() {
    if (uiController.IsReady) HandleUIReady();
    else uiController.OnReady.AddListener(HandleUIReady);
  }

  private void Awake() {
    AudioManager = GetComponent<AudioManager>();

    uiController = UIController.Instance;
  }

  #endregion
}
