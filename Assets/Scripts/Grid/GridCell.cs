using System;
using UnityEngine;
using UnityEngine.UI;
public class GridCell : MonoBehaviour, IHoverable, ISelectable
{
    private Vector2Int gridPosition;

    [SerializeField] SpriteRenderer gridSprite;
    [SerializeField] SpriteRenderer hoverSprite;

    [SerializeField] private Material onHoverMaterial;
    [SerializeField] private Material offHoverMaterial;

    [SerializeField] private Material whiteMaterial;
    [SerializeField] private Material redMaterial;
    [SerializeField] private Material greenMaterial;

    public bool isSelectable = false;

    public void Init(Vector2Int gridPosition)
    {
        this.gridPosition = gridPosition;
    }

    public Vector2Int GetGridPosition() => gridPosition;
    public void OnHover() => hoverSprite.material = onHoverMaterial;
    public void OffHover() => hoverSprite.material = offHoverMaterial;

    public void OnSelected()
    {

    }
    public void OnDeselected()
    {

    }
    public bool IsSelectable()
    {
        return isSelectable;
    }

    public void HighLightValid() => gridSprite.material = greenMaterial;
    public void HighLightInvalid() => gridSprite.material = redMaterial; 
    public void UnHighLight() => gridSprite.material = whiteMaterial;

}
