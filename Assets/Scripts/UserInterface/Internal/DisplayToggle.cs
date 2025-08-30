using UnityEngine;
using UnityEngine.Events;

public abstract class DisplayToggle : MonoBehaviour {
  #region Constants

  #endregion

  #region Internal

  #endregion

  #region Fields

  #endregion

  #region Events

  [HideInInspector] public UnityEvent OnShown;
  [HideInInspector] public UnityEvent OnHidden;


  #endregion

  #region Properties

  public bool IsVisible {
    get => gameObject.activeSelf;
    set => Display(value);
  }

  #endregion

  #region Methods

  public virtual void Toggle() => Display(!IsVisible);

  public virtual void Show() => ChangeVisibility(true);

  public virtual void Hide() => ChangeVisibility(false);

  public virtual void Display(bool value) => ChangeVisibility(value);

  protected virtual void ChangeVisibility(bool isVisible) {
    if (IsVisible == isVisible) return;
    gameObject.SetActive(isVisible);
    if (IsVisible) OnShow(); else OnHide();
  }

  protected virtual void OnShow() => OnShown.Invoke();

  protected virtual void OnHide() => OnHidden.Invoke();

  #endregion

  #region Coroutines

  #endregion

  #region Handlers

  #endregion

  #region Lifecycle

  #endregion
}
