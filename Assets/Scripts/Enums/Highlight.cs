/// <summary>그리드 셀 하이라이트 레이어. Action과 Hover는 독립적으로 관리된다.</summary>
public enum HighlightLayer
{
    Action = 0,
    Hover = 1,
}

/// <summary>그리드 셀 하이라이트 종류. GridCell이 이 값에 따라 색상을 결정한다.</summary>
public enum HighlightType
{
    SummonHighlight,
    MoveHighlight,
    AttackHighlight,
    HoverHighlight
}