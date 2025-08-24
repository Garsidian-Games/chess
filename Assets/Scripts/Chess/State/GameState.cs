using UnityEngine;
using System.Text;
using System.Linq;
using System.Collections.Generic;

public sealed class GameState {
  #region Constants

  #endregion

  #region Internal

  #endregion

  #region Fields

  private readonly MoveSet moveSet;

  private string _string;

  #endregion

  #region Events

  #endregion

  #region Properties

  public readonly BoardState BoardState;

  public Move[] Moves => moveSet.Moves;

  #endregion

  #region Methods

  public GameState Make(Move move, PieceType promotion = PieceType.None) => new(BoardState.After(move, promotion));

  public override string ToString() {
    if (BoardState.IsRoot) return string.Empty;

    if (BoardState.Move.IsCastleKS) return "O-O";
    if (BoardState.Move.IsCastleQS) return "O-O-O";

    if (string.IsNullOrEmpty(_string)) {
      StringBuilder stringBuilder = new();

      stringBuilder.Append(BoardState.Move.Piece.Char);

      var movesToSquare = moveSet.Moves.Where(move => move.To == BoardState.Move.To);
      if (movesToSquare.Count() > 1) {
        if (BoardState.PiecesOnFile(BoardState.Move.From.File, BoardState.Move.Piece.SideType, BoardState.Move.Piece.PieceType) == 0)
          stringBuilder.Append(BoardState.Move.From.FileChar);
        else if (BoardState.PiecesOnRank(BoardState.Move.From.Rank, BoardState.Move.Piece.SideType, BoardState.Move.Piece.PieceType) == 0)
          stringBuilder.Append(BoardState.Move.From.Rank);
        else stringBuilder.Append(BoardState.Move.From.name);
      }

      if (BoardState.Move.IsCapture) {
        if (BoardState.Move.Piece.IsPawn) stringBuilder.Append(BoardState.Move.From.FileChar);
        stringBuilder.Append('x');
      }

      stringBuilder.Append(BoardState.Move.To.name);

      if (BoardState.Move.IsPromotion) stringBuilder.Append($"={Piece.CharFor(BoardState.Promotion)}");

      if (moveSet.IsMate) stringBuilder.Append("#");
      else if (BoardState.CoverageMap.InCheck(BoardState.SideToMove)) stringBuilder.Append("+");

      _string = stringBuilder.ToString();
    }

    return _string;
  }

  #endregion

  #region Coroutines

  #endregion

  #region Handlers

  #endregion

  #region Constructor

  public GameState(BoardState boardState) {
    BoardState = boardState;
    moveSet = new(BoardState);
  }

  public GameState(Board board, Side white, Side black) {
    BoardState = new(board, white, black);
    moveSet = new(BoardState);
  }

  #endregion
}
