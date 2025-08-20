using Cysharp.Threading.Tasks;
using UnityEngine;

public class SummonEffect : IEffect
{
    EffectData data;
    BaseObject effectOwner; 

    public SummonEffect(EffectData data, BaseObject baseObject)
    {
        this.data = data;
        this.effectOwner = baseObject; 
    }

    public async UniTask ExecuteEffect(Vector2Int targetPosition)
    {
        CardDatabase database = ServiceLocator.Get<CardDatabase>(); 
        SummonSystem summonSystem = ServiceLocator.Get<SummonSystem>();
        // 데이터베이스에서 Summon할 카드의 데이터를 찾고 cardData를 받은 후에 

        CardData summonData = database.GetCardData<CardData>(data.parameter1);
        await summonSystem.Summon(effectOwner.OwnerID, summonData, targetPosition, effectOwner.Data); 
    }
}
