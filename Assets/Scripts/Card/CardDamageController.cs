using System;
using Unity.VisualScripting;
using UnityEngine;

public class CardDamageController : MonoBehaviour, IDamageable
{
    IUISystem uiSystem;
    Card card; 

    public Action<int> OnDamaged;

    public void Init(IUISystem uiSystem, Card card)
    {
        this.uiSystem = uiSystem;
        this.card = card; 
    }

    public void TakeDamage(int damage, bool isDirect = false)
    {
        if (card.IsKing)
        {
            if (isDirect)
                damage *= 2; 
        }
        
        else
        {
            //Temp 
            CardSystem cardSystem = FindAnyObjectByType<CardSystem>();

            int playerID = this.card.IsMyCard ? 0 : 1; 

            Card King = cardSystem.GetPlayerKing(playerID);
            King.GetComponent<CardDamageController>()?.TakeDamage(damage, false); 
        }

        OnDamaged?.Invoke(damage);

        GameObject obj = uiSystem.Pop<DamagePopupUI>();

        Vector3 worldPos = card.GetComponent<CardMovement>().PRS.position; 
        obj.transform.position = Camera.main.WorldToScreenPoint(worldPos);
        obj.GetComponent<DamagePopupUI>().SetupDamage(damage);
    }

    public bool IsAlies() => card.IsMyCard;

    public void OnDestroy()
    {
        OnDamaged = null; 
    }
}
