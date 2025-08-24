using System;
using System.Collections.Generic;

public sealed class Coverage : IEquatable<Coverage> {
  #region Properties

  public readonly Piece Source;

  public readonly Square From;

  public readonly Square To;

  public readonly Piece Subject;

  #endregion

  #region Fields

  private readonly int _hash;

  #endregion

  #region Methods

  public bool Equals(Coverage other) {
    if (other is null) return false;
    if (ReferenceEquals(this, other)) return true;
    if (_hash != other._hash) return false;
    return EqualityComparer<Piece>.Default.Equals(Source, other.Source)
        && EqualityComparer<Piece>.Default.Equals(Subject, other.Subject)
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

  public override string ToString() => string.Format("Coverage(Source={0} From={1} To={2} Subject={3})", Source, From, To, Subject);

  #endregion

  #region Constructor

  public Coverage(Piece source, Square from, Square to, Piece subject = null) {
    Source = source;
    From = from;
    To = to;
    Subject = subject;
    _hash = HashCode.Combine(source, from, to, subject);
  }

  #endregion
}