using JetBrains.Annotations;
using System;
using UnityEngine;
using UnityEngine.UI;
public class GridCell : MonoBehaviour, IHoverable 
{
    private Vector2Int gridPosition;

    [SerializeField] SpriteRenderer gridSprite;
    [SerializeField] SpriteRenderer hoverSprite;

    [SerializeField] private Material onHoverMaterial;
    [SerializeField] private Material offHoverMaterial;

    [SerializeField] private Material whiteMaterial;
    [SerializeField] private Material redMaterial;
    [SerializeField] private Material greenMaterial;

    public bool isMyCell;
    public bool isHighlighted;
    public bool isSelected;

    public bool isHoverable = true; 

    public void Init(Vector2Int gridPosition)
    {
        this.gridPosition = gridPosition;
        isHighlighted = false; 
    }

    public Vector2Int GetGridPosition() => gridPosition;

    #region Hoverable
    public void OnHover()
    {
        if (!isHoverable)
            return; 

        hoverSprite.material = onHoverMaterial;
    }
    public void OffHover()
    {
        if (!isHoverable)
            return; 
        hoverSprite.material = offHoverMaterial;
    }
    public bool IsHoverable() => isHoverable; 
    #endregion

    #region Highlight
    public void Highlight()
    {
        gridSprite.material = greenMaterial;
        isHighlighted = true; 
    }
    public void Unhighlight()
    {
        gridSprite.material = whiteMaterial;
        isHighlighted = false;
    }
    #endregion

    #region Action 
    public Action<GridCell> OnClicked;
    public void OnMouseDown()
    {
        OnClicked?.Invoke(this); 
    }
    #endregion 
}
