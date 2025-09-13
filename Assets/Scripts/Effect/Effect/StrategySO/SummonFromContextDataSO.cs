using UnityEngine;

[CreateAssetMenu(menuName = "SummonDataSO/SummonFromContextData")]
public class SummonFromContextDataSO : SummonDataSO
{
    // 소환할 데이터를, EffectContext로부터 가져옴. 

    // Selected, Kill, Death 
    [SerializeField] ContextKey sourceKey;
    // 해당 BaseObject를 생성한 오브젝트(ex: 무덤의 재료 객체)를 소환할 것인가에 대한 여부 
    [SerializeField] bool summonSourceData; 
    public override CardData GetCardData(BaseObject caster, EffectContext effectContext)
    {
        if(!effectContext.TryGet<ObjectContext>(sourceKey, out var objContext))
        {
            // 데이터가 없다면 
            Debug.Log($"해당 {sourceKey} 데이터를 찾을 수 없습니다.");
            return null; 
        }

        if (summonSourceData)
        {
            if(objContext.sourceData == null)
            {
                Debug.Log($"{sourceKey}의 ObjectContext에서 SourceData를 찾을 수 없습니다.");
                return null;  
            }

            return objContext.sourceData[0];
        }

        else
        {
            if(objContext.objectData == null)
            {
                Debug.Log($"{sourceKey}의 ObjectContext에서 ObjectData를 찾을 수 없습니다.");
                return null; 
            }

            return objContext.objectData; 
        }
    }
}
