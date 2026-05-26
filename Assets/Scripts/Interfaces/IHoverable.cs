using UnityEngine;
/// <summary>
/// 마우스 호버 감지를 지원하는 오브젝트가 구현하는 인터페이스.
/// IsHoverable()이 false를 반환하면 HoverSystem은 OnHover()를 호출하지 않는다.
/// </summary>
public interface IHoverable
{
    // Hoverable한 오브젝트 인터페이스
    public void OnHover();
    public void OffHover();
    public bool IsHoverable();
}
