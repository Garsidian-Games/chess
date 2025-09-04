using UnityEngine;
using System.Linq;

public abstract class BaseMoveView : PieceView {
  #region Constants

  #endregion

  #region Properties

  protected readonly Move[] moves;

  #endregion

  #region Methods

  protected abstract SquareState CreateFrom(Square square);

  protected override SquareState Create(Square square) {
    if (square == From) return CreateFrom(square);
    return new(MoveOptionsFor(square));
  }

  protected SquareState.SquareStateOptions MoveOptionsFor(Square square) {
    var opts = StandardOptionsFor(square, false);
    if (moves.Any(m => m.To == square)) {
      opts.PieceBorderColor = player.BorderColorOpponent;
      opts.TremblePiece = true;
    } else opts.ScreenVisible = true;
    return opts;
  }

  protected override SquareState CreateHintTo(Square square) => new(StandardOptionsForHintTo(square));

  #endregion

  #region Constructor

  public BaseMoveView(Square from, ViewState viewState, Player player) : base(from, viewState, player) {
    moves = GameState.MovesFor(Piece, From);
  }

  #endregion
}
