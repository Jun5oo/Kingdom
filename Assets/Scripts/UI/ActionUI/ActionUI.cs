using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ActionUI : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler, IPointerExitHandler, IPoolable
{
    [SerializeField] TextMeshProUGUI actionType; 
    
    IAction action;
    public Action<IAction> OnUIClicked;

    public void OnPointerDown(PointerEventData eventData) => OnUIClicked?.Invoke(action);
    public void OnPointerEnter(PointerEventData eventData) => transform.localScale = Vector3.one * 1.1f;
    public void OnPointerExit(PointerEventData eventData) => transform.localScale = Vector3.one; 
    public void Init(IAction action)
    {
        this.action = action; 

        switch (action.ActionType)
        {
            case ActionType.Summon:
                actionType.text = "S"; 
                break;
            case ActionType.Move:
                actionType.text = "M"; 
                break;
            case ActionType.Attack:
                actionType.text = "A"; 
                break;
            case ActionType.Upgrade:
                actionType.text = "U"; 
                break;
            default:
                actionType.text = "N";
                break; 
        }
    }
}
