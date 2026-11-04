using System;

namespace Refactor
{
    // 대상 후보를 context.Targets에 채우고, 유효하면 true.
    // plain 클래스 + SerializeReference로 EffectData 안에 인라인 저장된다.
    // 구체 타입에는 [Serializable] 필수.

    [Serializable]
    public abstract class Target
    {
        public abstract bool Resolve(EffectContext context);
    }
}
