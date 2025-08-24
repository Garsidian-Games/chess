using UnityEngine;

[CreateAssetMenu(fileName = "Piece", menuName = "Game/Piece")]
public class Piece : ScriptableObject {
  #region Constants

  public static PieceType[] PromotionTypes = new PieceType[] { PieceType.Queen, PieceType.Knight, PieceType.Rook, PieceType.Bishop };

  #endregion

  #region Fields

  [SerializeField] private SideType sideType;

  [SerializeField] private PieceType pieceType;

  [SerializeField] private Sprite icon;

  [SerializeField] private Sprite border;

  #endregion

  #region Properties

  public SideType SideType => sideType;

  public PieceType PieceType => pieceType;

  public Sprite Icon => icon;

  public Sprite Border => border;

  public char Char => CharFor(pieceType);
  public bool IsPawn => PieceType == PieceType.Pawn;
  public bool IsRook => PieceType == PieceType.Rook;
  public bool IsKnight => PieceType == PieceType.Knight;
  public bool IsBishop => PieceType == PieceType.Bishop;
  public bool IsQueen => PieceType == PieceType.Queen;
  public bool IsKing => PieceType == PieceType.King;
  public bool IsWhite => SideType == SideType.White;
  public bool IsBlack => SideType == SideType.Black;

  #endregion

  #region Methods

  public static char CharFor(PieceType pieceType) {
    return pieceType switch {
      PieceType.Rook => 'R',
      PieceType.Knight => 'N',
      PieceType.Bishop => 'B',
      PieceType.Queen => 'Q',
      PieceType.King => 'K',
      _ => '\0',
    };
  }

  #endregion
}
