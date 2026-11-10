namespace Refactor
{
    // 대상 후보를 context.Targets에 채우고, 유효하면 true.
    // JSON에서는 "type"으로 구체 클래스를 고른다(CardJsonLoader 등록표).
    public abstract class Target
    {
        public abstract bool Resolve(EffectContext context);
    }
}
