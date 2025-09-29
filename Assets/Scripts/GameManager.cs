using System;
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
    
    private void Start()
    {
        _factory = new(_cellPrefab);
        StartGame();
    }

    private void StartGame()
    {
        _score = 0;
        _moves = _settings.InitialMoves;
        _grid = new Cell[_settings.GridSize.y, _settings.GridSize.x];

        var originPosition = (Vector2)_gridOrigin.position;
        var cellSize = _cellPrefab.Size;
        
        for (var i = 0; i < _settings.GridSize.y; ++i)
        {
            for (var j = 0; j < _settings.GridSize.x; ++j)
            {
                var cell = _factory.Get();
                cell.transform.SetParent(_cellParent);
                cell.transform.position = originPosition + new Vector2(j * cellSize.x, i * cellSize.y);
                cell.InitializeData(_colors.GetRandomColor(), 1 + i);
                _grid[i, j] = cell;
            }
        }

        OnGameStart?.Invoke();
    }

    public void OnClick_MakeMove()
    {
        _score += 10;
        _moves -= 1;
        OnMoveMade?.Invoke(_score, _moves);

        if (_moves is 0)
        {
            OnGameOver?.Invoke();
        }
    }
    
    public void OnClick_Restart()
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
