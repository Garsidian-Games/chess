using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public sealed class SquareStateProvider {
  #region Constants

  #endregion

  #region Internal

  public enum Mode {
    Standard,
  }

  #endregion

  #region Fields

  private readonly GameController gameController;

  private readonly Player player;

  private readonly Dictionary<GameState, Dictionary<Square, SquareState>> states = new();

  private readonly Dictionary<GameState, IEnumerable<SquareState>> hints = new();

  private readonly Dictionary<GameState, Dictionary<Square, IEnumerable<SquareState>>> coverageFrom = new();

  private readonly Dictionary<GameState, Dictionary<Square, IEnumerable<SquareState>>> movesFor = new();

  private readonly Dictionary<GameState, Dictionary<Square, IEnumerable<SquareState>>> movesForWithHint = new();

  private Move hint;

  #endregion

  #region Events

  #endregion

  #region Properties

  public bool HasHint => hint != null;

  public Move Hint {
    get => hint;
    set { if (Hint == value) hint = null; else hint = value; }
  }

  public IEnumerable<SquareState> Standard {
    get {
      var standard = StandardDictionary;
      if (!HasHint) return standard.Values;

      if (!hints.ContainsKey(CurrentGameState)) {
        Dictionary<Square, SquareState> withHint = new(standard);
        withHint[Hint.From] = new(standard[Hint.From], Hint);
        withHint[Hint.To] = new(standard[Hint.To], Hint);
        hints[CurrentGameState] = withHint.Values;
      }

      return hints[CurrentGameState];
    }
  }

  private GameState CurrentGameState => gameController.GameManager.GameState;

  private Dictionary<Square, SquareState> StandardDictionary {
    get {
      if (!states.ContainsKey(CurrentGameState)) {
        Dictionary<Square, SquareState> state = new();
        var sideToMove = CurrentGameState.BoardState.SideToMove;
        var captureMoves = CurrentGameState.MovesFor(CurrentGameState.BoardState.SideToMove).Where(m => m.IsCapture);
        var dangers = CurrentGameState.BoardState.CoverageMap.Coverages.Where(c => c.Piece.SideType != sideToMove && c.Other != null && c.Other.SideType == sideToMove);
        foreach (var square in CurrentGameState.BoardState.Squares) state[square] = new(square, player, CurrentGameState, captureMoves, dangers);
        states[CurrentGameState] = state;
      }

      return states[CurrentGameState];
    }
  }

  #endregion

  #region Methods

  public void Clear() {
    states.Clear();
    hints.Clear();
  }

  public IEnumerable<SquareState> CoverageFrom(Square square, Coverage[] coverages, Color pieceBorderColor) {
    if (!coverageFrom.ContainsKey(CurrentGameState)) coverageFrom[CurrentGameState] = new();
    if (!coverageFrom[CurrentGameState].ContainsKey(square)) {
      List<SquareState> states = new();

      foreach (var kvp in StandardDictionary) {
        SquareState state;

        if (kvp.Key == square) {
          state = kvp.Value.CoverageFrom(pieceBorderColor);
        } else if (coverages.Any(c => c.To == kvp.Key)) {
          state = kvp.Value.CoverageTo();
        } else {
          state = kvp.Value.WithoutAlerts();
        }

        states.Add(state);
      }

      coverageFrom[CurrentGameState][square] = states;
    }

    return coverageFrom[CurrentGameState][square];
  }

  public IEnumerable<SquareState> MovesFor(Square square, Move[] moves, Color pieceBorderColor) {
    if (!movesFor.ContainsKey(CurrentGameState)) movesFor[CurrentGameState] = new();
    if (!movesFor[CurrentGameState].ContainsKey(square)) {
      List<SquareState> states = new();

      foreach (var kvp in StandardDictionary) {
        SquareState state;

        if (kvp.Key == square) {
          state = kvp.Value.MovesFrom(pieceBorderColor);
        } else {
          state = moves.Any(move => move.To == kvp.Key) ? kvp.Value.MovesTo(player.TargetColor) : kvp.Value.WithoutAlerts();
        }

        states.Add(state);
      }

      movesFor[CurrentGameState][square] = states;
    }

    var _movesFor = movesFor[CurrentGameState][square];
    if (!HasHint) return _movesFor;

    if (!movesForWithHint.ContainsKey(CurrentGameState)) movesForWithHint[CurrentGameState] = new();
    if (!movesForWithHint[CurrentGameState].ContainsKey(square)) {
      List<SquareState> statesWithHint = new(_movesFor);
      var hintSquareState = statesWithHint.First(state => state.Square == Hint.To);
      statesWithHint[statesWithHint.IndexOf(hintSquareState)] = hintSquareState.MoveHint();
      movesForWithHint[CurrentGameState][square] = statesWithHint;
    }

    return movesForWithHint[CurrentGameState][square];
  }

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
