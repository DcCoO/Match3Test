using System;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public static event Action<Cell> OnCellClicked;
    
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [field: SerializeField] public Vector2 Size { get; private set; }

    public ColorDefinition ColorDefinition { get; private set; }
    public Vector2Int Index { get; private set; }
    public int Id => ColorDefinition.Id;
    
    public void InitializeData(ColorDefinition colorDefinition, Vector2Int index, int sortingOrder = 0)
    { 
        ColorDefinition = colorDefinition;
        _spriteRenderer.sprite = colorDefinition.Sprite;
        if (sortingOrder is not 0) _spriteRenderer.sortingOrder = sortingOrder;
        Index = index;
        gameObject.SetActive(true);
    }

    private void OnMouseDown()
    {
        OnCellClicked?.Invoke(this);
    }
}
