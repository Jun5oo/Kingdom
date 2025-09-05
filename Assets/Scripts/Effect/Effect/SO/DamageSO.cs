using Cysharp.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(menuName = ("Effect/DamageSO"))]
public class DamageSO : EffectSO
{
    public int damage;
    public int multiplier;

    public override async UniTask Apply(BaseObject caster, EffectContext context)
    {
    }

    public override EffectType GetEffectType() => EffectType.Damage; 
}
