using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class GameScoreWindow : MonoBehaviour {
  #region Constants

  #endregion

  #region Internal

  #endregion

  #region Fields

  [SerializeField] private GameObject prefab;

  [SerializeField] private RectTransform container;

  private readonly List<ScoreRow> rows = new();

  #endregion

  #region Events

  #endregion

  #region Properties

  #endregion

  #region Methods

  public void Show(GameState[] states) {
    gameObject.SetActive(true);
    rows.ForEach(row => row.Hide());

    for (int index = 0; index < states.Length; index++) {
      int rowIndex = Mathf.FloorToInt(index / 2);
      var row = Get(rowIndex);
      var annotation = states[index].ToString();

      Debug.LogFormat("{0} {1} {2}", rowIndex, row, annotation);

      if (index % 2 == 0) row.WhiteText = annotation;
      else row.BlackText = annotation;
    }
  }

  public void Hide() {
    gameObject.SetActive(false);
  }

  private ScoreRow Get(int index) {
    while (index >= rows.Count) rows.Add(Instantiate(prefab, container).GetComponent<ScoreRow>());
    var row = rows[index];
    row.Show();
    return row;
  }

  #endregion

  #region Coroutines

  #endregion

  #region Handlers

  #endregion

  #region Lifecycle

  #endregion
}
