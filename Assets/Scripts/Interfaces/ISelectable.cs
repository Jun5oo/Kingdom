using UnityEngine;

/// <summary>
/// 선택가능한 오브젝트의 인터페이스 
/// </summary>

public interface ISelectable
{
    public void OnSelected();
    public void OnDeselected();
    public bool IsSelectable(); 
}
