using UnityEngine;
public interface IHoverable
{
    // Hoverable한 오브젝트 인터페이스 
    public void OnHover();
    public void OffHover();
    public bool IsHoverable();
}
