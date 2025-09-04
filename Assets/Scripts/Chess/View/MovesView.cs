using UnityEngine;
using System.Linq;

public sealed class MovesView : BaseView {
  #region Constants

  #endregion

  #region Properties

  public readonly Square From;

  public readonly Piece Piece;

  private readonly Move[] moves;

  #endregion

  #region Methods

  protected override SquareState Create(Square square) {
    var opts = StandardOptionsFor(square, false);

    if (square == From) {
      opts.PieceBorderColor = player.BorderColorPlayer;
      opts.WobblePiece = true;
    } else if (moves.Any(m => m.To == square)) {
      opts.PieceBorderColor = player.BorderColorOpponent;
      opts.TargetColor = player.BorderColorPlayer;
      opts.TremblePiece = true;
    }
    
    return new(opts);
  }

  protected override SquareState CreateHintFrom(Square square) => new(StandardOptionsFor(square, false));

  protected override SquareState CreateHintTo(Square square) {
    var opts = StandardOptionsFor(square, false);
    opts.HighlightVisible = true;
    opts.BorderColor = player.BorderColorInspected;
    return new(opts);
  }

  #endregion

  #region Constructor

  public MovesView(Square from, ViewState viewState, Player player) : base(viewState, player) {
    From = from;
    Piece = GameState.BoardState[From];
    moves = GameState.MovesFor(Piece, From);
  }

  #endregion
}
