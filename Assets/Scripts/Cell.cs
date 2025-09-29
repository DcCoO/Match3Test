using UnityEngine;

public class Cell : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;

    private ColorDefinition _colorDefinition;
    
    [field: SerializeField] public Vector2 Size { get; private set; }

    public void InitializeData(ColorDefinition colorDefinition, int sortingOrder)
    {
        _spriteRenderer.sprite = colorDefinition.Sprite;
        _spriteRenderer.sortingOrder = sortingOrder;
    }

    private void OnMouseDown()
    {
        print("Cell clicked!");
    }
}
