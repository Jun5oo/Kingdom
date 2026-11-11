using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Refactor
{
    // 진행 중인 한 판의 상태. 규칙 판정과 연출은 별도 처리기가 담당한다.
    public sealed class GameState
    {
        public int CurrentPlayerId { get; internal set; }
        public int TurnNumber { get; internal set; } = 1;

        private readonly Dictionary<int, PlayerState> players = new();
        private readonly Dictionary<int, UnitState> units = new();
        // 트리거 순서(§4.2)의 기준이므로 생성 순으로만 증가하고 재사용하지 않는다.
        private int nextUnitId = 1;

        // 외부에서는 목록을 조회만 한다. 등록/제거는 아래 메서드를 거친다.
        public IReadOnlyDictionary<int, PlayerState> Players { get; }
        public IReadOnlyDictionary<int, UnitState> Units { get; }

        public BoardState Board { get; }

        // 항상 2인 게임이므로 생성 시 두 플레이어를 받는다. firstPlayer가 선공이다.
        public GameState(PlayerState firstPlayer, PlayerState secondPlayer,
            int boardWidth = 7, int boardHeight = 7)
        {
            if (firstPlayer == null)
                throw new ArgumentNullException(nameof(firstPlayer));
            if (secondPlayer == null)
                throw new ArgumentNullException(nameof(secondPlayer));
            if (firstPlayer.Id == secondPlayer.Id)
                throw new ArgumentException("플레이어 ID가 중복되었습니다.", nameof(secondPlayer));

            players.Add(firstPlayer.Id, firstPlayer);
            players.Add(secondPlayer.Id, secondPlayer);
            CurrentPlayerId = firstPlayer.Id;

            Board = new BoardState(boardWidth, boardHeight);
            Players = new ReadOnlyDictionary<int, PlayerState>(players);
            Units = new ReadOnlyDictionary<int, UnitState>(units);
        }

        internal int AllocateUnitId() => nextUnitId++;

        // 상태의 기본 제약만 검사한다. 소환 비용/범위는 행동 처리기가 검사한다.
        // 실패할 때는 유닛 목록과 보드를 모두 변경하지 않는다.
        public bool TrySpawnUnit(UnitState unit, int x, int y)
        {
            if (unit == null || units.ContainsKey(unit.Id) ||
                !players.ContainsKey(unit.OwnerId))
                return false;

            if (!Board.TryPlace(unit.Id, x, y))
                return false;

            units.Add(unit.Id, unit);
            return true;
        }

        public bool TryMoveUnit(int unitId, int x, int y)
        {
            if (!units.ContainsKey(unitId))
                return false;

            return Board.TryMove(unitId, x, y);
        }

        // 제거는 사망 판정이 아니다. 사망/귀환 등 의미는 호출하는 처리기가 결정한다.
        public bool TryRemoveUnit(int unitId)
        {
            if (!units.ContainsKey(unitId) || !Board.TryRemove(unitId))
                return false;

            units.Remove(unitId);
            return true;
        }
    }
}
