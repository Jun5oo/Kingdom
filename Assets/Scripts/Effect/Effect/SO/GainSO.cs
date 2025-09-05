using Cysharp.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(menuName = ("Effect/GainSO"))]
public class GainSO : EffectSO
{
    public int value;
    public ResourceType resourceType;

    public override UniTask Apply(BaseObject caster, EffectContext context)
    {
        ActionResourceSystem actionResourceSystem = ServiceLocator.Get<ActionResourceSystem>();
        AbilityResourceSystem abilityResourceSystem = ServiceLocator.Get<AbilityResourceSystem>();

        switch (resourceType)
        {
            case ResourceType.Action:
                actionResourceSystem.Add(caster.OwnerID, value); 
                break; 
            case ResourceType.Ability:
                abilityResourceSystem.Add(caster.OwnerID, value);
                break; 
        }

        return UniTask.CompletedTask;

    }

    public override EffectType GetEffectType() => EffectType.Gain;
}
