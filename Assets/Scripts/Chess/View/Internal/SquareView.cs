using UnityEngine;

public abstract class SquareView : BaseView {
  #region Constructor

  public SquareView(Square square, ViewState viewState, Player player) : base(viewState, player) { }

  #endregion
}
