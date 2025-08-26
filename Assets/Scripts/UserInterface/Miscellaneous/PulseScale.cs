using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class PulseScale : MonoBehaviour {
  #region Fields

  [Header("Targets")]
  [Tooltip("UI Images to pulse. At least one is expected unless 'Use This Transform If Empty' is enabled.")]
  [SerializeField] private Image[] images;

  [Tooltip("If true and no Images are assigned, pulse this component's Transform instead.")]
  [SerializeField] private bool useThisTransformIfEmpty = false;

  [Header("Settings")]
  [Tooltip("Peak scale offset as a fraction of base scale (e.g., 0.1 = ±10%).")]
  [SerializeField] [Range(0f, 1f)] private float amplitude = 0.1f;

  [Tooltip("Oscillations per second.")]
  [SerializeField] [Min(0f)] private float frequency = 1.0f;

  [Tooltip("Initial phase offset in degrees (useful for desyncing multiple pulsers).")]
  [SerializeField] [Range(0f, 360f)] private float phaseDegrees = 0f;

  [Tooltip("Use unscaled time (ignores Time.timeScale).")]
  [SerializeField] private bool useUnscaledTime = false;

  [Tooltip("Randomize the starting phase on enable.")]
  [SerializeField] private bool randomizePhaseOnEnable = false;

  [Tooltip("Reset the scale(s) to the original when disabled or destroyed.")]
  [SerializeField] private bool resetOnDisable = true;

  private struct Target {
    public Transform Transform;
    public Vector3 BaseScale;
  }

  private readonly List<Target> _targets = new();
  private float _phaseRadians;

  #endregion

  #region Lifecycle

  private void Awake() {
    _phaseRadians = phaseDegrees * Mathf.Deg2Rad;
    BuildTargets();
  }

  private void OnEnable() {
    if (randomizePhaseOnEnable)
      _phaseRadians = Random.value * Mathf.PI * 2f;

    // Rebuild in case the Images were assigned/changed while disabled
    BuildTargets();
  }

  private void OnDisable() {
    if (!resetOnDisable) return;

    for (int i = 0; i < _targets.Count; i++)
      _targets[i].Transform.localScale = _targets[i].BaseScale;
  }

  private void Update() {
    if (amplitude <= 0f || frequency <= 0f || _targets.Count == 0) return;

    float t = useUnscaledTime ? Time.unscaledTime : Time.time;
    // s(t) = 1 + A * sin(2π f t + φ)
    float scaleFactor = 1f + amplitude * Mathf.Sin((Mathf.PI * 2f * frequency * t) + _phaseRadians);

    for (int i = 0; i < _targets.Count; i++) {
      var tgt = _targets[i];
      tgt.Transform.localScale = tgt.BaseScale * scaleFactor; // uniform pulse
    }
  }

  private void OnValidate() {
    if (!Application.isPlaying)
      _phaseRadians = phaseDegrees * Mathf.Deg2Rad;

    // Friendly reminder if nothing assigned and no fallback
    if ((images == null || images.Length == 0) && !useThisTransformIfEmpty) {
#if UNITY_EDITOR
      UnityEditor.EditorApplication.delayCall += () =>
      {
        if (this == null) return;
        Debug.LogWarning($"[PulseScale] No Images assigned on '{name}' and 'Use This Transform If Empty' is disabled. Nothing will pulse.", this);
      };
#endif
    }
  }

  #endregion

  #region Helpers

  private void BuildTargets() {
    _targets.Clear();

    if (images != null) {
      for (int i = 0; i < images.Length; i++) {
        var img = images[i];
        if (img == null) continue;

        var tr = img.transform;
        _targets.Add(new Target { Transform = tr, BaseScale = tr.localScale });
      }
    }

    if (_targets.Count == 0 && useThisTransformIfEmpty) {
      _targets.Add(new Target { Transform = transform, BaseScale = transform.localScale });
    }
  }

  #endregion
}
