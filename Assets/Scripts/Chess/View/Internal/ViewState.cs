using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public sealed class ViewState {
  #region Fields

  private Move hint;

  #endregion

  #region Properties

  public readonly GameState GameState;

  public readonly IEnumerable<Move> CaptureMoves;

  public readonly IEnumerable<Coverage> Dangers;

  public Move Hint {
    get => hint;
    set { if (hint == value) hint = null; else hint = value; }
  }

  public bool HasHint => Hint != null;

  public BoardState BoardState => GameState.BoardState;

  public CoverageMap CoverageMap => BoardState.CoverageMap;

  public SideType SideToMove => GameState.BoardState.SideToMove;

  #endregion

  #region Constructor

  public ViewState(GameState gameState) {
    GameState = gameState;
    CaptureMoves = GameState.MovesFor(SideToMove).Where(m => m.IsCapture);
    Dangers = CoverageMap.Coverages.Where(c => c.Piece.SideType != SideToMove && c.Other != null && c.Other.SideType == SideToMove);
  }

  #endregion
}
