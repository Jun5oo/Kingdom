using Cysharp.Threading.Tasks;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ActionPopup : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler, IPointerExitHandler, IPoolable
{
    [SerializeField] Image iconImage;
    [SerializeField] TextMeshProUGUI actionInitial;  

    private IGameAction action;
    // ActionSystem에 전달하는 delegate 
    public Action<IGameAction> OnSelected;
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
    
    public void Init(IGameAction action)
    {
        this.action = action;
        this.spriteLoader = ServiceLocator.Get<SpriteLoader>(); 

        transform.localScale = Vector3.one; 

        OnUdatePopup(action);
    }

    public async UniTask OnUdatePopup(IGameAction action)
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
