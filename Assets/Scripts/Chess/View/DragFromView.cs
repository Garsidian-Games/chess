using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public sealed class DragFromView : MovesView {
  #region Constants

  #endregion

  #region Fields

  private readonly Dictionary<Square, SquareState> hover = new();

  #endregion

  #region Properties

  public Square Hover { get; private set; }

  #endregion

  #region Methods

  protected override SquareState For(Square square) {
    if (Hover == square) {
      var h = HoverFor(square);
      if (h != null) return h;
    }

    return base.For(square);
  }

  public void Enter(Square square) {
    Hover = square;
  }

  public bool Exit(Square square) {
    if (Hover != square) return false;
    Hover = null;
    return true;
  }

  protected override SquareState CreateFrom(Square square) => WithHiddenPiece(square);

  protected override SquareState CreateHintFrom(Square square) => WithHiddenPiece(square);

  private SquareState WithHiddenPiece(Square square) {
    var opts = StandardOptionsFor(square, false);
    opts.PieceVisible = false;
    return new(opts);
  }

  private SquareState HoverFor(Square square) {
    if (!hover.ContainsKey(square)) hover[square] = CreateHoverFor(square);
    return hover[square];
  }

  private SquareState CreateHoverFor(Square square) {
    var opts = StandardOptionsFor(square, false);
    if (square == From) {
      opts.PulsePiece = true;
      opts.PieceVisible = false;
      opts.PieceBorderColor = player.BorderColorPlayer;
    } else {
      var move = moves.FirstOrDefault(move => move.To == square);
      if (move != null) {
        bool isHint = ApplicableHint && viewState.Hint.To == square;

        opts.TextVisible = !move.IsCapture;
        opts.CenterTextOpacity = 1f;
        opts.BorderColor = isHint ? player.BorderColorInspected : player.BorderColorPlayer;
        opts.PieceBorderColor = player.BorderColorOpponent;
        opts.PulsePiece = true;
      } else return null;
    }

    return new(opts);
  }

  #endregion

  #region Constructor

  public DragFromView(Square from, ViewState viewState, Player player) : base(from, viewState, player) { }

  #endregion
}
