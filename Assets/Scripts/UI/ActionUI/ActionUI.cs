using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ActionUI : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler, IPointerExitHandler, IPoolable
{
    // 현재는 단순히 버튼에 Text를 입혔지만, 추후에는 행동 별 UI 이미지로 대체할 예정 
    [SerializeField] TextMeshProUGUI cardActionInitial;

    private IAction action;
    public Action<IAction> OnSelected;

    public void OnPointerDown(PointerEventData eventData) => OnSelected?.Invoke(action);
    public void OnPointerEnter(PointerEventData eventData) => transform.localScale = Vector3.one * 1.1f;
    public void OnPointerExit(PointerEventData eventData) => transform.localScale = Vector3.one; 
    
    public void Init(IAction action)
    {
        this.action = action;

        transform.localScale = Vector3.one; 
        OnUpdateText(action);
    }

    public void OnUpdateText(IAction action)
    {
        switch (action.ActionType)
        {
            case ActionType.Summon:
                cardActionInitial.text = "S";
                break;
            case ActionType.Move:
                cardActionInitial.text = "M";
                break;
            case ActionType.Attack:
                cardActionInitial.text = "A";
                break;
            default:
                cardActionInitial.text = "N";
                break;
        }
    }
}
