using System;
using System.Collections;
using UnityEngine;

namespace Refactor
{
    [Serializable]
    public abstract class Operation
    {
        public abstract EffectCategory Category { get; }
    }

    [Serializable]
    public class DamageOperation : Operation
    {
        [SerializeReference, SubclassSelector] Amount damage;   // FlatAmount / CountScaledAmount

        public override EffectCategory Category => EffectCategory.Damage;
    }
}
