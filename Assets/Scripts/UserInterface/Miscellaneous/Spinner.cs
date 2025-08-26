using UnityEngine;

[DisallowMultipleComponent]
public class Spinner : MonoBehaviour {
  #region Fields

  [Header("Target")]
  [Tooltip("Optional. If left empty, this component will spin its own RectTransform.")]
  [SerializeField] private RectTransform target;

  [Header("Settings")]
  [Tooltip("Rotation speed in degrees per second.")]
  [SerializeField] [Min(0f)] private float speed = 90f;

  [Tooltip("Clockwise rotation (true) or counter-clockwise (false).")]
  [SerializeField] private bool clockwise = true;

  [Tooltip("Initial offset angle applied on enable, in degrees.")]
  [SerializeField] [Range(0f, 360f)] private float startAngleDegrees = 0f;

  [Tooltip("Randomize the starting angle on enable.")]
  [SerializeField] private bool randomizeStartOnEnable = false;

  [Tooltip("Use unscaled time (ignores Time.timeScale).")]
  [SerializeField] private bool useUnscaledTime = false;

  [Tooltip("Reset rotation to original when disabled or destroyed.")]
  [SerializeField] private bool resetOnDisable = true;

  private RectTransform _rt;
  private Vector3 _baseEuler;
  private float _startAngleDeg;
  private float _startTime;

  #endregion

  #region Lifecycle

  private void Awake() {
    _rt = target != null ? target : GetComponent<RectTransform>();
    if (_rt == null) {
      Debug.LogWarning("[Spinner] No RectTransform found. Disabling.", this);
      enabled = false;
      return;
    }

    _baseEuler = _rt.localEulerAngles;
    _startAngleDeg = startAngleDegrees;
    _startTime = CurrentTime();
  }

  private void OnEnable() {
    if (_rt == null) return;

    if (randomizeStartOnEnable)
      _startAngleDeg = Random.value * 360f;
    else
      _startAngleDeg = startAngleDegrees;

    // Re-snapshot base in case something changed while disabled
    _baseEuler = _rt.localEulerAngles;
    _startTime = CurrentTime();
  }

  private void OnDisable() {
    if (_rt == null) return;
    if (resetOnDisable)
      _rt.localRotation = Quaternion.Euler(_baseEuler);
  }

  private void Update() {
    if (_rt == null || speed <= 0f) return;

    float elapsed = CurrentTime() - _startTime;
    float dir = clockwise ? -1f : 1f;              // UI Z+ is counter-clockwise; flip for "clockwise"
    float delta = dir * speed * elapsed + _startAngleDeg;
    float z = _baseEuler.z + delta;

    // Keep numbers reasonable (optional)
    if (z > 100000f || z < -100000f)
      z = Mathf.Repeat(z, 360f);

    _rt.localRotation = Quaternion.Euler(_baseEuler.x, _baseEuler.y, z);
  }

  private void OnValidate() {
    if (speed < 0f) speed = 0f;
  }

  #endregion

  #region Helpers

  private float CurrentTime() => useUnscaledTime ? Time.unscaledTime : Time.time;

  #endregion
}
