using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(AudioManager))]
[RequireComponent(typeof(GameManager))]
[RequireComponent(typeof(StockfishMananger))]
public class GameController : MonoBehaviour {
  #region Constants

  #endregion

  #region Internal

  #endregion

  #region Fields

  #endregion

  #region Events

  [HideInInspector] public UnityEvent OnReady;

  #endregion

  #region Properties

  public static GameController Instance => GameObject.FindWithTag("GameController").GetComponent<GameController>();

  public bool IsReady { get; private set; }

  public AudioManager AudioManager { get; private set; }

  public GameManager GameManager { get; private set; }

  public StockfishMananger StockfishMananger { get; private set; }

  #endregion

  #region Methods

  private void ReadyUp() {
    if (IsReady) return;
    if (!GameManager.IsReady) return;
    if (!StockfishMananger.IsReady) return;

    IsReady = true;
    OnReady.Invoke();
  }

  #endregion

  #region Coroutines

  #endregion

  #region Handlers

  #endregion

  #region Lifecycle

  private void Start() {
    if (GameManager.IsReady) ReadyUp();
    else GameManager.OnReady.AddListener(ReadyUp);

    if (StockfishMananger.IsReady) ReadyUp();
    else StockfishMananger.OnReady.AddListener(ReadyUp);
  }

  private void Awake() {
    AudioManager = GetComponent<AudioManager>();
    GameManager = GetComponent<GameManager>();
    StockfishMananger = GetComponent<StockfishMananger>();
  }

  #endregion
}
