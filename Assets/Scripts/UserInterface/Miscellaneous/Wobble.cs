using UnityEngine;

[DisallowMultipleComponent]
public class Wobble : MonoBehaviour {
  #region Fields

  [Header("Settings")]
  [Tooltip("Peak rotation from the starting angle, in degrees.")]
  [SerializeField] [Range(0f, 45f)] private float amplitude = 10f;

  [Tooltip("Oscillations per second.")]
  [SerializeField] [Min(0f)] private float frequency = 1.0f;

  [Tooltip("Initial phase offset in degrees (useful for desyncing multiple wobblers).")]
  [SerializeField] [Range(0f, 360f)] private float phaseDegrees = 0f;

  [Tooltip("Use unscaled time (ignores Time.timeScale).")]
  [SerializeField] private bool useUnscaledTime;

  [Tooltip("Randomize the starting phase on enable.")]
  [SerializeField] private bool randomizePhaseOnEnable;

  [Tooltip("Reset the rotation to the original when disabled or destroyed.")]
  [SerializeField] private bool resetOnDisable = true;

  private Vector3 rotation;

  private float radians;

  #endregion

  #region Lifecycle

  private void Update() {
    if (amplitude <= 0f || frequency <= 0f) return;

    float t = useUnscaledTime ? Time.unscaledTime : Time.time;
    float angle = amplitude * Mathf.Sin((Mathf.PI * 2f * frequency * t) + radians);

    transform.localRotation = Quaternion.Euler(rotation.x, rotation.y, rotation.z + angle);
  }

  private void OnDisable() {
    if (resetOnDisable) transform.localRotation = Quaternion.Euler(rotation);
  }

  private void OnEnable() {
    if (randomizePhaseOnEnable) radians = Random.value * Mathf.PI * 2f;
  }

  private void Awake() {
    rotation = transform.localEulerAngles;
    radians = phaseDegrees * Mathf.Deg2Rad;
  }

  #endregion
}
