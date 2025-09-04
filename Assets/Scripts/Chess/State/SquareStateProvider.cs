using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public sealed class SquareStateProvider {
  #region Constants

  #endregion

  #region Internal

  public enum ProviderMode {
    Standard,
    Moves,
    Coverages,
  }

  #endregion

  #region Fields

  private readonly GameController gameController;

  private readonly Player player;

  private readonly Dictionary<GameState, ViewState> viewStates = new();

  private readonly Dictionary<ViewState, StandardView> standardViews = new();

  private readonly Dictionary<ViewState, Dictionary<Square, CoveragesView>> coveragesViews = new();

  private ProviderMode mode;

  private ViewState viewState;

  private BaseView clickedView;

  #endregion

  #region Events

  #endregion

  #region Properties

  public bool HasHint => viewState.HasHint;

  public ProviderMode Mode {
    get => mode;
    set {
      mode = value;
      Apply();
    }
  }

  public Move Hint {
    get => ViewState.Hint;
    set {
      ViewState.Hint = value;
      Apply();
    }
  }

  public GameState GameState => gameController.GameManager.GameState;

  private ViewState ViewState {
    get {
      if (!viewStates.ContainsKey(GameState)) viewStates[GameState] = new(GameState);
      return viewStates[GameState];
    }
  }

  private StandardView StandardView {
    get {
      if (!standardViews.ContainsKey(ViewState)) standardViews[ViewState] = new(ViewState, player);
      return standardViews[ViewState];
    }
  }

  #endregion

  #region Methods

  public void Reset() {
    mode = ProviderMode.Standard;
    clickedView = null;
    Apply();
  }

  public void Clear() {
    viewStates.Clear();
    standardViews.Clear();
  }

  public void Click(Square square, ProviderMode mode) {
    this.mode = mode;
    clickedView = For(square, mode);
    Apply();
  }

  private BaseView For(Square square, ProviderMode mode) {
    return mode switch {
      ProviderMode.Coverages => new CoveragesView(square, ViewState, player),
      ProviderMode.Moves => new MovesView(square, ViewState, player),
      _ => throw new System.NotImplementedException(string.Format("{0} is not a valid click mode", mode)),
    };
  }

  private void Apply() {
    switch (Mode) {
      case ProviderMode.Standard:
        StandardView.Apply();
        break;
      case ProviderMode.Moves:
      case ProviderMode.Coverages:
        clickedView.Apply();
        break;
    }
  }

  #endregion

  #region Coroutines

  #endregion

  #region Handlers

  #endregion

  #region Lifecycle

  public SquareStateProvider(GameController gameController, Player player) {
    this.gameController = gameController;
    this.player = player;
  }

  #endregion
}
