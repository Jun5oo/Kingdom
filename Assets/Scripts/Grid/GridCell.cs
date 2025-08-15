using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GridCell : MonoBehaviour, IHoverable 
{
    private Vector2Int gridPosition;

    // Hover
    [SerializeField] SpriteRenderer hoverRenderer;
    // Highlight 
    [SerializeField] SpriteRenderer highlightRenderer;
    // Grid 
    [SerializeField] private Material onHoverMaterial;
    [SerializeField] private Material offHoverMaterial;

    [SerializeField] private Material attackMaterial;
    [SerializeField] private Material moveMaterial;
    [SerializeField] private Material hoverDangerMaterial; 
    [SerializeField] private Material summonMaterial;


    private Dictionary<HighlightType, SpriteRenderer> highlightTypeToRenderer;
    private Dictionary<HighlightType, Material> highlightTypeToMaterial;
    private Dictionary<HighlightLayer, SpriteRenderer> highlightLayerToRenderer; 

    private bool isHighlighted;
    private bool isHoverable;

    public Action<GridCell> OnHovered;
    public Action UnHovered;
    public Action<GridCell> OnClicked;

    public void Init(Vector2Int gridPosition, Sprite sprite)
    {
        this.gridPosition = gridPosition;
        
        isHoverable = true; 
        isHighlighted = false; 
        
        highlightTypeToRenderer = new Dictionary<HighlightType, SpriteRenderer>();
        highlightTypeToMaterial = new Dictionary<HighlightType, Material>();
        highlightLayerToRenderer = new Dictionary<HighlightLayer, SpriteRenderer>();

        highlightTypeToRenderer[HighlightType.SummonHighlight] = highlightRenderer; 
        highlightTypeToRenderer[HighlightType.MoveHighlight] = highlightRenderer;
        highlightTypeToRenderer[HighlightType.AttackHighlight] = highlightRenderer;
        highlightTypeToRenderer[HighlightType.HoverHighlight] = hoverRenderer;

        highlightTypeToMaterial[HighlightType.SummonHighlight] = summonMaterial;
        highlightTypeToMaterial[HighlightType.MoveHighlight] = moveMaterial;
        highlightTypeToMaterial[HighlightType.AttackHighlight] = attackMaterial;
        highlightTypeToMaterial[HighlightType.HoverHighlight] = hoverDangerMaterial; 
        
        highlightLayerToRenderer[HighlightLayer.Action] = highlightRenderer;
        highlightLayerToRenderer[HighlightLayer.Hover] = hoverRenderer;
    }

    public Vector2Int GetGridPosition() => gridPosition;

    #region Hoverable
    public void OnHover()
    {
        if (!isHoverable)
            return;

        OnHovered?.Invoke(this); 
        hoverRenderer.material = onHoverMaterial;
    }
    public void OffHover()
    {
        if (!isHoverable)
            return;

        UnHovered?.Invoke();
        hoverRenderer.material = offHoverMaterial;
    }
    public bool IsHoverable() => isHoverable; 
    #endregion
    #region Highlight
    public void Highlight(HighlightType type, HighlightLayer layer)
    {
        SpriteRenderer sr = highlightLayerToRenderer[layer]; 
        Material mat = highlightTypeToMaterial[type];

        if (type != HighlightType.HoverHighlight)
            isHighlighted = true;

        sr.material = mat; 
    }
    public void Unhighlight(HighlightLayer layer)
    {
        SpriteRenderer sr = highlightLayerToRenderer[layer];

        if (layer == HighlightLayer.Action)
            isHighlighted = false;

        sr.material = offHoverMaterial; 
    }
    public bool IsHighlighted() => isHighlighted;
    #endregion

    void OnMouseDown()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
            OnClicked?.Invoke(this);
    }
}
