public class SummonSelfDataSO : SummonDataSO
{
    // 자신의 데이터를 소환하는 경우, 다만 이런 카드효과가 있을 경우 보통 스테이스도 동일해야해서 cardData로 복사하면 안 될듯 싶다.   
    public override CardData GetCardData(BaseObject caster, EffectContext effectContext)
    {
        if (caster == null)
            return null;

        return caster.Data; 
    }
}
