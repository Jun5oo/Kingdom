using System;
using System.Collections.Generic;

namespace Refactor
{
    // 한 번의 Submit 동안 상태 변경, 이벤트 기록, 트리거 해결을 맡는다.
    internal sealed class EffectProcessor
    {
        // 트리거가 서로를 끝없이 부르는 카드 조합이 서버를 멈추지 않게 한다.
        private const int MaxResolveSteps = 1000;

        private readonly GameState state;
        private readonly CardDatabase cards;
        private readonly List<GameEvent> events = new();
        // shortcut: Submit 안에서만 유지된다. 3단계 선택 대기가 생기면 GameState로 옮긴다.
        private readonly Queue<PendingTrigger> triggerQueue = new();
        // 사망은 큐가 빈 뒤 한꺼번에 처리하므로, 그때까지 마지막으로 피해를 준 플레이어를 기억한다.
        private readonly Dictionary<int, int> lastDamageSource = new();

        public EffectProcessor(GameState state, CardDatabase cards)
        {
            this.state = state ?? throw new ArgumentNullException(nameof(state));
            this.cards = cards ?? throw new ArgumentNullException(nameof(cards));
        }

        public IReadOnlyList<GameEvent> Events => events;

        public void Raise(GameEvent gameEvent) => Raise(gameEvent, null);

        // 큐를 모두 처리한 뒤 사망자를 한꺼번에 처리한다. 사망 트리거가 생기면 반복한다(§4.3, 하스스톤식).
        public void RunToCompletion()
        {
            for (int step = 0; step < MaxResolveSteps; step++)
            {
                if (triggerQueue.Count > 0)
                {
                    PendingTrigger pending = triggerQueue.Dequeue();
                    if (IsStillActive(pending))
                        Execute(pending.Effect, pending.EffectIndex, pending.SourceUnitId, pending.OwnerId, pending.TriggerEvent);
                    continue;
                }

                if (!ResolveDeaths())
                    return;
            }

            throw new InvalidOperationException("트리거 해결이 끝나지 않습니다. 카드 데이터를 확인하세요.");
        }

        // 엔진이 CanActivateEffect로 검증을 끝낸 뒤 부른다.
        public void Activate(int unitId, int effectIndex)
        {
            UnitState unit = state.Units[unitId];
            Execute(GetEffects(unit)[effectIndex], effectIndex, unitId, unit.OwnerId, null);
        }

        // 모든 피해가 지나가는 지점. 5단계에서 여신의 성역 확인을 여기에 넣는다.
        public void Damage(int unitId, int amount, int sourcePlayerId)
        {
            if (!state.Units.TryGetValue(unitId, out UnitState unit))
                return;

            unit.CurrentHP -= amount;
            lastDamageSource[unitId] = sourcePlayerId;
            Raise(new UnitDamaged(unitId, amount, unit.CurrentHP));
        }

        // 릴리스는 사망 일괄 처리를 기다리지 않고 즉시 보드에서 제거한다(§4.3).
        public void Release(int unitId, int sourcePlayerId)
        {
            if (!state.Units.TryGetValue(unitId, out UnitState unit))
                return;

            GridPos position = RemoveUnit(unitId);
            Raise(new UnitDied(unitId, unit.CardDefinitionId, unit.OwnerId, position, DeathCause.Release, sourcePlayerId), unit);
        }

        // 칸이 비어 있지 않으면 소환하지 않는다(§4.4).
        public bool TrySummon(string cardId, int level, int ownerId, GridPos position)
        {
            if (!state.Board.Contains(position.X, position.Y) || state.Board.GetUnitIdAt(position.X, position.Y).HasValue)
                return false;

            if (!cards.TryGetLevel(cardId, level, out UnitLevelData data))
                throw new InvalidOperationException($"소환할 카드 정의가 없습니다: {cardId}, {level}성");

            UnitState unit = new(state.AllocateUnitId(), cardId, ownerId, data.BaseCP, data.BaseHP, level);
            if (!state.TrySpawnUnit(unit, position.X, position.Y))
                throw new InvalidOperationException($"유닛을 소환할 수 없습니다: {cardId}");

            Raise(new UnitSummoned(unit.Id, cardId, ownerId, level, position, unit.CP, unit.CurrentHP));
            return true;
        }

        public void ChangeAbilityResource(int playerId, int delta)
        {
            PlayerState player = state.Players[playerId];
            player.AbilityResource += delta;
            Raise(new AbilityResourceChanged(playerId, player.AbilityResource));
        }

        public void ChangeActionResource(int playerId, int delta)
        {
            PlayerState player = state.Players[playerId];
            player.ActionResource += delta;
            Raise(new ActionResourceChanged(playerId, player.ActionResource));
        }

        // 이벤트를 기록하고, 반응하는 트리거를 필드에 나온 순서(유닛 ID)로 큐 뒤에 붙인다(§4.2).
        // departed는 보드를 떠난 유닛으로, 자기 자신의 사망에만 반응한다.
        private void Raise(GameEvent gameEvent, UnitState departed)
        {
            events.Add(gameEvent);

            EventTrigger trigger = gameEvent switch
            {
                UnitDied => EventTrigger.OnUnitDead,
                TurnStarted => EventTrigger.OnTurnStarted,
                _ => EventTrigger.None,
            };
            if (trigger == EventTrigger.None)
                return;

            List<UnitState> listeners = new(state.Units.Values);
            if (departed != null)
                listeners.Add(departed);
            listeners.Sort((a, b) => a.Id.CompareTo(b.Id));

            foreach (UnitState unit in listeners)
            {
                IReadOnlyList<EffectData> effects = GetEffects(unit);
                for (int i = 0; i < effects.Count; i++)
                {
                    if (effects[i].Type == EffectType.Trigger && effects[i].Trigger == trigger)
                        triggerQueue.Enqueue(new PendingTrigger(unit.Id, unit.OwnerId, effects[i], i, gameEvent));
                }
            }
        }

        // 큐에서 기다리는 동안 보드를 떠난 유닛의 트리거는 발동하지 않는다. 자기 사망에 반응하는 트리거는 예외다.
        private bool IsStillActive(PendingTrigger pending)
        {
            return state.Units.ContainsKey(pending.SourceUnitId) ||
                   (pending.TriggerEvent is UnitDied died && died.UnitId == pending.SourceUnitId);
        }

        private void Execute(EffectData effect, int effectIndex, int sourceUnitId, int ownerId, GameEvent triggerEvent)
        {
            EffectContext context = new(state, cards, this, sourceUnitId, ownerId, triggerEvent);
            if (effect.Condition != null && !effect.Condition.CanExecute(context))
                return;

            // 주인이 이미 떠났으면 다시 쓸 일이 없으므로 횟수를 세지 않는다.
            if (state.Units.TryGetValue(sourceUnitId, out UnitState source))
            {
                if (!source.CanUseEffect(effectIndex, effect.UsesPerTurn))
                    return;

                source.MarkEffectUsed(effectIndex);
            }

            foreach (Operation operation in effect.Operations)
                operation.Execute(context);
        }

        // HP가 0 이하인 유닛을 한꺼번에 제거한 뒤 나온 순서로 사망 이벤트를 낸다. 사망자가 있었으면 true.
        // 모두 제거한 뒤 이벤트를 내므로, 함께 죽은 유닛끼리는 서로의 사망에 반응하지 않는다.
        private bool ResolveDeaths()
        {
            List<UnitState> dead = new();
            foreach (UnitState unit in state.Units.Values)
            {
                if (unit.CurrentHP <= 0)
                    dead.Add(unit);
            }

            if (dead.Count == 0)
                return false;

            dead.Sort((a, b) => a.Id.CompareTo(b.Id));
            List<GridPos> positions = new(dead.Count);
            foreach (UnitState unit in dead)
                positions.Add(RemoveUnit(unit.Id));

            for (int i = 0; i < dead.Count; i++)
            {
                UnitState unit = dead[i];
                int? killer = lastDamageSource.TryGetValue(unit.Id, out int source) ? source : (int?)null;
                Raise(new UnitDied(unit.Id, unit.CardDefinitionId, unit.OwnerId, positions[i], DeathCause.Damage, killer), unit);
            }

            return true;
        }

        private GridPos RemoveUnit(int unitId)
        {
            if (!state.Board.TryGetPosition(unitId, out int x, out int y) || !state.TryRemoveUnit(unitId))
                throw new InvalidOperationException($"유닛을 보드에서 제거할 수 없습니다: {unitId}");

            return new GridPos(x, y);
        }

        private IReadOnlyList<EffectData> GetEffects(UnitState unit)
        {
            if (!cards.TryGetLevel(unit.CardDefinitionId, unit.Level, out UnitLevelData data))
                throw new InvalidOperationException($"유닛의 카드 정의가 없습니다: {unit.CardDefinitionId}, {unit.Level}성");

            return data.Effects;
        }

        private readonly struct PendingTrigger
        {
            public int SourceUnitId { get; }
            public int OwnerId { get; }
            public EffectData Effect { get; }
            public int EffectIndex { get; }
            public GameEvent TriggerEvent { get; }

            public PendingTrigger(int sourceUnitId, int ownerId, EffectData effect, int effectIndex, GameEvent triggerEvent)
            {
                SourceUnitId = sourceUnitId;
                OwnerId = ownerId;
                Effect = effect;
                EffectIndex = effectIndex;
                TriggerEvent = triggerEvent;
            }
        }
    }
}
