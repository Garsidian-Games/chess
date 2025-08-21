using UnityEngine;

public class Player : MonoBehaviour {
  #region Constants

  #endregion

  #region Internal

  [System.Serializable]
  public class Music {
    public AudioSource theme;
    public AudioSource play;
  }


  [System.Serializable]
  public class Sounds {
  }

  #endregion

  #region Fields

  [Header("Audio")]
  [SerializeField] private Music music;
  [SerializeField] private Sounds sounds;

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

  #endregion

  #region Lifecycle

  #endregion
}
