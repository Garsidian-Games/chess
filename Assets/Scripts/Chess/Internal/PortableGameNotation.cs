using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public sealed class PortableGameNotation {
  #region Constants

  #endregion

  #region Internal

  #endregion

  #region Fields

  private readonly Board board;

  private readonly string pgn;

  #endregion

  #region Events

  #endregion

  #region Properties

  public readonly bool IsValid;

  public readonly bool EndsInCheckmate;

  public GameState GameState { get; private set; }

  #endregion

  #region Methods

  public override string ToString() => pgn;

  #endregion

  #region Coroutines

  #endregion

  #region Handlers

  #endregion

  #region Constructor

  public PortableGameNotation(Board board, Side white, Side black, string pgn) {
    this.board = board;
    this.pgn = pgn;

    GameState = new(board, white, black);

    bool isInvalid = false;
    bool wasJustDot = false;
    Queue<string> parts = new(pgn.Split(" "));
    //Debug.Log(string.Join(":", pgn.Split(" ")));
    if (parts.Count == 0) isInvalid = true;
    while (parts.Count > 0) {
      var str = parts.Dequeue();
      if (string.IsNullOrWhiteSpace(str)) {
        // Debug.Log("Trailing whitespace");
        break;
      }

      string moveStr;
      if (str.Contains('.')) {
        wasJustDot = true;
        var subParts = str.Split('.');
        var moveNumberStr = subParts[0];
        moveStr = subParts[1];
      } else {
        wasJustDot = false;
        moveStr = str;
      }

      //Debug.Log(moveStr);
      if (moveStr.Length < 2) {
        // support for `1. e4` vs `1.e4`
        if (wasJustDot) continue;
        isInvalid = true;
        Debug.Log("Malformed text");
        break;
      }

      var lastCharacter = moveStr.Last();
      if (lastCharacter == '#') {
        IsValid = true;
        EndsInCheckmate = true;
        moveStr = moveStr[..^1];
      }

      var moves = GameState.MovesFor(GameState.BoardState.SideToMove);
      Move move = null;

      bool givesCheck = lastCharacter == '+';
      if (givesCheck) moveStr = moveStr[..^1];

      if (moveStr == "O-O") {
        move = moves.FirstOrDefault(move => move.IsCastleKS);
        if (move == null) {
          isInvalid = true;
          Debug.Log("Missing King Side Castle");
          break;
        }
        GameState = GameState.MakeMove(move);
        continue;
      }

      if (moveStr == "O-O-O") {
        move = moves.FirstOrDefault(move => move.IsCastleQS);
        if (move == null) {
          isInvalid = true;
        }
        GameState = GameState.MakeMove(move);
        continue;
      }

      PieceType promotion;
      if (moveStr.Contains('=')) {
        var promoBits = moveStr.Split('=');
        moveStr = promoBits[0];
        promotion = Piece.TypeFrom(promoBits[1][0]);
      } else promotion = PieceType.None;

      PieceType firstCharPieceType = Piece.TypeFrom(moveStr[0]);
      PieceType pieceType;
      if (firstCharPieceType == PieceType.None) pieceType = PieceType.Pawn;
      else {
        pieceType = firstCharPieceType;
        moveStr = moveStr[1..];
      }

      //Debug.Log(promotion);
      //Debug.Log(pieceType);
      //Debug.Log(moveStr);

      string squareName;
      bool isCapture;
      char? pawnFile = null;
      if (moveStr.Contains('x')) {
        var captureBits = moveStr.Split('x');
        moveStr = captureBits[0];
        squareName = captureBits[1];
        isCapture = true;

        if (pieceType == PieceType.Pawn) {
          pawnFile = moveStr.Last();
          moveStr = moveStr[..^1];
        }
      } else {
        isCapture = false;
        if (moveStr.Length < 2) {
          isInvalid = true;
          Debug.Log("Malformed text");
          break;
        }
        squareName = moveStr[^2..];
        moveStr = moveStr[..^2];
      }

      //Debug.Log(squareName);
      //Debug.Log(moveStr);

      string fromSquare = null;
      char? fromFile = null;
      char? fromRank = null;
      if (moveStr.Length == 2) fromSquare = moveStr;
      else if (moveStr.Length == 1) {
        if ((uint)(moveStr[0] - 'a') <= 7) fromFile = moveStr[0];
        else fromRank = moveStr[0];
      }

      //foreach (var m in moves) Debug.Log(m);
      //Debug.Log(squareName);
      //Debug.Log(string.Join(",", moves.Select(move => move.To.name)));
      //Debug.Log(moves.Count(move => move.To.name == squareName));
      //Debug.Log(moves.Count(move => move.IsCapture == isCapture));
      //Debug.Log(moves.Count(move => move.Piece.PieceType == pieceType));
      moves = moves.Where(move => move.To.name == squareName && move.IsCapture == isCapture && move.Piece.PieceType == pieceType).ToArray();
      //Debug.Log("Filtered:");
      //foreach (var m in moves) Debug.Log(m);

      if (!string.IsNullOrEmpty(fromSquare)) move = moves.FirstOrDefault(move => move.From.name == fromSquare);
      else if (fromFile.HasValue) move = moves.FirstOrDefault(move => move.From.FileChar == fromFile.Value);
      else if (fromRank.HasValue) move = moves.FirstOrDefault(move => move.From.Rank == fromFile.Value);
      else if (moves.Count() == 1) move = moves.First();

      if (move == null) {
        Debug.LogFormat("Missing Move {0} {1} {2} {3} {4} {5} {6} {7}", fromSquare, fromFile, fromRank, moves.Count(), moveStr, squareName, isCapture, pieceType);
        //Debug.Log(fromSquare);
        //Debug.Log(fromFile);
        //Debug.Log(fromRank);
        //Debug.Log(moves.Count());
        //Debug.Log(moveStr);
        isInvalid = true;
        break;
      }

      GameState = GameState.MakeMove(move, promotion);
      if (EndsInCheckmate) break;
    }

    if (GameState.IsMate) {
      IsValid = true;
      EndsInCheckmate = true;
    } else if (isInvalid) GameState = null;
    else IsValid = true;
  }

  #endregion
}
