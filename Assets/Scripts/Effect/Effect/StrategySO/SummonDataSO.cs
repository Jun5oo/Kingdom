using UnityEngine;

public abstract class SummonDataSO : ScriptableObject
{
    public abstract CardData GetCardData(BaseObject caster, EffectContext effectContext); 
}
