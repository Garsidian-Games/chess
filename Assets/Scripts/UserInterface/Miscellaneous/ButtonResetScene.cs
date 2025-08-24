using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonResetScene : MonoBehaviour {
  #region Fields

  private Button button;

  #endregion

  #region Handlers

  private void HandleButtonClick() {
    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
  }

  #endregion

  #region Lifecycle

  private void Start() {
    button.onClick.AddListener(HandleButtonClick);
  }

  private void Awake() {
    button = GetComponent<Button>();
  }

  #endregion
}
