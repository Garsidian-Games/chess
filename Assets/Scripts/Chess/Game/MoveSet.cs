using UnityEngine;
using UnityEngine.Assertions;
using System.Collections.Generic;
using System.Linq;

public sealed class MoveSet {
  #region Constants

  private readonly int[] CastleKingSideFiles = new[] { 5, 6 };

  private readonly int[] CastleQueenSideFiles = new[] { 2, 3 };

  #endregion

  #region Internal

  #endregion

  #region Fields

  private readonly Side sideToMove;

  private readonly BoardState boardState;

  private readonly HashSet<Move> moves = new();

  #endregion

  #region Events

  #endregion

  #region Properties

  public readonly bool IsMate;

  public readonly bool IsStalemate;

  public Move[] Moves => moves.ToArray();

  #endregion

  #region Methods

  private void AddPawnMovesFrom(Square from) {
    var piece = boardState[from];
    if (piece == null || !piece.IsPawn) return;

    var side = boardState.SideFor(piece);

    if (!AddPawnMove(piece, from, boardState[from.Rank + side.PawnMoveDirection, from.File])) return;
    if (from.Rank != side.PawnRank) return;

    AddPawnMove(piece, from, boardState[from.Rank + (side.PawnMoveDirection * 2), from.File], true);
  }

  private bool AddPawnMove(Piece piece, Square from, Square to, bool doublePush = false) {
    if (to == null) return false;

    var other = boardState[to];
    if (other != null) return false;

    var side = boardState.SideFor(piece);
    Add(new(piece, from, to, doublePush ? MoveFlag.DoublePush : to.Rank == side.PromotionRank ? MoveFlag.Promotion : 0, to));
    return true;
  }

  private void TryAddEnPassant(Move move) {
    if (move == null) return;
    if (!move.IsDoublePush) return;

    var pawn = move.Piece;
    var side = boardState.SideFor(pawn);
    var from = move.From;
    var ends = move.To;
    var thru = boardState[ends.Rank + (-1 * side.PawnMoveDirection), ends.File];
    var hits = boardState.CoverageMap.Coverages.Where(coverage => coverage.To == thru && coverage.Piece.IsPawn && coverage.Piece.SideType != pawn.SideType);

    foreach (var hit in hits) Add(new(hit.Piece, hit.From, hit.To, MoveFlag.Capture | MoveFlag.EnPassant, ends));
  }

  private void TryAddQueenSideCastle(int rank) {
    if (boardState.IsPieceOn(boardState[rank, 1])) return;
    if (boardState.IsPieceOn(boardState[rank, 2])) return;
    if (boardState.IsPieceOn(boardState[rank, 3])) return;
    if (boardState.IsDirty(boardState[rank, 0])) return;

    var kingSquare = boardState[rank, 4];
    if (boardState.IsDirty(kingSquare)) return;

    if (boardState.CoverageMap.Coverages.Any(c => c.Piece.SideType != sideToMove.SideType && c.To.Rank == rank && CastleQueenSideFiles.Contains(c.To.File)))
      return;

    Add(new(boardState[kingSquare], kingSquare, boardState[rank, 2], MoveFlag.CastleQueenSide));
  }

  private void TryAddKingSideCastle(int rank) {
    if (boardState.IsPieceOn(boardState[rank, 5])) return;
    if (boardState.IsPieceOn(boardState[rank, 6])) return;
    if (boardState.IsDirty(boardState[rank, 7])) return;

    var kingSquare = boardState[rank, 4];
    if (boardState.IsDirty(kingSquare)) return;

    if (boardState.CoverageMap.Coverages.Any(c => c.Piece.SideType != sideToMove.SideType && c.To.Rank == rank && CastleKingSideFiles.Contains(c.To.File)))
      return;

    Add(new(boardState[kingSquare], kingSquare, boardState[rank, 6], MoveFlag.CastleKingSide));
  }

  private Move For(Coverage coverage) {
    if (coverage.Other == null) return new Move(coverage.Piece, coverage.From, coverage.To);
    var side = boardState.SideFor(coverage.Piece);
    return new Move(
      coverage.Piece,
      coverage.From,
      coverage.To,
      MoveFlag.Capture |
        (coverage.Other.IsKing ? MoveFlag.GivesCheck : 0) |
        (coverage.Piece.IsPawn && coverage.To.Rank == side.PromotionRank ? MoveFlag.Promotion : 0),
      coverage.To
    );
  }

  private void Add(Move move) {
    var next = boardState.After(move, move.IsPromotion ? Piece.PromotionTypes[0] : PieceType.None);
    if (next.CoverageMap.InCheck(sideToMove.SideType)) return;
    moves.Add(move);
  }

  #endregion

  #region Constructor

  public MoveSet(BoardState boardState) {
    sideToMove = boardState.SideFor(boardState.SideToMove);
    this.boardState = boardState;

    foreach (var coverage in boardState.CoverageMap.Coverages) {
      if (coverage.Piece.SideType != sideToMove.SideType) continue;
      if (coverage.Piece.IsPawn) {
        if (coverage.Other == null) continue;
        if (coverage.Other.SideType == sideToMove.SideType) continue;
      } else if (coverage.Other != null && coverage.Other.SideType == sideToMove.SideType) continue;

      Add(For(coverage));
    }

    foreach (var square in boardState.Squares) AddPawnMovesFrom(square);

    TryAddEnPassant(boardState.Move);

    if (!boardState.CoverageMap.InCheck(sideToMove.SideType)) {
      TryAddKingSideCastle(sideToMove.HomeRank);
      TryAddQueenSideCastle(sideToMove.HomeRank);
    }

    IsMate = moves.Count == 0;
  }

  #endregion
}
