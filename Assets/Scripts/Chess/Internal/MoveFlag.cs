using System;

[Flags]
public enum MoveFlag : ushort {
  None = 0,

  Capture = 1 << 0,
  EnPassant = 1 << 1,
  DoublePush = 1 << 2,
  CastleKingSide = 1 << 3,
  CastleQueenSide = 1 << 4,
  Promotion = 1 << 5,
  GivesCheck = 1 << 6,
}
