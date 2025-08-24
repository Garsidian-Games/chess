using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;

public class Player : MonoBehaviour {
  #region Constants

  #endregion

  #region Internal

  [System.Serializable] public class SideEvent : UnityEvent<SideType> { }

  [System.Serializable]
  public class Music {
    public AudioResource Theme;
    public AudioResource Game;
  }

  #endregion

  #region Fields

  [Header("Audio")]
  [SerializeField] private Music music;

  [Header("Settings")]
  [SerializeField] private string prefPlayAsBlack = "PlayAsBlack";

  private SideType side;

  private GameController gameController;
  private UIController uiController;

  #endregion

  #region Events

  [HideInInspector] public SideEvent OnSideChanged;

  #endregion

  #region Properties

  public static Player Instance => GameObject.FindWithTag("Player").GetComponent<Player>();

  public SideType Side {
    get => side;
    set {
      if (side == value) return;

      side = value;
      if (side == SideType.Black) PlayerPrefs.SetInt(prefPlayAsBlack, 1);
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
    gameController.AudioManager.PlayMusic(music.Theme);
  }

  private void HandleSquareClicked(Square square) {
    Debug.LogFormat("Clicked {0}", square);
  }

  private void HandleSquareDragBegan(Square square) {
    Debug.LogFormat("Drag Began {0}", square);
  }

  private void HandleSquareDragEnded(Square square) {
    Debug.LogFormat("Drag Ended {0}", square);
  }

  private void HandleSquareDropped(Square square) {
    Debug.LogFormat("Dropped {0}", square);
  }

  private void HandleSquareEntered(Square square) {
    Debug.LogFormat("Entered {0}", square);
  }

  private void HandleSquareExited(Square square) {
    Debug.LogFormat("Exited {0}", square);
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

    uiController.Board.OnSquareClicked.AddListener(HandleSquareClicked);
    uiController.Board.OnSquareDragBegan.AddListener(HandleSquareDragBegan);
    uiController.Board.OnSquareDragEnded.AddListener(HandleSquareDragEnded);
    uiController.Board.OnSquareDropped.AddListener(HandleSquareDropped);
    uiController.Board.OnSquareEntered.AddListener(HandleSquareEntered);
    uiController.Board.OnSquareExited.AddListener(HandleSquareExited);
  }

  private void Awake() {
    gameController = GameController.Instance;
    uiController = UIController.Instance;

    Side = PlayerPrefs.HasKey(prefPlayAsBlack) ? SideType.Black : SideType.White;
  }

  #endregion
}
