using System;
using System.Collections.Generic;

namespace Refactor
{
    // 게임 내내 하나를 유지하며 상태 변경, 이벤트 기록, 트리거 해결을 맡는다. Submit 사이에 남는 해결 정보는 GameState에 둔다.
    internal sealed class Processor
    {
        // 트리거가 서로를 끝없이 부르는 카드 조합이 서버를 멈추지 않게 한다.
        private const int MaxResolveSteps = 1000;

        private readonly GameState state;
        private readonly CardDatabase cards;
        // 진행 중인 Submit의 이벤트. Submit이 끝날 때 TakeEvents로 꺼낸다.
        private List<GameEvent> events = new();
        // Submit이 시작될 때의 유닛별 CP. 오라처럼 이벤트 없이 바뀐 CP를 TakeEvents에서 찾아 알린다.
        private Dictionary<int, int> cpAtSubmitStart = new();

        public Processor(GameState state, CardDatabase cards)
        {
            this.state = state ?? throw new ArgumentNullException(nameof(state));
            this.cards = cards ?? throw new ArgumentNullException(nameof(cards));
        }

        // 상태를 바꾸기 전에 부른다. 검증에 실패한 Submit의 기록은 다음 BeginSubmit이 덮어쓴다.
        public void BeginSubmit()
        {
            cpAtSubmitStart = new Dictionary<int, int>();
            foreach (int unitId in state.Units.Keys)
                cpAtSubmitStart[unitId] = GetCP(unitId);
        }

        // 엔진이 CanActivateEffect로 검증을 끝낸 뒤 부른다.
        public void Activate(int unitId, int effectIndex)
        {
            UnitState unit = state.Units[unitId];
            Execute(new PendingEffect(unitId, unit.CardDefinitionId, unit.OwnerId, GetEffects(unit)[effectIndex], effectIndex, null));
        }

        // 엔진이 CanChoose로 검증을 끝낸 뒤 부른다. 멈췄던 효과를 Operation부터 이어서 실행한다.
        public void Choose(IReadOnlyList<int> unitIds)
        {
            PendingEffect pending = state.ChoosingEffect.Value;
            state.ChoosingEffect = null;
            state.PendingChoice = null;

            List<TargetUnit> targets = new(unitIds.Count);
            foreach (int unitId in unitIds)
            {
                state.Board.TryGetPosition(unitId, out int x, out int y);
                targets.Add(new TargetUnit(unitId, new GridPos(x, y), state.Units[unitId].OriginCardId));
            }

            EffectContext context = CreateContext(pending);
            context.Targets = targets;
            pending.Effect.Execute(context);
        }

        // 큐를 모두 처리한 뒤 사망자를 한꺼번에 처리한다. 사망 트리거가 생기면 반복한다(§4.3, 하스스톤식).
        // 선택 대기가 생기면 멈추고, 선택 응답을 받은 Submit에서 다시 부른다.
        public void RunToCompletion()
        {
            for (int step = 0; step < MaxResolveSteps; step++)
            {
                if (state.PendingChoice != null)
                    return;

                if (state.TriggerQueue.Count > 0)
                {
                    PendingEffect pending = state.TriggerQueue.Dequeue();
                    if (IsStillActive(pending))
                        Execute(pending);
                    continue;
                }

                if (!ResolveDeaths())
                {
                    state.LastDamageSource.Clear();
                    state.PendingDestroy.Clear();
                    return;
                }
            }

            throw new InvalidOperationException("트리거 해결이 끝나지 않습니다. 카드 데이터를 확인하세요.");
        }

        // 꺼낸 목록은 결과가 가져가므로 새 목록으로 바꿔, 다음 Submit이 이전 결과를 건드리지 않게 한다.
        public IReadOnlyList<GameEvent> TakeEvents()
        {
            RaiseCPChanges();
            List<GameEvent> taken = events;
            events = new List<GameEvent>();
            return taken;
        }

        public void Raise(GameEvent gameEvent) => Raise(gameEvent, null);

        // 상태 이상과 지속 효과의 수치를 모두 더한 뒤 0 미만이면 0으로 한다. 합을 먼저 내므로 붙은 순서와 결과가 무관하다.
        // shortcut: 조회할 때마다 보드의 지속 효과를 모두 Lua로 확인한다. 유닛이나 지속 효과가 많아져 느려지면 Submit 단위로 캐시한다.
        public int GetCP(int unitId)
        {
            UnitState unit = state.Units[unitId];
            int cp = unit.BaseCP;
            foreach (Status status in unit.Statuses)
                cp += status.BonusCP;

            foreach (UnitState source in state.Units.Values)
            {
                // 사망 처리를 기다리는 유닛의 지속 효과는 이미 꺼진 것으로 본다.
                if (state.IsDying(source))
                    continue;

                foreach (EffectData effect in GetEffects(source))
                {
                    if (effect.Type != EffectType.Continuous)
                        continue;

                    EffectContext context = new(state, cards, null, source.Id, source.CardDefinitionId, source.OwnerId, null);
                    if (effect.CanExecute(context) && effect.GetCandidates(context).Contains(unitId))
                        cp += effect.CP;
                }
            }

            return Math.Max(0, cp);
        }

        // 모든 피해가 지나가는 지점.
        public void Damage(int unitId, int amount, int sourcePlayerId)
        {
            if (!state.Units.TryGetValue(unitId, out UnitState unit))
                return;

            amount = PreventDamage(unit, amount);
            unit.CurrentHP -= amount;
            state.LastDamageSource[unitId] = sourcePlayerId;
            Raise(new UnitDamaged(unitId, amount, unit.CurrentHP));
        }

        // 즉시 제거하지 않고 사망 일괄 처리에서 원인 Destroy로 처리한다. 피해가 아니므로 보호막으로 막을 수 없다.
        public void Destroy(int unitId, int sourcePlayerId)
        {
            if (state.Units.ContainsKey(unitId))
                state.PendingDestroy[unitId] = sourcePlayerId;
        }

        // 같은 유닛에 여러 개가 쌓인다(§5.3).
        public void AddStatus(int unitId, StatusType? type, int bonusCP, int? turns, int ownerId, int sourceUnitId, string sourceCardId, EffectData onExpire)
        {
            if (!state.Units.TryGetValue(unitId, out UnitState unit))
                return;

            Status status = new(state.AllocateStatusId(), type, turns, bonusCP, ownerId, sourceUnitId, sourceCardId, onExpire);
            unit.AddStatus(status);
            Raise(new StatusAdded(unitId, status.Id, type, turns, sourceCardId));
        }

        // 턴이 시작되는 플레이어 유닛의 상태 이상만 줄인다. 만료 효과는 나온 순서(유닛 ID)로 큐 뒤에 붙는다.
        public void TickStatuses(int playerId)
        {
            List<UnitState> units = new();
            foreach (UnitState unit in state.Units.Values)
            {
                if (unit.OwnerId == playerId)
                    units.Add(unit);
            }
            units.Sort((a, b) => a.Id.CompareTo(b.Id));

            foreach (UnitState unit in units)
            {
                foreach (Status status in new List<Status>(unit.Statuses))
                {
                    if (!status.RemainingTurns.HasValue)
                        continue;

                    status.RemainingTurns -= 1;
                    if (status.RemainingTurns > 0)
                        continue;

                    RemoveStatus(unit, status);
                    if (status.OnExpire != null)
                        state.TriggerQueue.Enqueue(new PendingEffect(unit.Id, status.SourceCardId, status.OwnerId, status.OnExpire, -1, null));
                }
            }
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
        public bool TrySummon(string cardId, int ownerId, GridPos position, string originCardId = null)
        {
            if (!state.Board.Contains(position.X, position.Y) || state.Board.GetUnitIdAt(position.X, position.Y).HasValue)
                return false;

            if (!cards.TryGetCard(cardId, out CardDefinition card))
                throw new InvalidOperationException($"소환할 카드 정의가 없습니다: {cardId}");

            UnitState unit = new(state.AllocateUnitId(), cardId, ownerId, card.BaseCP, card.BaseHP, originCardId);
            if (!state.TrySpawnUnit(unit, position.X, position.Y))
                throw new InvalidOperationException($"유닛을 소환할 수 없습니다: {cardId}");

            Raise(new UnitSummoned(unit.Id, cardId, ownerId, position, GetCP(unit.Id), unit.CurrentHP));
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
                        state.TriggerQueue.Enqueue(new PendingEffect(unit.Id, unit.CardDefinitionId, unit.OwnerId, effects[i], i, gameEvent));
                }
            }
        }

        // Submit 시작 때(그 사이 소환된 유닛은 소환 이벤트의 값)와 지금 CP가 다른 유닛을 나온 순서로 알린다. 중간 값은 알리지 않는다.
        private void RaiseCPChanges()
        {
            Dictionary<int, int> reported = new(cpAtSubmitStart);
            foreach (GameEvent gameEvent in events)
            {
                if (gameEvent is UnitSummoned summoned)
                    reported[summoned.UnitId] = summoned.CP;
            }

            List<int> unitIds = new(state.Units.Keys);
            unitIds.Sort();
            foreach (int unitId in unitIds)
            {
                int cp = GetCP(unitId);
                if (reported.TryGetValue(unitId, out int before) && before != cp)
                    Raise(new UnitCPChanged(unitId, cp));
            }

            cpAtSubmitStart = new Dictionary<int, int>();
        }

        // 조건, 횟수, 비용을 확인한 뒤 지불한다. 고를 대상이 있으면 선택 대기로 멈춘다.
        private void Execute(PendingEffect pending)
        {
            EffectData effect = pending.Effect;
            EffectContext context = CreateContext(pending);
            if (!effect.CanExecute(context))
                return;

            if (!effect.IsCostEnough(context))
                return;

            // 주인이 이미 떠났으면 다시 쓸 일이 없으므로 횟수를 세지 않는다. 상태 이상의 만료 효과는 횟수 제한이 없다.
            if (effect.UsesPerTurn > 0 && state.Units.TryGetValue(pending.SourceUnitId, out UnitState source))
            {
                if (!source.CanUseEffect(pending.EffectIndex, effect.UsesPerTurn))
                    return;

                source.MarkEffectUsed(pending.EffectIndex);
            }

            effect.PayCost(context);

            // 후보가 없으면 빈 대상으로 operation을 실행한다. 대상을 도는 동작은 자연히 건너뛴다(§4.4).
            List<int> candidates = effect.GetCandidates(context);
            if (candidates != null && candidates.Count > 0)
            {
                state.ChoosingEffect = pending;
                state.PendingChoice = new PendingChoice(pending.OwnerId, candidates, Math.Min(effect.TargetCount, candidates.Count));
                Raise(new ChoiceRequested(state.PendingChoice));
                return;
            }

            effect.Execute(context);
        }

        // 피해를 막거나 줄이는 키워드. 적용 순서가 결과를 바꾸므로 순서를 여기서 고정한다(§5.3).
        private int PreventDamage(UnitState unit, int amount)
        {
            // 겹쳐 걸린 보호막도 1회에 모두 사라진다. 피해가 0이면 소모되지 않는다.
            if (amount > 0 && unit.HasStatus(StatusType.Shield))
            {
                foreach (Status status in new List<Status>(unit.Statuses))
                {
                    if (status.Type == StatusType.Shield)
                        RemoveStatus(unit, status);
                }
                return 0;
            }

            return amount;
        }

        // 만료, 보호막 소모 등 모든 제거가 여기를 지난다. 만료 효과는 실행하지 않는다.
        private void RemoveStatus(UnitState unit, Status status)
        {
            unit.RemoveStatus(status);
            Raise(new StatusRemoved(unit.Id, status.Id));
        }

        // 사망 대기 유닛(HP 0 이하, 파괴 예약)을 한꺼번에 제거한 뒤 나온 순서로 사망 이벤트를 낸다. 사망자가 있었으면 true.
        // 모두 제거한 뒤 이벤트를 내므로, 함께 죽은 유닛끼리는 서로의 사망에 반응하지 않는다.
        private bool ResolveDeaths()
        {
            List<UnitState> dead = new();
            foreach (UnitState unit in state.Units.Values)
            {
                if (state.IsDying(unit))
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
                // 파괴와 치명 피해를 함께 받았으면 파괴로 본다.
                if (state.PendingDestroy.TryGetValue(unit.Id, out int destroyer))
                {
                    Raise(new UnitDied(unit.Id, unit.CardDefinitionId, unit.OwnerId, positions[i], DeathCause.Destroy, destroyer), unit);
                    continue;
                }

                int? killer = state.LastDamageSource.TryGetValue(unit.Id, out int source) ? source : (int?)null;
                Raise(new UnitDied(unit.Id, unit.CardDefinitionId, unit.OwnerId, positions[i], DeathCause.Damage, killer), unit);
            }

            return true;
        }

        // 큐에서 기다리는 동안 보드를 떠난 유닛의 트리거는 발동하지 않는다. 자기 사망에 반응하는 트리거는 예외다.
        private bool IsStillActive(PendingEffect pending)
        {
            return state.Units.ContainsKey(pending.SourceUnitId) ||
                   (pending.TriggerEvent is UnitDied died && died.UnitId == pending.SourceUnitId);
        }

        private EffectContext CreateContext(PendingEffect pending)
            => new EffectContext(state, cards, this, pending.SourceUnitId, pending.SourceCardId, pending.OwnerId, pending.TriggerEvent);

        private GridPos RemoveUnit(int unitId)
        {
            if (!state.Board.TryGetPosition(unitId, out int x, out int y) || !state.TryRemoveUnit(unitId))
                throw new InvalidOperationException($"유닛을 보드에서 제거할 수 없습니다: {unitId}");

            return new GridPos(x, y);
        }

        private IReadOnlyList<EffectData> GetEffects(UnitState unit)
        {
            if (!cards.TryGetCard(unit.CardDefinitionId, out CardDefinition card))
                throw new InvalidOperationException($"유닛의 카드 정의가 없습니다: {unit.CardDefinitionId}");

            return card.Effects;
        }
    }
}
