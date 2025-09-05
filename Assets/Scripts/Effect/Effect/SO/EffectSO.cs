using Cysharp.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(menuName = "Effect/EffectSO")]
public abstract class EffectSO : ScriptableObject
{
    public abstract EffectType GetEffectType();
    public abstract UniTask Apply(BaseObject caster, EffectContext context); 
}
