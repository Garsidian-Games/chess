using UnityEngine;
using System.Linq;

public sealed class CoveragesView : BaseView {
  #region Constants

  #endregion

  #region Properties

  public readonly Square From;

  public readonly Piece Piece;

  private readonly Coverage[] coverages;

  private readonly Color borderColor;

  #endregion

  #region Methods

  protected override SquareState Create(Square square) {
    var opts = StandardOptionsFor(square, false);

    if (square == From) {
      opts.PieceBorderColor = borderColor;
      opts.PulsePiece = true;
    } else if (coverages.Any(c => c.To == square)) {
      opts.HighlightVisible = true;
    }
    
    return new(opts);
  }

  protected override SquareState CreateHintFrom(Square square) => new(StandardOptionsFor(square, false));

  protected override SquareState CreateHintTo(Square square) => new(StandardOptionsFor(square, false));

  #endregion

  #region Constructor

  public CoveragesView(Square from, ViewState viewState, Player player) : base(viewState, player) {
    From = from;
    Piece = GameState.BoardState[From];
    coverages = GameState.BoardState.CoverageMap[Piece, From];
    borderColor = player.BorderColorFor(Piece.SideType);
  }

  #endregion
}
