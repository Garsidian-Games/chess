using UnityEngine;
using System.Linq;

public sealed class MovesView : BaseMoveView {
  #region Constants

  #endregion

  #region Properties

  #endregion

  #region Methods

  protected override SquareState CreateFrom(Square square) {
    var opts = StandardOptionsFor(square, false);
    opts.PieceBorderColor = player.BorderColorPlayer;
    opts.WobblePiece = true;
    return new(opts);
  }

  protected override SquareState CreateHintFrom(Square square) => CreateFrom(square);

  #endregion

  #region Constructor

  public MovesView(Square from, ViewState viewState, Player player) : base(from, viewState, player) { }

  #endregion
}
