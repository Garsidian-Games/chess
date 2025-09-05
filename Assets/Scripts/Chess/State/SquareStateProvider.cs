using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public sealed class SquareStateProvider {
  #region Constants

  #endregion

  #region Internal

  public enum ProviderMode {
    Standard,
    DragFrom,
    MovesFor,
    CoveragesFrom,
    CoveragesTo,
  }

  private class SquareViewCache {
    private readonly Dictionary<ViewState, Dictionary<Square, SquareView>> views = new();
    private readonly ProviderMode mode;

    private SquareView Create(Square square, ViewState viewState, Player player) {
      return mode switch {
        ProviderMode.DragFrom => new DragFromView(square, viewState, player),
        ProviderMode.MovesFor => new MovesFromView(square, viewState, player),
        ProviderMode.CoveragesFrom => new CoveragesFromView(square, viewState, player),
        ProviderMode.CoveragesTo => new CoveragesToView(square, viewState, player),
        _ => throw new System.NotImplementedException(),
      };
    }

    public SquareView Get(Square square, ViewState viewState, Player player) {
      if (!views.ContainsKey(viewState)) views[viewState] = new();
      if (!views[viewState].ContainsKey(square)) views[viewState][square] = Create(square, viewState, player);
      return views[viewState][square];
    }

    public SquareViewCache(ProviderMode mode) {
      this.mode = mode;
    }
  }

  #endregion

  #region Fields

  private readonly GameController gameController;

  private readonly Player player;

  private readonly Dictionary<GameState, ViewState> viewStates = new();

  private readonly Dictionary<ViewState, StandardView> standardViews = new();

  private readonly SquareViewCache coveragesFromViews = new(ProviderMode.CoveragesFrom);

  private readonly SquareViewCache coveragesToViews = new(ProviderMode.CoveragesTo);

  private readonly SquareViewCache movesFromViews = new(ProviderMode.MovesFor);

  private readonly SquareViewCache dragFromViews = new(ProviderMode.DragFrom);

  private ProviderMode mode;

  private ViewState viewState;

  private BaseView activeView;

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
    activeView = null;
    Apply();
  }

  public void Clear() {
    viewStates.Clear();
    standardViews.Clear();
  }

  public void Click(Square square, ProviderMode mode) {
    this.mode = mode;
    activeView = For(square, mode);
    Apply();
  }

  public void DragFrom(Square square) {
    mode = ProviderMode.DragFrom;
    activeView = For(square, mode);
    Apply();
  }

  public void DragEnter(Square square) {
    Assert.IsTrue(mode == ProviderMode.DragFrom);
    (activeView as DragFromView).Enter(square);
    Apply();
  }

  public void DragExit(Square square) {
    Assert.IsTrue(mode == ProviderMode.DragFrom);
    if ((activeView as DragFromView).Exit(square)) Apply();
  }

  private BaseView For(Square square, ProviderMode mode) {
    return mode switch {
      ProviderMode.DragFrom => dragFromViews.Get(square, ViewState, player),
      ProviderMode.MovesFor => movesFromViews.Get(square, ViewState, player),
      ProviderMode.CoveragesFrom => coveragesFromViews.Get(square, ViewState, player),
      ProviderMode.CoveragesTo => coveragesToViews.Get(square, ViewState, player),
      _ => throw new System.NotImplementedException(string.Format("{0} is not a valid click mode", mode)),
    };
  }

  private void Apply() {
    switch (Mode) {
      case ProviderMode.Standard:
        StandardView.Apply();
        break;
      case ProviderMode.DragFrom:
      case ProviderMode.MovesFor:
      case ProviderMode.CoveragesFrom:
      case ProviderMode.CoveragesTo:
        activeView.Apply();
        break;
      default:
        throw new System.NotImplementedException(string.Format("{0} is not a valid mode!", Mode));
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
