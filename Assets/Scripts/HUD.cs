using System.Globalization;
using TMPro;
using UnityEngine;

public class HUD : MonoBehaviour
{
    private const string ScoreFormat = "<color=#{0}>score</color>    <color=#{1}>{2}</color>";
    
    [SerializeField] private TMP_Text _scoreText;
    [SerializeField] private Color _scoreColor;
    [SerializeField] private Color _scoreValueColor;
    [SerializeField] private TMP_Text _movesText;
    [SerializeField] private Settings _settings;

    private string _scoreColorHex;
    private string _scoreValueColorHex;
    
    private void Awake()
    {
        _scoreColorHex = ColorUtility.ToHtmlStringRGB(_scoreColor);
        _scoreValueColorHex = ColorUtility.ToHtmlStringRGB(_scoreValueColor);
    }

    private void OnEnable()
    {
        GameManager.OnGameStart += HandleGameStart;
        GameManager.OnMoveMade += HandleUpdateScore;
    }

    private void OnDisable()
    {
        GameManager.OnGameStart -= HandleGameStart;
        GameManager.OnMoveMade -= HandleUpdateScore;
    }

    private void HandleUpdateScore(int newScore, int newMoves)
    {
        _movesText.SetText(GetFormattedValue(newMoves));
        _scoreText.SetText(string.Format(ScoreFormat, _scoreColorHex, _scoreValueColorHex, GetFormattedValue(newScore)));
    }
    
    private void HandleGameStart()
    {
        _movesText.SetText(GetFormattedValue(_settings.InitialMoves));
        _scoreText.SetText(string.Format(ScoreFormat, _scoreColorHex, _scoreValueColorHex, 0));
    }

    private string GetFormattedValue(int value)
    {
        return value.ToString("N0", new CultureInfo("pt-BR"));
    }
}
