using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// GridCell을 처리한느 클래스  
/// </summary>

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
    /// <summary>
    /// GridSystem에 해당 GridCell이 클릭되었음을 전달. 
    /// </summary>
    public Action<GridCell> OnClicked;
    public void OnMouseDown()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
            OnClicked?.Invoke(this);
    }
    #endregion 
}
