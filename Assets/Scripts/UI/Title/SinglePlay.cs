using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 싱글플레이 버튼 컴포넌트. 클릭 시 진영 선택 패널을 활성화하고 호버 시 1.1× 스케일 피드백을 준다.
/// </summary>
public class SinglePlay : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] GameObject selectionPanel; 

    public void ActivePanel()
    {
        if (selectionPanel != null) 
        {
            selectionPanel.SetActive(true);
        }
    }

    public void OnPointerEnter(PointerEventData eventData) => transform.localScale = Vector3.one * 1.1f;
    public void OnPointerExit(PointerEventData eventData) => transform.localScale = Vector3.one; 
}
