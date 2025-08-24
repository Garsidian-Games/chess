using UnityEngine;
using System;
using System.Collections.Generic;

public sealed class Move : IEquatable<Move> {
  #region Fields

  private readonly int _hash;

  #endregion

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

  #region Methods

  public bool Equals(Move other) {
    if (other is null) return false;
    if (ReferenceEquals(this, other)) return true;
    if (_hash != other._hash) return false;
    return EqualityComparer<MoveFlag>.Default.Equals(Flags, other.Flags)
        && EqualityComparer<Piece>.Default.Equals(Piece, other.Piece)
        && EqualityComparer<Square>.Default.Equals(From, other.From)
        && EqualityComparer<Square>.Default.Equals(To, other.To)
        && EqualityComparer<Square>.Default.Equals(Captures, other.Captures);
  }

  public override bool Equals(object obj) => Equals(obj as Move);

  public override int GetHashCode() => _hash;

  public static bool operator ==(Move left, Move right) {
    if (ReferenceEquals(left, right)) return true;
    if (left is null || right is null) return false;
    return left.Equals(right);
  }
  public static bool operator !=(Move left, Move right) => !(left == right);

  public override string ToString() => string.Format("Move(Piece={0} From={1} To={2} Captures={3})", Piece, From, To, Captures);

  #endregion

  #region Constructor

  public Move(Piece piece, Square from, Square to, MoveFlag flags = MoveFlag.None, Square captures = null) {
    Piece = piece;
    From = from;
    To = to;
    Flags = flags;
    Captures = captures;
    _hash = HashCode.Combine(piece, from, to, flags, captures);
  }

  #endregion
}
