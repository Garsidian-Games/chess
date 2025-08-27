using UnityEngine;

[DisallowMultipleComponent]
public class Tremble : MonoBehaviour {
  #region Fields

  [Header("Settings")]
  [Tooltip("Peak offset from the starting position, in units (UI = pixels).")]
  [SerializeField] [Min(0f)] private float amplitude = 10f;

  [Tooltip("Oscillations per second.")]
  [SerializeField] [Min(0f)] private float frequency = 10f;

  [Tooltip("Initial phase offset in degrees (useful for desyncing multiple trembles).")]
  [SerializeField] [Range(0f, 360f)] private float phaseDegrees = 0f;

  [Tooltip("Add a bit of irregular noise on top of the sine. 0 = pure sine.")]
  [SerializeField] [Range(0f, 1f)] private float jitter = 0.15f;

  [Tooltip("How fast the jitter wiggles.")]
  [SerializeField] [Min(0f)] private float jitterFrequency = 25f;

  [Tooltip("Direction of the shake in local XY (UI: anchored pos). (1,0) = left–right.")]
  [SerializeField] private Vector2 direction = Vector2.right;

  [Tooltip("Use unscaled time (ignores Time.timeScale).")]
  [SerializeField] private bool useUnscaledTime = false;

  [Tooltip("Randomize the starting phase on enable.")]
  [SerializeField] private bool randomizePhaseOnEnable = false;

  [Tooltip("Reset the position to the original when disabled or destroyed.")]
  [SerializeField] private bool resetOnDisable = true;

  private RectTransform _rect;       // if UI
  private bool _useRect;
  private Vector3 _baseLocalPos;     // for Transform
  private Vector2 _baseAnchoredPos;  // for RectTransform
  private float _phaseRad;
  private float _jitterSeed;

  #endregion

  #region Lifecycle

  private void Awake() {
    _rect = GetComponent<RectTransform>();
    _useRect = _rect != null;

    if (_useRect) _baseAnchoredPos = _rect.anchoredPosition;
    else _baseLocalPos = transform.localPosition;

    _phaseRad = phaseDegrees * Mathf.Deg2Rad;
    _jitterSeed = Random.value * 1000f;

    NormalizeDirection();
  }

  private void OnEnable() {
    if (randomizePhaseOnEnable)
      _phaseRad = Random.value * Mathf.PI * 2f;

    // Re-snapshot base in case parent/layout changed while disabled
    if (_useRect) _baseAnchoredPos = _rect.anchoredPosition;
    else _baseLocalPos = transform.localPosition;
  }

  private void OnDisable() {
    if (!resetOnDisable) return;

    if (_useRect) _rect.anchoredPosition = _baseAnchoredPos;
    else transform.localPosition = _baseLocalPos;
  }

  private void Update() {
    if (amplitude <= 0f || frequency <= 0f) return;

    float t = useUnscaledTime ? Time.unscaledTime : Time.time;

    // Base sine wobble
    float offset = amplitude * Mathf.Sin((Mathf.PI * 2f * frequency * t) + _phaseRad);

    // Optional irregular jitter (-1..1)
    if (jitter > 0f && jitterFrequency > 0f) {
      float n = Mathf.PerlinNoise(_jitterSeed, t * jitterFrequency) * 2f - 1f;
      offset += amplitude * jitter * n;
    }

    Vector2 dir2 = direction.sqrMagnitude > 0.0001f ? direction.normalized : Vector2.right;

    if (_useRect) {
      _rect.anchoredPosition = _baseAnchoredPos + dir2 * offset;
    } else {
      Vector3 dir3 = new Vector3(dir2.x, dir2.y, 0f);
      transform.localPosition = _baseLocalPos + dir3 * offset;
    }
  }

  private void OnValidate() {
    if (!Application.isPlaying)
      _phaseRad = phaseDegrees * Mathf.Deg2Rad;

    if (frequency < 0f) frequency = 0f;
    if (amplitude < 0f) amplitude = 0f;
    if (jitterFrequency < 0f) jitterFrequency = 0f;

    NormalizeDirection();
  }

  #endregion

  #region Helpers

  private void NormalizeDirection() {
    if (direction.sqrMagnitude < 0.0001f) direction = Vector2.right;
  }

  #endregion
}
