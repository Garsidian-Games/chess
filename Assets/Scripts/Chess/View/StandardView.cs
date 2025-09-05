using UnityEngine;
using System.Linq;

public sealed class StandardView : BaseView {
  #region Constants

  #endregion

  #region Methods

  protected override SquareState Create(Square square) {
    var opts = StandardOptionsFor(square);

    if (!GameState.IsRoot) {
      if (GameState.BoardState.Move.From == square) {
        opts.PieceBorder = GameState.BoardState.Move.Piece;
        opts.PieceBorderColor = player.BorderColorLastMoved;
      }

      if (GameState.BoardState.Move.To == square) {
        opts.PieceBorderColor = player.BorderColorLastMoved;
      }
    }

    return new(opts);
  }

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
