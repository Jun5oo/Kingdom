using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;

public interface IEffect
{
    public EffectType EffectType { get; }
    public EffectData EffectData { get; }
    public Trigger Trigger { get; }

    public List<Func<UniTask>> ToEvents(BaseObject owner, EffectContext context); 
}
