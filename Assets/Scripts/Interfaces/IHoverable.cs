using UnityEngine;

/// <summary>
/// Hover 가능한 오브젝트의 인터페이스
/// </summary>
public interface IHoverable
{
    public void OnHover();
    public void OffHover();
    public bool IsHoverable();
}
