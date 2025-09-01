using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public sealed class SquareStateProvider {
  #region Constants

  #endregion

  #region Internal

  #endregion

  #region Fields

  private readonly GameController gameController;

  private readonly Player player;

  private readonly Dictionary<GameState, Dictionary<Square, SquareState>> squareStates = new();

  #endregion

  #region Events

  #endregion

  #region Properties

  public SquareState[] SquareStates => CurrentSquareStates.Values.ToArray();

  public SquareState this[Square square] => CurrentSquareStates[square];

  private Dictionary<Square, SquareState> CurrentSquareStates {
    get {
      var gameState = gameController.GameManager.GameState;

      if (!squareStates.ContainsKey(gameState)) {
        squareStates[gameState] = new();
        var sideToMove = gameState.BoardState.SideToMove;
        var captureMoves = gameState.MovesFor(gameState.BoardState.SideToMove).Where(m => m.IsCapture);
        var dangers = gameState.BoardState.CoverageMap.Coverages.Where(c => c.Piece.SideType != sideToMove && c.Other != null && c.Other.SideType == sideToMove);
        foreach (var square in gameState.BoardState.Squares) squareStates[gameState][square] = new(square, player, gameState, captureMoves, dangers);
      }

      return squareStates[gameState];
    }
  }

  #endregion

  #region Methods

  #endregion

  #region Coroutines

  #endregion

  #region Handlers

  #endregion

  #region Lifecycle

  public SquareStateProvider(GameController gameController, Player player) {
    this.gameController = gameController;
    this.player = player;
  }

  #endregion
}
