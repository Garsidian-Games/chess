using UnityEngine;
using UnityEngine.Assertions;
using System.Collections.Generic;
using System.Linq;

public sealed class CoverageMap {
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

  #endregion

  #region Fields

  private readonly BoardState boardState;

  private readonly HashSet<Coverage> coverages = new();

  #endregion

  #region Properties

  public readonly bool WhiteInCheck;

  public readonly bool BlackInCheck;

  public Coverage[] Coverages => coverages.ToArray();

  public Coverage[] this[Square square] => coverages.Where(c => c.To == square).ToArray();

  public Coverage[] this[Piece piece, Square square] => coverages.Where(c => c.Piece == piece && c.From == square).ToArray();

  #endregion

  #region Methods

  public bool InCheck(SideType sideType) {
    return sideType switch {
      SideType.White => WhiteInCheck,
      SideType.Black => BlackInCheck,
      _ => throw new System.ArgumentException(string.Format("{0} is not a valid side", sideType)),
    };
  }

  private void AddFor(Square square) => AddFor(boardState[square], square);
  private void AddFor(Piece piece, Square square) {
    if (square == null) throw new System.ArgumentException("Square cannot be null!");
    if (piece == null) return;

    switch (piece.PieceType) {
      case PieceType.Pawn: AddForPawn(piece, square); break;
      case PieceType.Rook: AddForRook(piece, square); break;
      case PieceType.Knight: AddForKnight(piece, square); break;
      case PieceType.Bishop: AddForBishop(piece, square); break;
      case PieceType.Queen: AddForQueen(piece, square); break;
      case PieceType.King: AddForKing(piece, square); break;
    }
  }

  private void AddForPawn(Piece piece, Square from) {
    Assert.IsTrue(piece.IsPawn, string.Format("{0} is not a pawn!", piece));

    var side = boardState.SideFor(piece);
    Add(piece, from, boardState[from.Rank + side.PawnMoveDirection, from.File - 1]);
    Add(piece, from, boardState[from.Rank + side.PawnMoveDirection, from.File + 1]);
  }

  private void AddForRook(Piece piece, Square from) {
    Assert.IsTrue(piece.IsRook, string.Format("{0} is not a rook!", piece));

    AddRay(piece, from, 1, 0);
    AddRay(piece, from, -1, 0);
    AddRay(piece, from, 0, 1);
    AddRay(piece, from, 0, -1);
  }

  private void AddForKnight(Piece piece, Square from) {
    Assert.IsTrue(piece.IsKnight, string.Format("{0} is not a knight!", piece));

    foreach (var movement in KnightMovements)
      Add(piece, from, boardState[from.Rank + movement.x, from.File + movement.y]);
  }

  private void AddForBishop(Piece piece, Square from) {
    Assert.IsTrue(piece.IsBishop, string.Format("{0} is not a bishop!", piece));

    AddRay(piece, from, 1, 1);
    AddRay(piece, from, 1, -1);
    AddRay(piece, from, -1, 1);
    AddRay(piece, from, -1, -1);
  }

  private void AddForQueen(Piece piece, Square from) {
    Assert.IsTrue(piece.IsQueen, string.Format("{0} is not a queen!", piece));

    AddRay(piece, from, 1, 1);
    AddRay(piece, from, 1, -1);
    AddRay(piece, from, -1, 1);
    AddRay(piece, from, -1, -1);
    AddRay(piece, from, 1, 0);
    AddRay(piece, from, -1, 0);
    AddRay(piece, from, 0, 1);
    AddRay(piece, from, 0, -1);
  }

  private void AddForKing(Piece piece, Square from) {
    Assert.IsTrue(piece.IsKing, string.Format("{0} is not a king!", piece));

    foreach (var movement in KingMovements)
      Add(piece, from, boardState[from.Rank + movement.x, from.File + movement.y]);
  }

  private void AddRay(Piece piece, Square from, int rankDirection, int fileDirection) {
    for (int i = 1; i < Board.Dimension; i++) {
      if (!Add(piece, from, boardState[from.Rank + i * rankDirection, from.File + i * fileDirection]))
        break;
    }
  }

  private bool Add(Piece piece, Square from, Square to) {
    if (to == null) return false;

    var other = boardState[to];
    coverages.Add(new(piece, from, to, other));

    return other == null;
  }

  #endregion

  #region Constructor

  public CoverageMap(BoardState boardState) {
    this.boardState = boardState;

    foreach (var square in boardState.Squares) AddFor(square);

    WhiteInCheck = coverages.Any(c => c.Other != null && c.Other.IsKing && c.Other.SideType != c.Piece.SideType && c.Other.IsWhite);
    BlackInCheck = coverages.Any(c => c.Other != null && c.Other.IsKing && c.Other.SideType != c.Piece.SideType && c.Other.IsBlack);
  }

  #endregion
}
