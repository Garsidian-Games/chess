using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class SliderSound : MonoBehaviour {
  #region Fields

  [SerializeField] private AudioResource sound;

  private Slider slider;

  private GameController gameController;

  #endregion

  #region Handlers

  private void HandleValueChanged(float _) {
    if (!enabled) return;
    gameController.AudioManager.PlaySound(sound, true);
  }

  #endregion

  #region Lifecycle

  private void Start() {
    slider.onValueChanged.AddListener(HandleValueChanged);
  }

  private void Awake() {
    slider = GetComponent<Slider>();

    gameController = GameController.Instance;
  }

  #endregion
}
