/// <summary>카드 종류 (Unit / Spell).</summary>
public enum CardType
{
    Unit, 
    Spell 
}

/// <summary>공격/이동 범위 패턴. RangeResolver가 이 값을 Vector2Int 목록으로 변환한다.</summary>
public enum RangeType
{
    None, 
    Straight, // 상하좌우 
    Bishop,  // 대각선 
    Knight, // L자 
    Queen // 8 방향 
}