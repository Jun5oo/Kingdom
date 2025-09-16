using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Effect/SummonSO")]
public class SummonSO : EffectSO 
{
    [SerializeField] SummonDataSO dataSO; 

    public override EffectType GetEffectType() => EffectType.Summon;

    public override async UniTask Apply(BaseObject caster, TriggeredEffect binding, EffectContext context)
    {
        SummonSystem summonSystem = ServiceLocator.Get<SummonSystem>(); 

        CardData cardData = dataSO.GetCardData(caster, context);

        if(cardData == null)
        {
            Debug.Log($"카드 데이터를 찾을 수 없습니다.");
            return;
        }

        if (!context.TryGet<List<Vector2Int>>(ContextKey.Position, out List<Vector2Int> positions))
        {
            Debug.Log("해당 ContextKey: Positions를 찾을 수 없습니다");
            return;
        }

        await summonSystem.Summon(caster.OwnerID, cardData, positions[0], caster.Data);
    }


}
