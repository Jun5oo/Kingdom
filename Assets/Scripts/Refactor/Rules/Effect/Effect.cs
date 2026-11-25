using System;
using System.Collections.Generic;
using MoonSharp.Interpreter;

namespace Refactor
{
    // 발동 방식
    public enum EffectType
    {
        Ignition,   // 플레이어 직접 발동
        Trigger,    // 게임 이벤트로 자동 발동
        Continuous, // 필드에 있는 동안 지속
    }

    // Trigger 타입일 때 어떤 이벤트에 반응하는지.
    public enum EventTrigger
    {
        None,
        OnTurnStarted,
        OnTurnEnded,
        OnUnitDead,
    }

    // 카드 Lua 파일에서 읽은 효과 하나. 런타임 상태(사용 횟수 등)는 UnitState가 가진다.
    public sealed class EffectData
    {
        public EffectType Type { get; }
        public EventTrigger Trigger { get; }
        // 턴당 사용 횟수. 0은 무제한.
        public int UsesPerTurn { get; }
        public int AbilityResourceCost { get; }
        public int ActionResourceCost { get; }
        // 고를 개수. 후보가 이보다 적으면 후보 수만큼 고른다.
        public int TargetCount { get; }
        // 지속 효과가 target 유닛에 더하는 CP. 다른 종류는 0이다.
        public int CP { get; }

        // Lua 함수. 조건과 대상은 없으면 null이다. 지속 효과에는 operation이 없다.
        private readonly Closure condition;
        private readonly Closure target;
        private readonly Closure operation;

        internal EffectData(EffectType type, EventTrigger trigger, int usesPerTurn,
            int abilityResourceCost, int actionResourceCost, int targetCount, int cp,
            Closure condition, Closure target, Closure operation)
        {
            Type = type;
            Trigger = trigger;
            UsesPerTurn = usesPerTurn;
            AbilityResourceCost = abilityResourceCost;
            ActionResourceCost = actionResourceCost;
            if (type != EffectType.Continuous && operation == null)
                throw new ArgumentNullException(nameof(operation));

            TargetCount = targetCount;
            CP = cp;
            this.condition = condition;
            this.target = target;
            this.operation = operation;
        }

        // 상태를 바꾸지 않는다.
        public bool CanExecute(EffectContext context)
            => condition == null || Call(condition, new LuaEffectApi(context, condition.OwnerScript, true)).CastToBool();

        public bool IsCostEnough(EffectContext context)
        {
            PlayerState owner = context.State.Players[context.OwnerId];
            return owner.AbilityResource >= AbilityResourceCost && owner.ActionResource >= ActionResourceCost;
        }

        internal void PayCost(EffectContext context)
        {
            if (AbilityResourceCost > 0)
                context.Processor.ChangeAbilityResource(context.OwnerId, -AbilityResourceCost);
            if (ActionResourceCost > 0)
                context.Processor.ChangeActionResource(context.OwnerId, -ActionResourceCost);
        }

        // 대상이 없는 효과면 null. 상태를 바꾸지 않고, 필드에 나온 순서(유닛 ID)로 정렬해 돌려준다.
        // 지속 효과에서는 고를 후보가 아니라 적용받는 유닛 목록이다.
        public List<int> GetCandidates(EffectContext context)
        {
            if (target == null)
                return null;

            DynValue result = Call(target, new LuaEffectApi(context, target.OwnerScript, true));
            if (result.Type != DataType.Table)
                throw new InvalidOperationException("카드 스크립트 오류: target은 유닛 ID 목록을 돌려줘야 합니다.");

            List<int> candidates = new();
            foreach (DynValue id in result.Table.Values)
                candidates.Add((int)id.Number);
            candidates.Sort();
            return candidates;
        }

        // 고른 대상은 context.Targets에 담겨 operation의 두 번째 인수로 넘어간다.
        internal void Execute(EffectContext context)
        {
            LuaEffectApi api = new(context, operation.OwnerScript, false);
            Call(operation, api, api.CreateTargetTable());
        }

        // 스크립트 실행 오류는 카드 데이터의 버그이므로 파일 이름과 줄 위치를 담아 던진다.
        private static DynValue Call(Closure function, params object[] args)
        {
            try
            {
                return function.Call(args);
            }
            catch (InterpreterException exception)
            {
                throw new InvalidOperationException($"카드 스크립트 오류: {exception.DecoratedMessage ?? exception.Message}", exception);
            }
        }
    }
}
