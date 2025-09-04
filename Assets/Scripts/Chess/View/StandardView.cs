using UnityEngine;
using System.Linq;

public sealed class StandardView : BaseView {
  #region Constants

  #endregion

  #region Methods

  protected override SquareState Create(Square square) => new(StandardOptionsFor(square));

  protected override SquareState CreateHintFrom(Square square) {
    var opts = StandardOptionsFor(square);
    opts.PieceBorderColor = player.BorderColorInspected;
    opts.PulsePiece = true;
    return new(opts);
  }

  protected override SquareState CreateHintTo(Square square) {
    var opts = StandardOptionsFor(square);
    opts.BorderColor = player.BorderColorInspected;
    opts.HighlightVisible = true;
    return new(opts);
  }

  #endregion

  #region Constructor

  public StandardView(ViewState viewState, Player player) : base(viewState, player) { }

  #endregion
}
