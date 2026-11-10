using Newtonsoft.Json;

namespace Refactor
{
    public abstract class Operation
    {
        public abstract EffectCategory Category { get; }
    }

    public class DamageOperation : Operation
    {
        [JsonProperty] public Amount Damage { get; private set; }   // Flat / CountScaled

        public override EffectCategory Category => EffectCategory.Damage;
    }
}
