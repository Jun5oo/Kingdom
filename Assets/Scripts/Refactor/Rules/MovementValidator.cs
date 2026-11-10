using System;

namespace Refactor
{
    // 공통 조건을 확인한 뒤 카드에 설정된 이동 규칙에 판정을 맡긴다.
    public sealed class MovementValidator
    {
        public bool CanMove(BoardState board, MovementRule rule,
            int fromX, int fromY, int targetX, int targetY, out string reason)
        {
            if (board == null)
                throw new ArgumentNullException(nameof(board));

            if (rule == null)
            {
                reason = "이동 규칙이 없습니다.";
                return false;
            }

            if (!board.Contains(fromX, fromY) || !board.Contains(targetX, targetY))
            {
                reason = "보드 밖으로 이동할 수 없습니다.";
                return false;
            }

            if (fromX == targetX && fromY == targetY)
            {
                reason = "현재 위치로 이동할 수 없습니다.";
                return false;
            }

            if (board.GetUnitIdAt(targetX, targetY).HasValue)
            {
                reason = "도착 칸이 비어 있지 않습니다.";
                return false;
            }

            return rule.CanMove(board, fromX, fromY, targetX, targetY, out reason);
        }
    }
}
