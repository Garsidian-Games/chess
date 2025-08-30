using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class GameScoreWindow : UIWindow {
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

  [HideInInspector] public ScoreRow.ScoreEvent OnScoreClicked;

  #endregion

  #region Properties

  #endregion

  #region Methods

  public override void Toggle() => throw new System.InvalidOperationException();

  public override void Show() => throw new System.InvalidOperationException();

  public void Show(GameState[] states) {
    rows.ForEach(row => row.Hide());

    for (int index = 0; index < states.Length; index++) {
      int rowIndex = Mathf.FloorToInt(index / 2);
      var row = Get(rowIndex);
      var annotation = states[index].ToString();

      //Debug.LogFormat("{0} {1} {2}", rowIndex, row, annotation);

      if (index % 2 == 0) row.WhiteText = annotation;
      else row.BlackText = annotation;
    }

    ChangeVisibility(true);
  }

  private ScoreRow Get(int index) {
    while (index >= rows.Count) {
      var scoreRow = Instantiate(prefab, container).GetComponent<ScoreRow>();
      scoreRow.OnScoreClicked.AddListener(OnScoreClicked.Invoke);
      rows.Add(scoreRow);
    }
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
