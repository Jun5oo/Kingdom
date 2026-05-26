using Cysharp.Threading.Tasks;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 선택된 IAction을 나타내는 팝업 버튼. IPoolable로 PoolManager에서 관리된다.
/// 클릭 시 OnSelected(action)으로 ActionSystem에 전달하고 OnClicked()로 ActionDisplayer에 닫기를 알린다.
/// </summary>
public class ActionPopup : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler, IPointerExitHandler, IPoolable
{
    [SerializeField] Image iconImage;
    [SerializeField] TextMeshProUGUI actionInitial;  

    private IAction action;
    // ActionSystem에 전달하는 delegate 
    public Action<IAction> OnSelected;
    // ActionDisplayer에 전달하는 delegate 
    public Action OnClicked;

    SpriteLoader spriteLoader; 

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
        this.spriteLoader = ServiceLocator.Get<SpriteLoader>(); 

        transform.localScale = Vector3.one; 

        OnUdatePopup(action);
    }

    public async UniTask OnUdatePopup(IAction action)
    {
        Sprite sprite = null; 

        switch (action.ActionType)
        {
            case ActionType.Summon:
                sprite = await spriteLoader.LoadSpriteAsync("icon_summon"); 
                break;
            case ActionType.Move:
                sprite = await spriteLoader.LoadSpriteAsync("icon_move"); 
                break;
            case ActionType.Attack:
                sprite = await spriteLoader.LoadSpriteAsync("icon_attack"); 
                break;
            case ActionType.Resurrection:
                sprite = await spriteLoader.LoadSpriteAsync("icon_ability"); 
                break;
            case ActionType.DivineShield:
                sprite =  await spriteLoader.LoadSpriteAsync("icon_ability"); 
                break; 
            default:
                sprite = await spriteLoader.LoadSpriteAsync("icon_upgrade"); 
                break;
        }

        this.iconImage.sprite = sprite;
    }
}
