using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections.Generic;

[RequireComponent(typeof(GridLayoutGroup))]
public class Board : MonoBehaviour {
  #region Constants

  public const int Dimension = 8;

  public const int Files = Dimension;

  public const int Ranks = Dimension;

  public const int SquareCount = Files * Ranks;

  #endregion

  #region Internal

  #endregion

  #region Fields

  [SerializeField] private GameObject squarePrefab;

  private readonly List<Square> squares = new();

  private GridLayoutGroup gridLayoutGroup;

  #endregion

  #region Events

  [HideInInspector] public UnityEvent OnReady;
  [HideInInspector] public Square.SquareEvent OnSquareClicked;
  [HideInInspector] public Square.SquareEvent OnSquareDragBegan;
  [HideInInspector] public Square.SquareEvent OnSquareDragEnded;
  [HideInInspector] public Square.SquareEvent OnSquareDropped;
  [HideInInspector] public Square.SquareEvent OnSquareEntered;
  [HideInInspector] public Square.SquareEvent OnSquareExited;

  #endregion

  #region Properties

  public bool IsReady { get; private set; }

  public Square[] Squares => squares.ToArray();

  public Square this[int index] => squares[index];

  public Square this[int rank, int file] {
    get {
      if (rank < 0 || file < 0) return null;
      if (rank >= Ranks || file >= Files) return null;

      return this[rank * 8 + file];
    }
  }

  #endregion

  #region Methods

  public void Render(GameState gameState) {
    foreach (var square in squares) square.Piece = gameState[square];
  }

  private void Add(Square square) {
    square.OnClicked.AddListener(OnSquareClicked.Invoke);
    square.OnDragBegan.AddListener(OnSquareDragBegan.Invoke);
    square.OnDragEnded.AddListener(OnSquareDragEnded.Invoke);
    square.OnDropped.AddListener(OnSquareDropped.Invoke);
    square.OnEntered.AddListener(OnSquareEntered.Invoke);
    square.OnExited.AddListener(OnSquareExited.Invoke);
    squares.Add(square);
  }

  #endregion

  #region Coroutines

  #endregion

  #region Handlers

  #endregion

  #region Lifecycle

  private void Start() {
    for (int i = 0; i < SquareCount; i++) Add(Instantiate(squarePrefab, transform).GetComponent<Square>());
    IsReady = true;
    OnReady.Invoke();
  }

  private void Awake() {
    gridLayoutGroup = GetComponent<GridLayoutGroup>();
    gridLayoutGroup.spacing = Vector2.zero;
    gridLayoutGroup.startCorner = GridLayoutGroup.Corner.LowerLeft;
    gridLayoutGroup.startAxis = GridLayoutGroup.Axis.Horizontal;
    gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
    gridLayoutGroup.constraintCount = Files;
  }

  #endregion
}
