using UnityEngine;

[CreateAssetMenu(fileName = "Colors", menuName = "Scriptable Objects/Colors")]
public class Colors : ScriptableObject
{
    [SerializeField] private ColorDefinition[] _colorList;

    public ColorDefinition GetRandomColor() => _colorList[Random.Range(0, _colorList.Length)];
}
