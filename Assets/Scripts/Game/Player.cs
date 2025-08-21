using UnityEngine;

public class Player : MonoBehaviour {
  #region Constants

  #endregion

  #region Internal

  [System.Serializable]
  public class Music {
    public AudioSource Theme;
    public AudioSource Game;
  }


  [System.Serializable]
  public class Sounds {
  }

  #endregion

  #region Fields

  [Header("Audio")]
  [SerializeField] private Music music;
  [SerializeField] private Sounds sounds;

  private GameController gameController;

  #endregion

  #region Events

  #endregion

  #region Properties

  public static Player Instance => GameObject.FindWithTag("Player").GetComponent<Player>();

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

  private void Start() {
    if (gameController.IsReady) HandleGameReady();
    else gameController.OnReady.AddListener(HandleGameReady);
  }

  private void Awake() {
    gameController = GameController.Instance;
  }

  #endregion
}
