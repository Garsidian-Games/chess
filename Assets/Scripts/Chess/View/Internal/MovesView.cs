using UnityEngine;
using System.Linq;

public abstract class MovesView : PieceView {
  #region Constants

  #endregion

  #region Properties

  protected readonly Move[] moves;

  protected virtual float CenterTextOpacity => 0.5f;

  #endregion

  #region Methods

  protected abstract SquareState CreateFrom(Square square);

  protected override SquareState Create(Square square) {
    if (square == From) return CreateFrom(square);
    return new(MoveOptionsFor(square));
  }

  protected SquareState.SquareStateOptions MoveOptionsFor(Square square) {
    var opts = StandardOptionsFor(square, false);
    var move = moves.FirstOrDefault(m => m.To == square);
    if (move != null) {
      opts.TextVisible = !move.IsCapture;
      opts.CenterTextOpacity = CenterTextOpacity;
      opts.PieceBorderColor = player.BorderColorOpponent;
      opts.TremblePiece = true;
    } else opts.ScreenVisible = true;
    return opts;
  }

  protected override SquareState CreateHintTo(Square square) => new(StandardOptionsForHintTo(square));

  #endregion

  #region Constructor

  public MovesView(Square from, ViewState viewState, Player player) : base(from, viewState, player) {
    moves = GameState.MovesFor(Piece, From);
  }

  #endregion
}
