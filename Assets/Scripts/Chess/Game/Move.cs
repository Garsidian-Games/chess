using UnityEngine;

public sealed class Move {
  #region Properties

  public readonly MoveFlag Flags;

  public readonly Piece Piece;

  public readonly Square From;

  public readonly Square To;

  public readonly Square Captures;

  public bool IsCapture => (Flags & MoveFlag.Capture) != 0;

  public bool IsEnPassant => (Flags & MoveFlag.EnPassant) != 0;

  public bool IsDoublePush => (Flags & MoveFlag.DoublePush) != 0;

  public bool IsCastleKS => (Flags & MoveFlag.CastleKingSide) != 0;

  public bool IsCastleQS => (Flags & MoveFlag.CastleQueenSide) != 0;

  public bool IsPromotion => (Flags & MoveFlag.Promotion) != 0;

  public bool GivesCheck => (Flags & MoveFlag.GivesCheck) != 0;

  #endregion

  #region Constructor

  public Move(Piece piece, Square from, Square to, MoveFlag flags = MoveFlag.None, Square captures = null) {
    Piece = piece;
    From = from;
    To = to;
    Flags = flags;
    Captures = captures;
  }

  #endregion
}
