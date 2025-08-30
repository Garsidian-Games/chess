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
  [SerializeField] private TextMeshProUGUI yourTurn;
  [SerializeField] private TextMeshProUGUI pleaseWait;
  [SerializeField] private TextMeshProUGUI thinking;
  [SerializeField] private TextMeshProUGUI check;

  private Color defaultBorderColor;

  private bool useMoveTimer;
  private bool forPlayer;
  private bool turnToMove;

  #endregion

  #region Events

  #endregion

  #region Properties

  public string Title {
    get => title.text;
    set => title.text = value;
  }

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

  public bool UseMoveTimer {
    get => useMoveTimer;
    set {
      useMoveTimer = value;
      Sync();
    }
  }

  public bool ForPlayer {
    get => forPlayer;
    set {
      forPlayer = value;
      Sync();
    }
  }

  public bool TurnToMove {
    get => turnToMove;
    set {
      turnToMove = value;
      Sync();
    }
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

  private void Sync() {
    timer.gameObject.SetActive(UseMoveTimer);
    yourTurn.gameObject.SetActive(!UseMoveTimer && ForPlayer && TurnToMove);
    pleaseWait.gameObject.SetActive(!UseMoveTimer && ForPlayer && !TurnToMove);
    thinking.gameObject.SetActive(!UseMoveTimer && !ForPlayer && TurnToMove);
  }

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
