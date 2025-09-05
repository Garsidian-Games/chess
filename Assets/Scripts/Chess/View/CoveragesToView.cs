using UnityEngine;
using System.Linq;

public sealed class CoveragesToView : SquareView {
  #region Constants

  #endregion

  #region Properties

  public readonly Square To;

  private readonly Coverage[] coverages;

  private readonly Color borderColor;

  #endregion

  #region Methods

  protected override SquareState Create(Square square) {
    var opts = StandardOptionsFor(square, false);

    if (square == To) {
      opts.BorderColor = borderColor;
    } else if (coverages.Any(c => c.From == square)) {
      opts.PieceBorderColor = borderColor;
      opts.PulsePiece = true;
    }

    return new(opts);
  }

  protected override SquareState CreateHintFrom(Square square) => new(StandardOptionsFor(square, false));

  protected override SquareState CreateHintTo(Square square) => new(StandardOptionsFor(square, false));

  #endregion

  #region Constructor

  public CoveragesToView(Square to, ViewState viewState, Player player) : base(to, viewState, player) {
    To = to;
    coverages = GameState.BoardState.CoverageMap.To(to);
    borderColor = player.BorderColorInspected;
  }

  #endregion
}
