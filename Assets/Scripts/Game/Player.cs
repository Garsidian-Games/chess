using UnityEngine;
using UnityEngine.Events;

public class Player : MonoBehaviour {
  #region Constants

  #endregion

  #region Internal

  [System.Serializable] public class SideEvent : UnityEvent<Side> { }

  [System.Serializable]
  public class Music {
    public AudioSource Theme;
    public AudioSource Game;
  }

  #endregion

  #region Fields

  [Header("Audio")]
  [SerializeField] private Music music;

  [Header("Settings")]
  [SerializeField] private string prefPlayAsBlack = "PlayAsBlack";

  private Side side;

  private GameController gameController;

  #endregion

  #region Events

  [HideInInspector] public SideEvent OnSideChanged;

  #endregion

  #region Properties

  public static Player Instance => GameObject.FindWithTag("Player").GetComponent<Player>();

  public Side Side {
    get => side;
    set {
      if (side == value) return;

      side = value;
      if (side == Side.Black) PlayerPrefs.SetInt(prefPlayAsBlack, 1);
      else PlayerPrefs.DeleteKey(prefPlayAsBlack);

      OnSideChanged.Invoke(side);
    }
  }

  #endregion

  #region Methods

  #endregion

  #region Coroutines

  #endregion

  #region Handlers

  private void HandleGameReady() {
    music.Theme.Play();
  }

  #endregion

  #region Lifecycle

  private void Update() {
#if UNITY_EDITOR
    if (Input.GetKeyDown(KeyCode.P)) {
      string filename = string.Format("{0}/screenshots/chess_{1}.png", Application.persistentDataPath, System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
      ScreenCapture.CaptureScreenshot(filename);
      Debug.LogFormat("Captured screenshot {0}", filename);
    }
#endif
  }

  private void Start() {
    if (gameController.IsReady) HandleGameReady();
    else gameController.OnReady.AddListener(HandleGameReady);
  }

  private void Awake() {
    gameController = GameController.Instance;

    Side = PlayerPrefs.HasKey(prefPlayAsBlack) ? Side.Black : Side.White;
  }

  #endregion
}
