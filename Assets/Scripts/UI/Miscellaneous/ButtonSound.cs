using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonSound : MonoBehaviour {
  #region Fields

  [SerializeField] private AudioResource sound;

  private Button button;

  private GameController gameController;

  #endregion

  #region Handlers

  private void HandleButtonClick() {
    gameController.AudioManager.PlaySound(sound);
  }

  #endregion

  #region Lifecycle

  private void Start() {
    button.onClick.AddListener(HandleButtonClick);
  }

  private void Awake() {
    button = GetComponent<Button>();

    gameController = GameController.Instance;
  }

  #endregion
}
