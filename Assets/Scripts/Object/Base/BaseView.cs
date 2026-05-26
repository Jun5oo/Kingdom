using UnityEngine;

/// <summary>
/// Card/Token 시각 표현의 기반 클래스.
/// Anchor는 팝업(데미지, 행동 포인트 등) 표시를 위한 부착 위치를 제공한다.
/// </summary>
public abstract class BaseView : MonoBehaviour
{
    public abstract Transform Anchor { get; }
}
