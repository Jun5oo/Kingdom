using System;

namespace Refactor
{
    // 기본 공격만 처리한다. 반격, 방어 효과, 처형 이동은 아직 포함하지 않는다.
    public sealed class CombatResolver
    {
        public bool CanReach(BoardState board, RangeData range,
            int fromX, int fromY, int targetX, int targetY, out string reason)
        {
            if (range == null)
            {
                reason = "공격 규칙이 없습니다.";
                return false;
            }
            if (!board.Contains(fromX, fromY) || !board.Contains(targetX, targetY))
            {
                reason = "공격 위치가 보드 밖입니다.";
                return false;
            }

            int dx = targetX - fromX;
            int dy = targetY - fromY;
            int absX = Math.Abs(dx);
            int absY = Math.Abs(dy);
            int distance = Math.Max(absX, absY);
            bool allowed =
                ((dx == 0 || dy == 0) && (range.Directions & RangeDirection.Orthogonal) != 0) ||
                (absX == absY && (range.Directions & RangeDirection.Diagonal) != 0);
            if (!allowed || distance == 0 || distance > range.MaxDistance)
            {
                reason = "공격 범위 밖입니다.";
                return false;
            }

            for (int step = 1; step < distance; step++)
            {
                if (board.GetUnitIdAt(fromX + Math.Sign(dx) * step, fromY + Math.Sign(dy) * step).HasValue)
                {
                    reason = "공격 경로에 다른 유닛이 있습니다.";
                    return false;
                }
            }

            reason = string.Empty;
            return true;
        }

        // 상태를 변경하지 않고 피해량만 계산한다. 실제 반영은 엔진이 담당한다.
        public int CalculateDamage(UnitState attacker, UnitState defender)
        {
            return Math.Min(Math.Max(0, attacker.CP), Math.Max(0, defender.CurrentHP));
        }
    }
}
