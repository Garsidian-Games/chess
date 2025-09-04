using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public abstract class BaseView {
  #region Constants

  public static float[] CoverageOpacity = new float[] { 0f, 0.2f, 0.4f, 0.6f };

  #endregion

  #region Internal

  #endregion

  #region Fields

  protected readonly ViewState viewState;

  protected readonly Player player;

  protected GameState GameState => viewState.GameState;
  protected IEnumerable<Move> CaptureMoves => viewState.CaptureMoves;
  protected IEnumerable<Coverage> Dangers => viewState.Dangers;

  private readonly Dictionary<Square, SquareState> state = new();

  private SquareState hintFrom;
  private SquareState hintTo;

  #endregion

  #region Events

  #endregion

  #region Properties

  public SquareState this[Square square] => For(square);

  #endregion

  #region Methods

  protected static float CoverageOpacityFor(int count) => CoverageOpacity[Mathf.Min(count, CoverageOpacity.Length - 1)];

  public void Apply() {
    foreach (var square in viewState.BoardState.Squares) For(square).Apply();
  }

  private SquareState For(Square square) {
    if (viewState.HasHint) {
      if (viewState.Hint.From == square) return GetHintFrom(square);
      if (viewState.Hint.To == square) return GetHintTo(square);
    }

    return Get(square);
  }

  private SquareState GetHintFrom(Square square) {
    if (hintFrom == null) hintFrom = CreateHintFrom(square);
    return hintFrom;
  }

  private SquareState GetHintTo(Square square) {
    if (hintTo == null) hintTo = CreateHintTo(square);
    return hintTo;
  }

  protected virtual SquareState Get(Square square) {
    if (!state.ContainsKey(square)) state[square] = Create(square);
    return state[square];
  }

  protected abstract SquareState Create(Square square);

  protected abstract SquareState CreateHintFrom(Square square);

  protected abstract SquareState CreateHintTo(Square square);

  protected virtual SquareState.SquareStateOptions StandardOptionsFor(Square square, bool withAlerts = true) {
    var boardState = GameState.BoardState;
    var coverageMap = boardState.CoverageMap;

    var sideToMove = boardState.SideToMove;
    var piece = boardState[square];
    var coveragesTo = coverageMap.To(square);

    float whiteOpacity = CoverageOpacityFor(coveragesTo.Count(c => c.Piece.IsWhite));
    float blackOpacity = CoverageOpacityFor(coveragesTo.Count(c => c.Piece.IsBlack));

    bool isBlocked;
    if (piece == null) isBlocked = false;
    else if (piece.IsPawn) {
      if (piece.SideType == sideToMove) {
        isBlocked = !GameState.AnyMovesFor(piece, square);
      } else {
        if (GameState.AnyMovesFor(piece, square)) isBlocked = false;
        else isBlocked = !coverageMap.From(square).Any(c => c.Other == null);
      }
    } else if (piece.SideType == sideToMove) {
      isBlocked = !GameState.AnyMovesFor(piece, square);
    } else {
      isBlocked = !coverageMap.From(square).Any(c => c.Other == null || c.Other.SideType != c.Piece.SideType);
    }

    Color? borderColorForCheck = null;
    if (boardState.WhiteInCheck && boardState.KingSquareFor(SideType.White) == square) {
      borderColorForCheck = player.BorderColorFor(SideType.White);
    } else if (boardState.BlackInCheck && boardState.KingSquareFor(SideType.Black) == square) {
      borderColorForCheck = player.BorderColorFor(SideType.Black);
    }

    return new() {
      Square = square,
      OpponentCoverageOpacity = player.IsWhite ? blackOpacity : whiteOpacity,
      PlayerCoverageOpacity = player.IsWhite ? whiteOpacity : blackOpacity,
      GreenAlert = withAlerts && CaptureMoves.Any(m => m.From == square) || Dangers.Any(c => c.From == square),
      RedAlert = withAlerts && CaptureMoves.Any(m => m.To == square) || Dangers.Any(c => c.To == square),
      BlockedAlert = withAlerts && isBlocked,
      PulsePiece = borderColorForCheck.HasValue,
      WobblePiece = borderColorForCheck.HasValue,
      PieceBorderColor = borderColorForCheck,
      BorderColor = borderColorForCheck,
    };
  }

  #endregion

  #region Coroutines

  #endregion

  #region Handlers

  #endregion

  #region Constructor

  public BaseView(ViewState viewState, Player player) {
    this.viewState = viewState;
    this.player = player;
  }

  #endregion
}
