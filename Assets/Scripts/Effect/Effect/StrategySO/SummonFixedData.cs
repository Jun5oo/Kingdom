using UnityEngine;
[CreateAssetMenu(menuName = "SummonDataSO/SummonFixedData")]
public class SummonFixedData : SummonDataSO
{
    // 등록된 카드데이터를 소환 
    [SerializeField] CardData cardData; 
    public override CardData GetCardData(BaseObject caster, EffectContext effectContext) => cardData;
}
