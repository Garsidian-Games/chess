using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public sealed class SquareState {
  #region Constants

  public static float[] CoverageOpacity = new float[] { 0f, 0.2f, 0.4f, 0.6f };

  #endregion

  #region Internal

  #endregion

  #region Fields

  #endregion

  #region Events

  #endregion

  #region Properties

  public readonly Square Square;

  public readonly bool HighlightVisible;

  public readonly Color? TargetColor;

  public readonly bool TargetVisible;

  public readonly Color? BorderColor;

  public readonly bool BorderVisible;

  public readonly bool ScreenVisible;

  public readonly float PlayerCoverageOpacity;

  public readonly float OpponentCoverageOpacity;

  public readonly Color? PieceBorderColor;

  public readonly bool WobblePiece;

  public readonly bool PulsePiece;

  public readonly bool TremblePiece;

  public readonly bool GreenAlert;

  public readonly bool RedAlert;

  public readonly bool BlockedAlert;

  public readonly bool HideAlerts;

  #endregion

  #region Methods

  private static float CoverageOpacityFor(int count) => CoverageOpacity[Mathf.Min(count, CoverageOpacity.Length - 1)];

  public void Apply() => Square.Apply(this);

  #endregion

  #region Coroutines

  #endregion

  #region Handlers

  #endregion

  #region Constructor

  public SquareState(Square square) {
    Square = square;
  }

  public SquareState(Square square, Player player, GameState gameState, IEnumerable<Move> captureMoves, IEnumerable<Coverage> dangers) {
    var boardState = gameState.BoardState;
    var coverageMap = boardState.CoverageMap;

    var sideToMove = boardState.SideToMove;
    var piece = boardState[square];
    var coveragesTo = coverageMap.To(square);

    float whiteOpacity = CoverageOpacityFor(coveragesTo.Count(c => c.Piece.IsWhite));
    float blackOpacity = CoverageOpacityFor(coveragesTo.Count(c => c.Piece.IsBlack));

    if (piece == null) BlockedAlert = false;
    else if (piece.IsPawn) {
      if (piece.SideType == sideToMove) {
        BlockedAlert = !gameState.AnyMovesFor(piece, square);
      } else {
        if (gameState.AnyMovesFor(piece, square)) BlockedAlert = false;
        else BlockedAlert = !coverageMap.From(square).Any(c => c.Other == null);
      }
    } else if (piece.SideType == sideToMove) {
      BlockedAlert = !gameState.AnyMovesFor(piece, square);
    } else {
      BlockedAlert = !coverageMap.From(square).Any(c => c.Other == null || c.Other.SideType != c.Piece.SideType);
    }

    Square = square;

    PlayerCoverageOpacity = player.IsWhite ? whiteOpacity : blackOpacity;
    OpponentCoverageOpacity = player.IsWhite ? blackOpacity : whiteOpacity;

    GreenAlert = captureMoves.Any(m => m.From == square) || dangers.Any(c => c.From == square);
    RedAlert = captureMoves.Any(m => m.To == square) || dangers.Any(c => c.To == square);

    Color? borderColorForCheck = null;
    if (boardState.WhiteInCheck && boardState.KingSquareFor(SideType.White) == square) {
      borderColorForCheck = player.BorderColorFor(SideType.White);
    } else if (boardState.BlackInCheck && boardState.KingSquareFor(SideType.Black) == square) {
      borderColorForCheck = player.BorderColorFor(SideType.Black);
    }

    if (borderColorForCheck.HasValue) {
      PulsePiece = true;
      WobblePiece = true;
      PieceBorderColor = borderColorForCheck.Value;
      BorderColor = borderColorForCheck.Value;
    }
  }

  #endregion
}
