using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class TextBounce : MonoBehaviour {
  [Header("Bounce Settings")]
  [Tooltip("How high the text bounces in units.")]
  public float bounceHeight = 10f;
  [Tooltip("How quickly it bounces up and down.")]
  public float bounceDuration = 0.2f;
  [Tooltip("Pause time between bounces.")]
  public float bounceDelay = 0.5f;

  private RectTransform rectTransform;
  private Vector3 basePosition;

  void Awake() {
    rectTransform = GetComponent<RectTransform>();
    basePosition = rectTransform.anchoredPosition;
  }

  void OnEnable() {
    StopAllCoroutines();
    StartCoroutine(BounceRoutine());
  }

  System.Collections.IEnumerator BounceRoutine() {
    while (true) {
      // Up
      float t = 0f;
      while (t < bounceDuration) {
        t += Time.unscaledDeltaTime;
        float normalized = t / bounceDuration;
        float y = Mathf.Sin(normalized * Mathf.PI * 0.5f) * bounceHeight;
        rectTransform.anchoredPosition = basePosition + new Vector3(0, y, 0);
        yield return null;
      }

      // Down
      t = 0f;
      while (t < bounceDuration) {
        t += Time.unscaledDeltaTime;
        float normalized = t / bounceDuration;
        float y = (1f - Mathf.Sin(normalized * Mathf.PI * 0.5f)) * bounceHeight;
        rectTransform.anchoredPosition = basePosition + new Vector3(0, y, 0);
        yield return null;
      }

      // Reset + delay
      rectTransform.anchoredPosition = basePosition;
      yield return new WaitForSecondsRealtime(bounceDelay);
    }
  }
}
