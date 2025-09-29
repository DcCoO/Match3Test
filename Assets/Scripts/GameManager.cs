using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static event Action OnGameStart;
    public static event Action<int, int> OnMoveMade;
    public static event Action OnGameOver;

    [SerializeField] private Cell _cellPrefab;
    [SerializeField] private Settings _settings;
    [SerializeField] private Colors _colors;
    [SerializeField] private Camera _mainCamera;

    [SerializeField] private Transform _gridOrigin;
    [SerializeField] private Transform _cellParent;

    private Cell[,] _grid;
    private Factory<Cell> _factory;
    private int _score;
    private int _moves;
    private bool _canPlay;
    
    private void OnEnable()
    {
        Cell.OnCellClicked += HandleCellClicked;
        GameOver.OnRestart += Restart;
    }

    private void OnDisable()
    {
        Cell.OnCellClicked -= HandleCellClicked;
        GameOver.OnRestart -= Restart;
    }
    
    private void Start()
    {
        _factory = new(_cellPrefab);
        StartGame();
    }

    private void StartGame()
    {
        _score = 0;
        _canPlay = true;
        _moves = _settings.InitialMoves;
        _grid = new Cell[_settings.GridSize.y, _settings.GridSize.x];

        var originPosition = (Vector2)_gridOrigin.position;
        var cellSize = _cellPrefab.Size;
        
        // Instantiate all cells, increasing their sorting order based on row, so the top ones appear in front of the bottom ones.
        // The world position of each cell is set based on the origin, considering the grid index of the cell multiplied by its size.
        for (var i = 0; i < _settings.GridSize.y; ++i)
        {
            for (var j = 0; j < _settings.GridSize.x; ++j)
            {
                var cell = _factory.Get();
                cell.transform.SetParent(_cellParent);
                cell.transform.position = originPosition + new Vector2(j * cellSize.x, i * cellSize.y);
                cell.InitializeData(_colors.GetRandomColor(), new Vector2Int(i, j), 1 + i);
                _grid[i, j] = cell;
            }
        }

        OnGameStart?.Invoke();
    }

    private bool IsInsideGrid(Vector2Int index)
    {
        if (index.x < 0) return false;
        if (index.y < 0) return false;
        if (index.x >= _settings.GridSize.y) return false;
        if (index.y >= _settings.GridSize.x) return false;
        return true;
    }

    private void HandleCellClicked(Cell cell)
    {
        if (!_canPlay) return;

        StartCoroutine(PlayRoutine(cell));
    }

    private IEnumerator PlayRoutine(Cell cell)
    {
        _canPlay = false;
        DisableMatchingCells(cell, out var score);
        yield return new WaitForSeconds(1f);
        FillGrid();
        
        _score += score;
        _moves -= 1;
        OnMoveMade?.Invoke(_score, _moves);

        if (_moves is 0)
        {
            yield return new WaitForSeconds(1f);
            OnGameOver?.Invoke();
        }
        else _canPlay = true;
    }

    /// <summary>
    /// This method simply disables all connected cells of the same Id, storing this quantity in <paramref name="score"/>.
    /// It uses the Breadth-First Search algorithm to find connected cells.
    /// </summary>
    /// <param name="initialCell">Clicked cell.</param>
    /// <param name="score">Number of connected cells of the same color as <paramref name="initialCell"/>, including <paramref name="initialCell"/>.</param>
    private void DisableMatchingCells(Cell initialCell, out int score)
    {
        score = 0;
        var id = initialCell.Id;
        var visited = new HashSet<(int, int)>();
        var toVisit = new Queue<Cell>();
        toVisit.Enqueue(initialCell);
        var directions = new List<Vector2Int> { Vector2Int.left, Vector2Int.right, Vector2Int.up, Vector2Int.down };

        while (toVisit.Count is not 0)
        {
            var cell = toVisit.Dequeue();
            if (!visited.Add((cell.Index.x, cell.Index.y))) continue;
            cell.gameObject.SetActive(false);
            ++score;

            foreach (var direction in directions)
            {
                var neighborIndex = cell.Index + direction;
                if (!IsInsideGrid(neighborIndex) || visited.Contains((neighborIndex.x, neighborIndex.y))) continue;
                
                var neighbor = _grid[neighborIndex.x, neighborIndex.y];
                if (neighbor.Id == id)
                {
                    toVisit.Enqueue(neighbor);
                }
            }
        }
    }

    /// <summary>
    /// This method returns the ColorDefinition of the first valid cell above the cell at index <paramref name="index"/>.
    /// If there is no such cell, return a random ColorDefinition.
    /// </summary>
    /// <param name="index">Index of a cell in grid.</param>
    /// <returns>The ColorDefinition of the first valid cell above the given index or a random one if there isn't a valid cell.</returns>
    private ColorDefinition GetFallingCell(Vector2Int index)
    {
        for (var i = index.x + 1; i < _settings.GridSize.y; ++i)
        {
            var cell = _grid[i, index.y];
            if (!cell.gameObject.activeSelf) continue;
            
            var colorDefinition = cell.ColorDefinition;
            cell.gameObject.SetActive(false);
            return colorDefinition;
        }

        return _colors.GetRandomColor();
    }

    /// <summary>
    /// This method will fill the grid gaps after the move was made.
    /// It will make some cells "fall" and spawn new cells to fill the remaining gaps.
    /// </summary>
    private void FillGrid()
    {
        for (var i = 0; i < _settings.GridSize.y; ++i)
        {
            for (var j = 0; j < _settings.GridSize.x; ++j)
            {
                var cell = _grid[i, j];
                if (!cell.gameObject.activeSelf)
                {
                    cell.InitializeData(GetFallingCell(cell.Index), cell.Index);
                }
            }
        }
    }
    
    private void Restart()
    {
        for (var i = 0; i < _settings.GridSize.y; ++i)
        {
            for (var j = 0; j < _settings.GridSize.x; ++j)
            {
                _factory.Release(_grid[i, j]);
            }
        }
        
        StartGame();
    }
}
