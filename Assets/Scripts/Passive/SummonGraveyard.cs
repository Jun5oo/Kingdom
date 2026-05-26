using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 언데드 왕의 패시브. 아군 유닛이 처치될 때 해당 위치에 무덤 토큰을 소환한다.
/// Graveyard 태그 유닛이 처치되는 경우는 무시한다(무한 소환 방지).
/// 소환은 EventQueue에 등록되어 현재 전투 해결 이후 처리된다.
/// </summary>
public class SummonGraveyard : IPassive
{
    BaseObject passiveOwner;

    TokenManager tokenManager;
    GridManager gridManager;
    DamageManager damageManager;
    SummonSystem summonSystem;

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

    /// <summary>
    /// 처치된 유닛이 이 패시브 소유자의 아군이고 Graveyard가 아닐 때,
    /// 해당 위치에 무덤 카드를 EventQueue를 통해 소환한다.
    /// </summary>
    void Summon(int playerID, Token deadToken)
    {
        if (playerID != passiveOwner.OwnerID)
            return;

        if (deadToken.Data.Tag == UnitTag.Graveyard)
            return;

        Vector2Int position = tokenManager.GetGridPositionOfToken(deadToken);

        CardDatabase database = ServiceLocator.Get<CardDatabase>();
        CardData graveyard = database.GetCardData<CardData>("무덤");
        EventQueue eventQueue = ServiceLocator.Get<EventQueue>();

        eventQueue.Enqueue(async () =>
        {
            await summonSystem.Summon(deadToken.OwnerID, graveyard, position, passiveOwner.Data, new List<CardData> { deadToken.Data });
        });
    }
}
