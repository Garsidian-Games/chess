using UnityEngine;

[CreateAssetMenu(fileName = "Side", menuName = "Game/Side")]
public class Side : ScriptableObject {
  #region Constants

  #endregion

  #region Internal

  #endregion

  #region Fields

  [Header("Settings")]
  [SerializeField] private SideType sideType;
  [SerializeField] [Range(-1, 1)] private int pawnMoveDirection;
  [SerializeField] [Range(0, 7)] private int homeRank;
  [SerializeField] [Range(0, 7)] private int pawnRank;
  [SerializeField] [Range(0, 7)] private int promotionRank;

  [Header("Pieces")]
  [SerializeField] private Piece pawn;
  [SerializeField] private Piece rook;
  [SerializeField] private Piece knight;
  [SerializeField] private Piece bishop;
  [SerializeField] private Piece queen;
  [SerializeField] private Piece king;

  #endregion

  #region Events

  #endregion

  #region Properties

  public SideType SideType => sideType;

  public int PawnMoveDirection => pawnMoveDirection;

  public int HomeRank => homeRank;

  public int PawnRank => pawnRank;

  public int PromotionRank => promotionRank;

  public Piece Pawn => pawn;

  public Piece Rook => rook;

  public Piece Knight => knight;

  public Piece Bishop => bishop;

  public Piece Queen => queen;

  public Piece King => king;

  #endregion

  #region Methods

  public Piece For(PieceType pieceType) {
    return pieceType switch {
      PieceType.Pawn => pawn,
      PieceType.Rook => rook,
      PieceType.Knight => knight,
      PieceType.Bishop => bishop,
      PieceType.Queen => queen,
      PieceType.King => king,
      _ => null,
    };
  }

  #endregion

  #region Coroutines

  #endregion

  #region Handlers

  #endregion

  #region Lifecycle

  #endregion
}
