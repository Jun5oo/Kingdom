using System;
using System.Collections.Generic;
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
    [SerializeField] SpriteRenderer highlightSprite; 

    [SerializeField] private Material onHoverMaterial;
    [SerializeField] private Material offHoverMaterial;

    [SerializeField] private Material whiteMaterial;
    [SerializeField] private Material redMaterial;
    [SerializeField] private Material blueMaterial;
    [SerializeField] private Material orangeMaterial; 
    [SerializeField] private Material greenMaterial;


    private Dictionary<HighlightType, SpriteRenderer> typeRendererDictionary;
    private Dictionary<HighlightType, Material> typeMaterialDictionary;
    private Dictionary<HighlightLayer, SpriteRenderer> layerRendererDictionary; 

    public bool isMyCell;
    public bool isHighlighted;
    public bool isSelected;

    public bool isHoverable = true;

    public event Action<GridCell> OnCellHovered;
    public event Action OnCellUnhovered; 

    public void Init(Vector2Int gridPosition)
    {
        this.gridPosition = gridPosition;
        isHoverable = true; 
        isHighlighted = false; 
        
        typeRendererDictionary = new Dictionary<HighlightType, SpriteRenderer>();

        typeRendererDictionary[HighlightType.ValidSummon] = gridSprite; 
        typeRendererDictionary[HighlightType.ValidMove] = highlightSprite;
        typeRendererDictionary[HighlightType.ValidAttack] = highlightSprite;
        typeRendererDictionary[HighlightType.EnemyAttackRange] = hoverSprite;

        typeMaterialDictionary = new Dictionary<HighlightType, Material>();
        typeMaterialDictionary[HighlightType.ValidSummon] = greenMaterial;
        typeMaterialDictionary[HighlightType.ValidMove] = blueMaterial;
        typeMaterialDictionary[HighlightType.ValidAttack] = redMaterial;
        typeMaterialDictionary[HighlightType.EnemyAttackRange] = orangeMaterial; 
        
        layerRendererDictionary = new Dictionary<HighlightLayer, SpriteRenderer>();
        layerRendererDictionary[HighlightLayer.Action] = highlightSprite;
        layerRendererDictionary[HighlightLayer.Hover] = hoverSprite;
        layerRendererDictionary[HighlightLayer.Outline] = gridSprite; 
    }

    public Vector2Int GetGridPosition() => gridPosition;

    #region Hoverable
    public void OnHover()
    {
        if (!isHoverable)
            return;

        OnCellHovered?.Invoke(this); 
        hoverSprite.material = onHoverMaterial;
    }
    public void OffHover()
    {
        if (!isHoverable)
            return;

        OnCellUnhovered?.Invoke();
        hoverSprite.material = offHoverMaterial;
    }
    public bool IsHoverable() => isHoverable; 
    #endregion

    #region Highlight
    public void Highlight(HighlightType type, HighlightLayer layer)
    {
        SpriteRenderer sr = layerRendererDictionary[layer]; 
        Material mat = typeMaterialDictionary[type];

        switch (type)
        {
            case HighlightType.ValidSummon:
                sr.material = mat;
                isHighlighted = true;
                break;
            case HighlightType.ValidMove:
                sr.material = mat;
                isHighlighted = true;
                break;
            case HighlightType.ValidAttack:
                sr.material = mat;
                isHighlighted = true;
                break;
            case HighlightType.EnemyAttackRange:
                sr.material = mat;
                break; 
        }
    }
    public void Unhighlight(HighlightLayer layer)
    {
        SpriteRenderer sr = layerRendererDictionary[layer];

        switch (layer)
        {
            case HighlightLayer.Action:
                sr.material = offHoverMaterial;
                isHighlighted = false; 
                break; 
            case HighlightLayer.Hover:
                sr.material = offHoverMaterial;
                break;
            case HighlightLayer.Outline:
                isHighlighted = false; 
                sr.material = whiteMaterial;
                break; 
        }
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
