using UnityEngine;
using UnityEngine.Assertions;
using System.Collections.Generic;

public sealed class BoardState {
  #region Fields

  private readonly Board board;

  private readonly Side white;

  private readonly Side black;

  private readonly Dictionary<Square, Piece> state = new();

  #endregion

  #region Properties

  public SideType SideToMove { get; private set; }

  public Square[] Squares => board.Squares;

  public Square this[int index] => board[index];

  public Square this[int rank, int file] => board[rank, file];

  public Piece this[Square square] => IsPieceOn(square) ? state[square] : null;

  private Side SideFor(Piece piece) => SideFor(piece.SideType);

  private Side SideFor(SideType sideType) => sideType == SideType.White ? white : black;

  #endregion

  #region Methods

  public int PiecesOnFile(int file, SideType sideType, PieceType pieceType) {
    int count = 0;
    for (int rank = 0; rank < Board.Ranks; rank++) {
      if (IsMatch(this[board[rank, file]], sideType, pieceType)) count++;
    }
    return count;
  }

  public int PiecesOnRank(int rank, SideType sideType, PieceType pieceType) {
    int count = 0;
    for (int file = 0; file < Board.Files; file++) {
      if (IsMatch(this[board[rank, file]], sideType, pieceType)) count++;
    }
    return count;
  }

  public bool IsPieceOn(Square square) => state.ContainsKey(square);

  private static bool IsMatch(Piece piece, SideType sideType, PieceType pieceType) {
    if (piece == null) return false;
    if (piece.SideType != sideType) return false;
    if (piece.PieceType != pieceType) return false;

    return true;
  }

  private void Setup(Side side) {
    for (int file = 0; file < Board.Files; file++) {
      state[board[side.PawnRank, file]] = side.Pawn;
    }

    state[board[side.HomeRank, 0]] = side.Rook;
    state[board[side.HomeRank, 1]] = side.Knight;
    state[board[side.HomeRank, 2]] = side.Bishop;
    state[board[side.HomeRank, 3]] = side.Queen;
    state[board[side.HomeRank, 4]] = side.King;
    state[board[side.HomeRank, 5]] = side.Bishop;
    state[board[side.HomeRank, 6]] = side.Knight;
    state[board[side.HomeRank, 7]] = side.Rook;
  }

  #endregion

  #region Constructor

  public BoardState(BoardState boardState, Move move, PieceType promotion) {
    Assert.IsTrue(promotion == PieceType.None ? !move.IsPromotion : move.IsPromotion);

    SideToMove = boardState.SideToMove == SideType.White ? SideType.Black : SideType.White;

    board = boardState.board;
    white = boardState.white;
    black = boardState.black;

    state = new(boardState.state);

    if (move.IsCapture) state.Remove(move.Captures);

    if (move.IsCastleKS) {
      state.Remove(board[move.From.Rank, 7]);
      state[board[move.From.Rank, 5]] = SideFor(move.Piece).Rook;
    }

    if (move.IsCastleQS) {
      state.Remove(board[move.From.Rank, 0]);
      state[board[move.From.Rank, 3]] = SideFor(move.Piece).Rook;
    }

    state.Remove(move.From);
    state[move.To] = move.IsPromotion ? SideFor(move.Piece).For(promotion) : move.Piece;
  }

  public BoardState(Board board, Side white, Side black) {
    SideToMove = SideType.White;

    this.board = board;
    this.white = white;
    this.black = black;

    Setup(white);
    Setup(black);
  }

  #endregion
}
