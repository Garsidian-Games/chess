using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;

public class SideStatus : MonoBehaviour {
  #region Constants

  #endregion

  #region Internal

  #endregion

  #region Fields

  [SerializeField] private SideType sideType;
  [SerializeField] private CapturedPieces capturedPieces;
  [SerializeField] private Image border;
  [SerializeField] private TextMeshProUGUI title;
  [SerializeField] private TextMeshProUGUI timer;
  [SerializeField] private TextMeshProUGUI check;

  private Color defaultBorderColor;

  #endregion

  #region Events

  #endregion

  #region Properties

  public Color BorderColor {
    get => border.color;
    set => border.color = value;
  }

  public bool InCheck {
    get => check.enabled;
    set => check.enabled = value;
  }

  public float ElapsedTime {
    set => timer.text = FormatTimer(value);
  }

  #endregion

  #region Methods

  public static string FormatTimer(float seconds) {
    int minutes = Mathf.FloorToInt(seconds / 60f);
    int secs = Mathf.FloorToInt(seconds % 60f);
    return string.Format("{0:00}:{1:00}", minutes, secs);
  }

  public void ResetBorderColor() {
    BorderColor = defaultBorderColor;
  }

  public void Display(Piece[] captured) => capturedPieces.Display(captured.Where(piece => piece.SideType != sideType).ToArray());

  #endregion

  #region Coroutines

  #endregion

  #region Handlers

  #endregion

  #region Lifecycle

  private void Start() {
    InCheck = false;
    ElapsedTime = 0f;
  }

  private void Awake() {
    defaultBorderColor = border.color;
  }

  #endregion
}
