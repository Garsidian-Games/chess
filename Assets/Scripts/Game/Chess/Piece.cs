using UnityEngine;

[CreateAssetMenu(fileName = "Piece", menuName = "Game/Piece")]
public class Piece : ScriptableObject {
  #region Fields

  [SerializeField] private Side side;

  [SerializeField] private Sprite icon;

  [SerializeField] private Sprite border;

  #endregion

  #region Properties

  public Side Side => side;

  public Sprite Icon => icon;

  public Sprite Border => border;

  #endregion

  #region Methods

  #endregion
}
