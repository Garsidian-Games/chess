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

  private readonly BoardState boardState;

  private readonly PieceType promotion;

  private string _string;

  private readonly Dictionary<Move, Dictionary<PieceType, GameState>> cache = new();

  #endregion

  #region Events

  #endregion

  #region Properties

  public bool IsFresh => Move == null;

  public Piece this[Square square] => boardState[square];

  public SideType SideToMove => boardState.SideToMove;

  public Move[] LegalMoves { get; private set; }

  public bool IsMate { get; private set; }

  public bool InCheck { get; private set; }

  #endregion

  #region Methods

  public readonly Move Move;

  public readonly GameState Previous;

  public GameState For(Move move, PieceType promotion = PieceType.None) {
    if (!cache.ContainsKey(move)) cache[move] = new();
    if (!cache[move].ContainsKey(promotion)) cache[move][promotion] = new(this, move, promotion);
    return cache[move][promotion];
  }

  public override string ToString() {
    if (IsFresh) return string.Empty;

    if (Move.IsCastleKS) return "O-O";
    if (Move.IsCastleQS) return "O-O-O";

    if (string.IsNullOrEmpty(_string)) {
      StringBuilder stringBuilder = new();

      stringBuilder.Append(Move.Piece.Char);

      var movesToSquare = LegalMoves.Where(move => move.To == Move.To);
      if (movesToSquare.Count() > 1) {
        if (boardState.PiecesOnFile(Move.From.File, Move.Piece.SideType, Move.Piece.PieceType) == 0) stringBuilder.Append(Move.From.FileChar);
        else if (boardState.PiecesOnRank(Move.From.Rank, Move.Piece.SideType, Move.Piece.PieceType) == 0) stringBuilder.Append(Move.From.Rank);
        else stringBuilder.Append(Move.From.name);
      }

      if (Move.IsCapture) {
        if (Move.Piece.IsPawn) stringBuilder.Append(Move.From.FileChar);
        stringBuilder.Append('x');
      }

      stringBuilder.Append(Move.To.name);

      if (Move.IsPromotion) stringBuilder.Append($"={Piece.CharFor(promotion)}");

      if (IsMate) stringBuilder.Append("#");
      else if (InCheck) stringBuilder.Append("+");

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
    Move = move;
    Previous = gameState;
    this.promotion = promotion;
    boardState = new(gameState.boardState, move, promotion);
  }

  public GameState(BoardState boardState) {
    this.boardState = boardState;
  }

  #endregion
}
