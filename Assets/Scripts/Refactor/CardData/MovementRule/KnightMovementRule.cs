using System;

namespace Refactor
{
    [Serializable]
    public sealed class KnightMovementRule : MovementRule
    {
        public override bool CanMove(BoardState board,
            int fromX, int fromY, int targetX, int targetY, out string reason)
        {
            int dx = Math.Abs(targetX - fromX);
            int dy = Math.Abs(targetY - fromY);
            if ((dx == 1 && dy == 2) || (dx == 2 && dy == 1))
            {
                // 점프 이동: 중간 칸은 검사하지 않는다.
                reason = string.Empty;
                return true;
            }

            reason = "나이트의 이동 가능한 위치가 아닙니다.";
            return false;
        }
    }
}
