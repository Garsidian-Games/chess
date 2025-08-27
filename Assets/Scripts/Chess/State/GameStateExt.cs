using System.Text;
using System.Linq;

/// <summary>
/// Extension methods for GameState, including FEN conversion.
/// Keeps all FEN logic self-contained and clean.
/// </summary>
public static class GameStateExt {
  /// <summary>
  /// Converts the current GameState into a valid Stockfish-ready FEN string.
  /// </summary>
  public static string ToFEN(this GameState gameState) {
    StringBuilder fen = new StringBuilder();

    for (int rank = 7; rank >= 0; rank--) {
      int emptyCount = 0;

      for (int file = 0; file < 8; file++) {
        Piece piece = gameState.BoardState.PieceAt(rank, file);

        // ✅ FIX: Only count real pieces, skip placeholders
        if (piece == null || piece.PieceType == PieceType.None) {
          emptyCount++;
        } else {
          // Write accumulated empty squares before adding a piece
          if (emptyCount > 0) {
            fen.Append(emptyCount);
            emptyCount = 0;
          }

          fen.Append(GetPieceChar(piece));
        }
      }

      // Handle trailing empties at end of rank
      if (emptyCount > 0)
        fen.Append(emptyCount);

      // Add '/' except after the last rank
      if (rank > 0)
        fen.Append('/');
    }

    // Side to move
    fen.Append(' ');
    fen.Append(gameState.BoardState.SideToMove == SideType.White ? 'w' : 'b');

    // Castling rights
    fen.Append(' ');
    string castling = GetCastlingRights(gameState);
    fen.Append(string.IsNullOrEmpty(castling) ? "-" : castling);

    // En passant
    fen.Append(' ');
    fen.Append(GetEnPassantTarget(gameState));

    fen.Append(' ');
    fen.Append(gameState.HalfmoveClock);

    fen.Append(' ');
    fen.Append(gameState.FullmoveNumber);


    return fen.ToString();
  }

  /// <summary>
  /// Gets the FEN-compatible character for a given piece.
  /// </summary>
  private static char GetPieceChar(Piece piece) {
    char symbol = piece.PieceType switch {
      PieceType.Pawn => 'p',
      PieceType.Knight => 'n',
      PieceType.Bishop => 'b',
      PieceType.Rook => 'r',
      PieceType.Queen => 'q',
      PieceType.King => 'k',
      _ => '?'
    };

    return piece.IsWhite ? char.ToUpper(symbol) : symbol;
  }

  /// <summary>
  /// Builds the castling rights string (e.g. "KQkq" or "-").
  /// </summary>
  private static string GetCastlingRights(GameState gameState) {
    string rights = "";

    if (gameState.Moves.Any(move => move.IsCastleKS && move.Piece.SideType == SideType.White)) rights += "K";
    if (gameState.Moves.Any(move => move.IsCastleQS && move.Piece.SideType == SideType.White)) rights += "Q";
    if (gameState.Moves.Any(move => move.IsCastleKS && move.Piece.SideType == SideType.Black)) rights += "k";
    if (gameState.Moves.Any(move => move.IsCastleQS && move.Piece.SideType == SideType.Black)) rights += "q";

    return rights;
  }

  /// <summary>
  /// Returns the en passant target square (e.g. "e3"), or "-" if none.
  /// </summary>
  private static string GetEnPassantTarget(GameState gameState) {
    var ep = gameState.Moves.FirstOrDefault(move => move.IsEnPassant);
    if (ep == null) return "-";
    return ep.To.name;
  }
}
