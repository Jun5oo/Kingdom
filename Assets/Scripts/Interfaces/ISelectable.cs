using System;
public interface ISelectable
{
    // 선택이 가능한 오브젝트 인터페이스 
    public void OnSelected();
    public void OnDeselected();
    public bool IsSelectable();
    public BaseObject BaseObject { get; }   
    
    public event Action OnSelectedComplete;  
}
