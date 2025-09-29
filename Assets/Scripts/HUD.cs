using TMPro;
using UnityEngine;

public class HUD : MonoBehaviour
{
    [SerializeField] private TMP_Text _scoreText;
    [SerializeField] private Color _scoreColor;
    [SerializeField] private Color _scoreValueColor;
    private const string _scoreFormat = "<color=#{0}>score</color>    <color=#{1}>{2}</color>";
    
    [SerializeField] private TMP_Text _movesText;
}
