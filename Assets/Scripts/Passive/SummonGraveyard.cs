using System.Collections.Generic;
using UnityEngine;

public class SummonGraveyard : IPassive
{
    // 언데드왕의 패시브, 아군이 처치되었을 때 발동

    BaseObject passiveOwner; 

    TokenManager tokenManager;
    GridManager gridManager; 
    DamageManager damageManager;
    SummonSystem summonSystem; 

    // cardID : GRAVE_YARD; 

    public SummonGraveyard(BaseObject owner) 
    {
        tokenManager = ServiceLocator.Get<TokenManager>(); 
        gridManager = ServiceLocator.Get<GridManager>();    
        damageManager = ServiceLocator.Get<DamageManager>();
        summonSystem = ServiceLocator.Get<SummonSystem>();  

        this.passiveOwner = owner; 
    }

    public void Activate()
    {
        Debug.Log($"{this} 연결되었습니다."); 
        damageManager.OnPlayerUnitDead += Summon; 
    }

    public void Deactivate()
    {
        damageManager.OnPlayerUnitDead -= Summon; 
    }

    public void Summon(int playerID, Token deadToken)
    {
        if (playerID != passiveOwner.OwnerID)
            return; 

        Vector2Int position = tokenManager.GetGridPositionOfToken(deadToken);
        Vector3 worldPos = gridManager.GetWorldPosition(position);

        CardDatabase database = ServiceLocator.Get<CardDatabase>();

        var unitData = deadToken.Data;

        if (unitData.Tag == UnitTag.Graveyard)
            return; 

        CardData graveyard = database.GetCardData<CardData>("무덤");
        EventQueue eventQueue = ServiceLocator.Get<EventQueue>();

        eventQueue.Enqueue(async () =>
        {
            await summonSystem.Summon(deadToken.OwnerID, graveyard, position, passiveOwner.Data, new List<CardData> { deadToken.Data }); 
        }); 
    }
}
