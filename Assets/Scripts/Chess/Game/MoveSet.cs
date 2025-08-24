using UnityEngine;
using UnityEngine.Assertions;
using System.Collections.Generic;
using System.Linq;

public sealed class MoveSet {
  #region Constants

  private readonly static Vector2Int[] KingMovements = new Vector2Int[] {
    new(-1, 1),  new(0, 1),  new(1, 1),
    new(-1, 0),              new(1, 0),
    new(-1, -1), new(0, -1), new(1, -1),
  };

  private readonly static Vector2Int[] KnightMovements = new Vector2Int[] {
    new(1, 2), new(1, -2), new(-1, 2), new(-1, -2),
    new(2, 1), new(2, -1), new(-2, 1), new(-2, -1),
  };

  private readonly int[] CastleKingSideFiles = new[] { 5, 6 };

  private readonly int[] CastleQueenSideFiles = new[] { 2, 3 };

  #endregion

  #region Internal

  #endregion

  #region Fields

  private readonly BoardState boardState;

  private readonly HashSet<Move> moves = new();

  private readonly HashSet<Coverage> coverages = new();

  #endregion

  #region Events

  #endregion

  #region Properties

  public Move[] Moves => moves.ToArray();

  #endregion

  #region Methods

  private void AddMovesFrom(Square square) => AddMovesFrom(boardState[square], square);
  private void AddMovesFrom(Piece piece, Square square) {
    if (square == null) throw new System.ArgumentException("Square cannot be null!");
    if (piece == null) return;

    switch (piece.PieceType) {
      case PieceType.Pawn: AddPawnMovesFrom(piece, square); break;
      case PieceType.Rook: AddRookMovesFrom(piece, square); break;
      case PieceType.Knight: AddKnightMovesFrom(piece, square); break;
      case PieceType.Bishop: AddBishopMovesFrom(piece, square); break;
      case PieceType.Queen: AddQueenMovesFrom(piece, square); break;
      case PieceType.King: AddKingMovesFrom(piece, square); break;
    }
  }

  private void AddPawnMovesFrom(Piece piece, Square from) {
    Assert.IsTrue(piece.IsPawn, string.Format("{0} is not a pawn!", piece));
  }

  private void AddRookMovesFrom(Piece piece, Square from) {
    Assert.IsTrue(piece.IsRook, string.Format("{0} is not a rook!", piece));

    AddMovementRayFrom(piece, from, 1, 0);
    AddMovementRayFrom(piece, from, -1, 0);
    AddMovementRayFrom(piece, from, 0, 1);
    AddMovementRayFrom(piece, from, 0, -1);
  }

  private void AddKnightMovesFrom(Piece piece, Square from) {
    Assert.IsTrue(piece.IsKnight, string.Format("{0} is not a knight!", piece));

    foreach (var movement in KnightMovements)
      TryAddMove(piece, from, boardState[from.Rank + movement.x, from.File + movement.y]);
  }

  private void AddBishopMovesFrom(Piece piece, Square from) {
    Assert.IsTrue(piece.IsBishop, string.Format("{0} is not a bishop!", piece));

    AddMovementRayFrom(piece, from, 1, 1);
    AddMovementRayFrom(piece, from, 1, -1);
    AddMovementRayFrom(piece, from, -1, 1);
    AddMovementRayFrom(piece, from, -1, -1);
  }

  private void AddQueenMovesFrom(Piece piece, Square from) {
    Assert.IsTrue(piece.IsQueen, string.Format("{0} is not a queen!", piece));

    AddMovementRayFrom(piece, from, 1, 1);
    AddMovementRayFrom(piece, from, 1, -1);
    AddMovementRayFrom(piece, from, -1, 1);
    AddMovementRayFrom(piece, from, -1, -1);
    AddMovementRayFrom(piece, from, 1, 0);
    AddMovementRayFrom(piece, from, -1, 0);
    AddMovementRayFrom(piece, from, 0, 1);
    AddMovementRayFrom(piece, from, 0, -1);
  }

  private void AddKingMovesFrom(Piece piece, Square from) {
    Assert.IsTrue(piece.IsKing, string.Format("{0} is not a king!", piece));

    foreach (var movement in KingMovements)
      TryAddMove(piece, from, boardState[from.Rank + movement.x, from.File + movement.y]);
  }

  private void AddMovementRayFrom(Piece piece, Square from, int rankDirection, int fileDirection) {
    for (int i = 1; i < Board.Dimension; i++) {
      if (!TryAddMove(piece, from, boardState[from.Rank + i * rankDirection, from.File + i * fileDirection]))
        break;
    }
  }

  private bool TryAddMove(Piece piece, Square from, Square to) {
    if (to == null) return false;

    var other = boardState[to];
    if (other == null) {
      AddMove(piece, from, to);
      return true;
    }

    if (piece.SideType == other.SideType) AddCoverage(piece, from, to, other);
    else AddMove(piece, from, to, other);
    return false;
  }

  private void AddMove(Piece piece, Square from, Square to, Piece captured) => AddMove(piece, from, to, to, captured);
  private void AddMove(Piece piece, Square from, Square to, Square captures, Piece captured) {
    moves.Add(new(piece, from, to, MoveFlag.Capture | (captured.PieceType == PieceType.King ? MoveFlag.GivesCheck : 0), captures));
    AddCoverage(piece, from, to, captured);
  }

  private void AddMove(Piece piece, Square from, Square to) {
    moves.Add(new(piece, from, to));
    AddCoverage(piece, from, to);
  }

  private void AddCoverage(Piece piece, Square from, Square to, Piece other = null) => coverages.Add(new(piece, from, to, other));

  #endregion

  #region Coroutines

  #endregion

  #region Handlers

  #endregion

  #region Constructor

  public MoveSet(BoardState boardState) {
    this.boardState = boardState;
    foreach (var square in boardState.Squares) AddMovesFrom(square);
  }

  #endregion
}
