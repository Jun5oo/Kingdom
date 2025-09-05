using Cysharp.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;

[CreateAssetMenu(menuName = "Effect/SummonSO")]
public class SummonSO : EffectSO 
{
    public CardData summonData;
    public bool isSourceObject;
    public int quantity;

    public override EffectType GetEffectType() => EffectType.Summon;

    public override UniTask Apply(BaseObject caster, EffectContext context)
    {
        SummonSystem summonSystem = ServiceLocator.Get<SummonSystem>(); 
        EventQueue queue = ServiceLocator.Get<EventQueue>();    

        if(summonData == null)
        {
            if (isSourceObject)
            {
                if (context.TryGet<CardData>(ContextKey.SourceData, out CardData data))
                    summonData = data;
            }

            else
            {
                Debug.Log("소환시킬 카드 데이터를 찾을 수 없습니다.");
                return UniTask.CompletedTask; 
            }
        }

        queue.Enqueue(async () =>
        {
            await summonSystem.Summon(caster.OwnerID, summonData, Vector2Int.zero, caster.Data); 
        });

        return UniTask.CompletedTask;
    }


}
