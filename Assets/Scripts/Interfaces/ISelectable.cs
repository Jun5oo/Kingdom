using System;
/// <summary>
/// 마우스 클릭 선택을 지원하는 오브젝트가 구현하는 인터페이스.
/// OnSelectedComplete 이벤트가 발생하면 SelectionSystem이 선택 완료를 처리한다.
/// </summary>
public interface ISelectable
{
    // 선택이 가능한 오브젝트 인터페이스
    public void OnSelected();
    public void OnDeselected();
    public bool IsSelectable();
    public BaseObject BaseObject { get; }   
    
    public event Action OnSelectedComplete;  
}
