using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ActionPopup : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler, IPointerExitHandler, IPoolable
{
    // 현재는 단순히 버튼에 Text를 입혔지만, 추후에는 행동 별 UI 이미지로 대체할 예정 
    [SerializeField] TextMeshProUGUI actionInitial;

    private IAction action;
    // ActionSystem에 전달하는 delegate 
    public Action<IAction> OnSelected;
    // ActionDisplayer에 전달하는 delegate 
    public Action OnClicked; 

    public void OnPointerDown(PointerEventData eventData)
    {
        OnSelected?.Invoke(action);
        OnClicked?.Invoke();
    }
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
                actionInitial.text = "S";
                break;
            case ActionType.Move:
                actionInitial.text = "M";
                break;
            case ActionType.Attack:
                actionInitial.text = "A";
                break;
            case ActionType.Resurrection:
                actionInitial.text = "R";
                break;
            case ActionType.DivineShield:
                actionInitial.text = "D";
                break; 
            default:
                actionInitial.text = "N";
                break;
        }
    }
}
