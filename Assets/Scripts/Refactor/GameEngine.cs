using System;

namespace Refactor
{
    // 플레이어/AI의 요청을 받는 진입점. 현재는 DB를 통해 카드 SO를 조회한다.
    public sealed class GameEngine
    {
        public GameState State { get; }
        private readonly CardDatabase cardDatabase;

        public GameEngine(GameState state, CardDatabase cardDatabase)
        {
            State = state ?? throw new ArgumentNullException(nameof(state));
            this.cardDatabase = cardDatabase ?? throw new ArgumentNullException(nameof(cardDatabase));
        }

        // 유닛의 현재 등급에 해당하는 원본 데이터를 조회한다. 상태는 변경하지 않는다.
        public bool TryGetUnitLevelData(int unitId, out UnitLevelData levelData, out string reason)
        {
            levelData = null;

            if (!State.Units.TryGetValue(unitId, out UnitState unit))
            {
                reason = "유닛이 존재하지 않습니다.";
                return false;
            }

            if (!cardDatabase.TryGetCard(unit.CardDefinitionId, out UnitCardData card))
            {
                reason = $"등록되지 않은 카드입니다: {unit.CardDefinitionId}";
                return false;
            }

            if (!card.TryGetLevel(unit.Level, out levelData))
            {
                reason = $"카드에 해당 등급이 없습니다: {unit.CardDefinitionId}, {unit.Level}성";
                return false;
            }

            reason = string.Empty;
            return true;
        }

        // 유닛 행동의 공통 검증만 수행한다. 상태는 변경하지 않는다.
        // 이후 이동/공격 진입점에서 이 검증과 행동별 규칙을 모두 확인한 뒤 실행한다.
        // true는 이동 거리, 비용, 공격 대상까지 유효하다는 뜻이 아니다.
        public bool CanActWithUnit(int requestingPlayerId, int unitId, out string reason)
        {
            if (!State.Players.ContainsKey(requestingPlayerId))
            {
                reason = "등록되지 않은 플레이어입니다.";
                return false;
            }

            if (State.CurrentPlayerId != requestingPlayerId)
            {
                reason = "현재 행동할 플레이어가 아닙니다.";
                return false;
            }

            if (!State.Units.TryGetValue(unitId, out UnitState unit))
            {
                reason = "유닛이 존재하지 않습니다.";
                return false;
            }

            if (unit.OwnerId != requestingPlayerId)
            {
                reason = "자신의 유닛만 조작할 수 있습니다.";
                return false;
            }

            if (!State.Board.TryGetPosition(unitId, out _, out _))
            {
                reason = "보드에 배치되지 않은 유닛입니다.";
                return false;
            }

            reason = string.Empty;
            return true;
        }
    }
}
