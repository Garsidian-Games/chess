using UnityEngine;
using UnityEngine.Assertions;
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

  private readonly Dictionary<Move, Dictionary<PieceType, GameState>> cache = new();

  private string _string;

  #endregion

  #region Events

  #endregion

  #region Properties

  public bool IsRoot => Previous == null;

  public readonly GameState Previous;

  public readonly BoardState BoardState;

  public readonly int HalfmoveClock;

  public readonly int FullmoveNumber;

  public bool InCheck => BoardState.InCheck;

  public bool IsMate => moveSet.IsMate;

  public Move[] Moves => moveSet.Moves;

  #endregion

  #region Methods

  public Move[] MovesFor(SideType sideType) => moveSet.Moves.Where(move => move.Piece.SideType == sideType).ToArray();

  public Move[] MovesFor(Piece piece, Square from) => moveSet.Moves.Where(move => move.Piece == piece && move.From == from).ToArray();

  public GameState MakeMove(Move move, PieceType promotion = PieceType.None) {
    if (!cache.ContainsKey(move)) cache[move] = new();
    if (!cache[move].ContainsKey(promotion)) cache[move][promotion] = new(this, move, promotion);
    return cache[move][promotion];
  }

  public override string ToString() {
    if (BoardState.IsRoot) return string.Empty;

    if (BoardState.Move.IsCastleKS) return "O-O";
    if (BoardState.Move.IsCastleQS) return "O-O-O";

    if (string.IsNullOrEmpty(_string)) {
      StringBuilder stringBuilder = new();

      var move = BoardState.Move;
      var piece = move.Piece;
      var from = move.From;
      var to = move.To;

      if (!piece.IsPawn) stringBuilder.Append(piece.Char);
      
      var movesToSquare = Previous.MovesFor(Previous.BoardState.SideToMove).Where(move => move.To == to && move.Piece.PieceType == piece.PieceType);
      //foreach (var m in movesToSquare) Debug.Log(m);
      if (movesToSquare.Count() > 1) {
        if (Previous.BoardState.PiecesOnFile(from.File, piece.SideType, piece.PieceType) == 0)
          stringBuilder.Append(from.FileChar);
        else if (Previous.BoardState.PiecesOnRank(from.Rank, piece.SideType, piece.PieceType) == 0)
          stringBuilder.Append(from.Rank);
        else stringBuilder.Append(from.name);
      }

      if (move.IsCapture) {
        if (piece.IsPawn) stringBuilder.Append(from.FileChar);
        stringBuilder.Append('x');
      }

      stringBuilder.Append(to.name);

      if (move.IsPromotion) stringBuilder.Append($"={Piece.CharFor(BoardState.Promotion)}");

      if (moveSet.IsMate) stringBuilder.Append("#");
      else if (BoardState.InCheck) stringBuilder.Append("+");

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

  public GameState(GameState gameState, Move move, PieceType promotion) {
    Assert.IsTrue(gameState.Moves.Contains(move), string.Format("{0} is not valid in this game state!", move));

    Previous = gameState;
    BoardState = gameState.BoardState.After(move, promotion);
    moveSet = new(BoardState);

    if (BoardState.SideToMove == SideType.White) FullmoveNumber = gameState.FullmoveNumber + 1;
    HalfmoveClock = move.IsCapture || move.Piece.IsPawn ? 0 : HalfmoveClock + 1;
  }

  public GameState(Board board, Side white, Side black) {
    BoardState = new(board, white, black);
    moveSet = new(BoardState);

    HalfmoveClock = 0;
    FullmoveNumber = 1;
  }

  #endregion
}
