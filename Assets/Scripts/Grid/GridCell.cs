using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 그리드 한 칸을 나타내는 MonoBehaviour.
/// Hover(마우스 오버)와 Highlight(Action/Hover 레이어) 두 종류의 시각 상태를 독립 관리한다.
/// OnMouseDown 이벤트를 통해 클릭을 GridManager로 전달한다.
/// </summary>
public class GridCell : MonoBehaviour, IHoverable
{
    private Vector2Int gridPosition;

    [SerializeField] SpriteRenderer hoverRenderer;      // Hover 레이어 렌더러
    [SerializeField] SpriteRenderer highlightRenderer;  // Action 레이어 렌더러

    [SerializeField] private Material onHoverMaterial;
    [SerializeField] private Material offHoverMaterial;

    [SerializeField] private Material attackMaterial;
    [SerializeField] private Material moveMaterial;
    [SerializeField] private Material hoverDangerMaterial; // Hover 레이어에서 위험 표시용
    [SerializeField] private Material summonMaterial;

    private Dictionary<HighlightType, SpriteRenderer> highlightTypeToRenderer;
    private Dictionary<HighlightType, Material> highlightTypeToMaterial;
    private Dictionary<HighlightLayer, SpriteRenderer> highlightLayerToRenderer;

    private bool isHighlighted; // Action 레이어 하이라이트 여부
    private bool isHoverable;

    public Action<GridCell> OnHovered;
    public Action UnHovered;
    public Action<GridCell> OnClicked;

    /// <summary> 그리드 좌표를 설정하고 하이라이트 타입·레이어별 딕셔너리를 초기화한다. </summary>
    public void Init(Vector2Int gridPosition)
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
    /// <summary> 마우스가 올라왔을 때 hoverRenderer 머티리얼을 교체하고 OnHovered 이벤트를 발생시킨다. </summary>
    public void OnHover()
    {
        if (!isHoverable)
            return;

        OnHovered?.Invoke(this);
        hoverRenderer.material = onHoverMaterial;
    }

    /// <summary> 마우스가 벗어났을 때 hoverRenderer를 원래 머티리얼로 복원하고 UnHovered 이벤트를 발생시킨다. </summary>
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
    /// <summary>
    /// 지정 레이어의 렌더러에 타입에 맞는 머티리얼을 적용한다.
    /// HoverHighlight 타입은 isHighlighted 플래그를 변경하지 않는다.
    /// </summary>
    public void Highlight(HighlightType type, HighlightLayer layer)
    {
        SpriteRenderer sr = highlightLayerToRenderer[layer];
        Material mat = highlightTypeToMaterial[type];

        if (type != HighlightType.HoverHighlight)
            isHighlighted = true;

        sr.material = mat;
    }

    /// <summary> 지정 레이어의 렌더러를 기본 머티리얼로 초기화하고, Action 레이어면 isHighlighted를 false로 설정한다. </summary>
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
        // UI 위 클릭은 무시하여 카드/버튼 UI와 충돌 방지
        if (!EventSystem.current.IsPointerOverGameObject())
            OnClicked?.Invoke(this);
    }
}
