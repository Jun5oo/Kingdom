using System;
using MoonSharp.Interpreter;

namespace Refactor
{
    // 카드 Lua 스크립트의 ctx. 여기 있는 public 멤버가 스크립트가 할 수 있는 일의 전부다.
    // Lua에서는 첫 글자를 소문자로 불러도 된다(ctx:release, ctx.ownerId).
    public sealed class LuaEffectApi
    {
        private readonly EffectContext context;
        // 돌려줄 Lua 테이블을 만들 때 쓴다.
        private readonly Script script;
        // 조건과 대상 함수는 상태를 바꾸면 안 된다.
        private readonly bool readOnly;

        // 등록한 타입만 Lua로 넘길 수 있다. 첫 인스턴스를 만들기 전에 한 번 실행된다.
        static LuaEffectApi() => UserData.RegisterType<LuaEffectApi>();

        internal LuaEffectApi(EffectContext context, Script script, bool readOnly)
        {
            this.context = context;
            this.script = script;
            this.readOnly = readOnly;
        }

        public int OwnerId => context.OwnerId;
        public int SourceUnitId => context.SourceUnitId;

        private Processor Processor => readOnly
            ? throw new InvalidOperationException("카드 스크립트 오류: condition과 target에서는 상태를 바꿀 수 없습니다.")
            : context.Processor;

        // ── 조회 ──

        // 발동 이벤트가 유닛 사망이면 { unitId, cardId, ownerId, x, y, killerId }, 아니면 nil. 죽인 쪽을 모르면 killerId는 nil이다.
        public Table DeadUnit()
        {
            if (context.TriggerEvent is not UnitDied died)
                return null;

            Table result = new(script);
            result["unitId"] = died.UnitId;
            result["cardId"] = died.CardId;
            result["ownerId"] = died.OwnerId;
            result["x"] = died.Position.X;
            result["y"] = died.Position.Y;
            result["killerId"] = died.KillerPlayerId;
            return result;
        }

        // 카드의 분류 이름("Hero" 등). 모르는 카드면 nil.
        public string TagOf(string cardId)
            => context.Cards.TryGetCard(cardId, out CardDefinition card) ? card.Tag.ToString() : null;

        // 보드 위에서 주인("Self", "Opponent", "Any")과 분류가 맞는 유닛 ID 목록. tag가 nil이면 분류를 보지 않는다.
        public Table FindUnits(string owner, string tag)
        {
            if (!Enum.TryParse(owner, out PlayerRelation relation))
                throw new ArgumentException($"카드 스크립트 오류: 알 수 없는 주인 '{owner}'");

            UnitTag? unitTag = null;
            if (tag != null)
            {
                if (!Enum.TryParse(tag, out UnitTag parsed))
                    throw new ArgumentException($"카드 스크립트 오류: 알 수 없는 분류 '{tag}'");
                unitTag = parsed;
            }

            Table result = new(script);
            foreach (UnitState unit in context.State.Units.Values)
            {
                // HP가 0 이하인 유닛은 사망 처리를 기다리는 중이므로 고를 수 없다.
                if (unit.CurrentHP <= 0 || !relation.Matches(unit.OwnerId, context.OwnerId))
                    continue;

                if (!unitTag.HasValue ||
                    (context.Cards.TryGetCard(unit.CardDefinitionId, out CardDefinition card) && card.Tag == unitTag.Value))
                    result.Append(DynValue.NewNumber(unit.Id));
            }

            return result;
        }

        // ── 상태 변경 ──

        // 릴리스는 사망 일괄 처리를 기다리지 않고 즉시 보드에서 제거한다(§4.3).
        public void Release(int unitId) => Processor.Release(unitId, context.OwnerId);

        public void ChangeAbilityResource(int amount) => Processor.ChangeAbilityResource(context.OwnerId, amount);

        // 효과 주인의 유닛으로 소환한다. 칸이 차 있으면 소환하지 않고 false. originCardId는 소환된 유닛이 기억할 원래 카드다.
        public bool Summon(string cardId, int x, int y, string originCardId = null)
            => Processor.TrySummon(cardId, context.OwnerId, new GridPos(x, y), originCardId);

        // (x, y)에 오프셋 { {dx, dy}, ... }(보드 절대 좌표, §5.4)을 더한 칸의 상대 유닛에게 효과 주인의 CP만큼 피해를 준다. 반격은 없다.
        public void DamageArea(int x, int y, Table offsets)
        {
            Processor processor = Processor;
            // shortcut: 주인이 떠났으면 CP를 읽을 수 없어 건너뛴다. 사망 시 발동하는 범위 피해 카드가 생기면 떠나기 직전 CP를 쓰도록 바꾼다.
            if (!context.State.Units.TryGetValue(context.SourceUnitId, out UnitState source))
                return;

            CombatResolver combat = new();
            foreach (DynValue offset in offsets.Values)
            {
                int cellX = x + (int)offset.Table.Get(1).Number;
                int cellY = y + (int)offset.Table.Get(2).Number;
                if (!context.State.Board.Contains(cellX, cellY))
                    continue;

                int? unitId = context.State.Board.GetUnitIdAt(cellX, cellY);
                if (!unitId.HasValue)
                    continue;

                UnitState unit = context.State.Units[unitId.Value];
                // 앞선 피해로 이미 HP가 0 이하인 유닛은 사망 처리를 기다리므로 다시 때리지 않는다.
                if (!PlayerRelation.Opponent.Matches(unit.OwnerId, context.OwnerId) || unit.CurrentHP <= 0)
                    continue;

                processor.Damage(unit.Id, combat.CalculateDamage(source, unit), context.OwnerId);
            }
        }

        // 고른 대상을 { {unitId, x, y, originCardId}, ... }로 만든다. 고른 시점의 값이라 대상이 보드를 떠나도 읽을 수 있다.
        internal Table CreateTargetTable()
        {
            Table targets = new(script);
            foreach (TargetUnit unit in context.Targets)
            {
                Table entry = new(script);
                entry["unitId"] = unit.UnitId;
                entry["x"] = unit.Position.X;
                entry["y"] = unit.Position.Y;
                entry["originCardId"] = unit.OriginCardId;
                targets.Append(DynValue.NewTable(entry));
            }

            return targets;
        }
    }
}
