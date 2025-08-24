using System;
using System.Collections.Generic;

public sealed class Coverage : IEquatable<Coverage> {
  #region Properties

  public readonly Piece Piece;

  public readonly Piece Other;

  public readonly Square From;

  public readonly Square To;

  #endregion

  #region Fields

  private readonly int _hash;

  #endregion

  #region Methods

  public bool Equals(Coverage other) {
    if (other is null) return false;
    if (ReferenceEquals(this, other)) return true;
    if (_hash != other._hash) return false;
    return EqualityComparer<Piece>.Default.Equals(Piece, other.Piece)
        && EqualityComparer<Piece>.Default.Equals(Other, other.Other)
        && EqualityComparer<Square>.Default.Equals(From, other.From)
        && EqualityComparer<Square>.Default.Equals(To, other.To);
  }

  public override bool Equals(object obj) => Equals(obj as Coverage);

  public override int GetHashCode() => _hash;

  public static bool operator ==(Coverage left, Coverage right) {
    if (ReferenceEquals(left, right)) return true;
    if (left is null || right is null) return false;
    return left.Equals(right);
  }
  public static bool operator !=(Coverage left, Coverage right) => !(left == right);

  public override string ToString() => string.Format("Coverage(Piece={0} From={1} To={2} Other={3})", Piece, From, To, Other);

  #endregion

  #region Constructor

  public Coverage(Piece piece, Square from, Square to, Piece other = null) {
    Piece = piece;
    From = from;
    To = to;
    Other = other;
    _hash = HashCode.Combine(piece, from, to, other);
  }

  #endregion
}