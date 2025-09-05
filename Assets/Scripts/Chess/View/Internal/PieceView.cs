using UnityEngine;

public abstract class PieceView : SquareView {
  #region Constants

  #endregion

  #region Internal

  #endregion

  #region Fields

  #endregion

  #region Properties

  public readonly Square From;

  public readonly Piece Piece;

  protected override bool ApplicableHint {
    get {
      if (!base.ApplicableHint) return false;
      return viewState.Hint.From == From;
    }
  }

  #endregion

  #region Methods

  #endregion

  #region Coroutines

  #endregion

  #region Handlers

  #endregion

  #region Constructor

  public PieceView(Square from, ViewState viewState, Player player) : base(from, viewState, player) {
    From = from;
    Piece = GameState.BoardState[From];
  }

  #endregion
}
