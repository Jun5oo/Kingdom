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

    public bool isMyCell;
    public bool isHighlighted;
    public bool isSelected; 

    public void Init(Vector2Int gridPosition)
    {
        this.gridPosition = gridPosition;
        isHighlighted = false; 
    }

    public Vector2Int GetGridPosition() => gridPosition;

    #region Hoverable
    public void OnHover() => hoverSprite.material = onHoverMaterial;
    public void OffHover() => hoverSprite.material = offHoverMaterial;
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

    private void OnMouseDown()
    {
        OnClicked?.Invoke(this); 
    }
    #endregion 

    public void OnSelected()
    {
        OnClicked?.Invoke(this); 
    }

    public void OnDeselected()
    {
        throw new NotImplementedException();
    }

    public bool IsSelectable()
    {
        return isSelected; 
    }

}
