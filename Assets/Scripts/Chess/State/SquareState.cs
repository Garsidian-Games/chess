using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public sealed class SquareState {
  #region Constants

  #endregion

  #region Internal

  public class SquareStateOptions {
    public Square Square { get; set; }
    public float? OpponentCoverageOpacity { get; set; }
    public float? PlayerCoverageOpacity { get; set; }
    public Color? PieceBorderColor { get; set; }
    public Color? BorderColor { get; set; }
    public bool? HighlightVisible { get; set; }
    public bool? ScreenVisible { get; set; }
    public bool? WobblePiece { get; set; }
    public bool? PulsePiece { get; set; }
    public bool? TremblePiece { get; set; }
    public bool? GreenAlert { get; set; }
    public bool? RedAlert { get; set; }
    public bool? BlockedAlert { get; set; }
    public bool? PieceVisible { get; set; }
  }

  #endregion

  #region Fields

  #endregion

  #region Events

  #endregion

  #region Properties

  public readonly Square Square;

  public readonly float OpponentCoverageOpacity;

  public readonly float PlayerCoverageOpacity;

  public readonly Color? PieceBorderColor;

  public readonly Color? BorderColor;

  public readonly bool BorderVisible;

  public readonly bool HighlightVisible;

  public readonly bool ScreenVisible;

  public readonly bool WobblePiece;

  public readonly bool PulsePiece;

  public readonly bool TremblePiece;

  public readonly bool GreenAlert;

  public readonly bool RedAlert;

  public readonly bool BlockedAlert;

  public readonly bool PieceVisible;

  #endregion

  #region Methods

  public void Apply() => Square.Apply(this);

  public override string ToString() {
    return $"SquareState(Square={Square} GreenAlert={GreenAlert} RedAlert={RedAlert})";
  }

  #endregion

  #region Coroutines

  #endregion

  #region Handlers

  #endregion

  #region Constructor

  public SquareState(SquareStateOptions options) {
    Square = options.Square;
    OpponentCoverageOpacity = options.OpponentCoverageOpacity.Value;
    PlayerCoverageOpacity = options.PlayerCoverageOpacity.Value;
    PieceBorderColor = options.PieceBorderColor;
    BorderColor = options.BorderColor;
    BorderVisible = options.BorderColor.HasValue;
    HighlightVisible = options.HighlightVisible ?? false;
    ScreenVisible = options.ScreenVisible ?? false;
    WobblePiece = options.WobblePiece ?? false;
    PulsePiece = options.PulsePiece ?? false;
    TremblePiece = options.TremblePiece ?? false;
    GreenAlert = options.GreenAlert ?? false;
    RedAlert = options.RedAlert ?? false;
    BlockedAlert = options.BlockedAlert ?? false;
    PieceVisible = options.PieceVisible ?? true;
  }

  #endregion
}
