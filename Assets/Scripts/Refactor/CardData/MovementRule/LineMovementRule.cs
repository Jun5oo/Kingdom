using System;
using UnityEngine;

namespace Refactor
{
    [Serializable]
    public sealed class LineMovementRule : MovementRule
    {
        [SerializeField] private RangeDirection directions = RangeDirection.Orthogonal;
        [SerializeField, Min(1)] private int maxDistance = 1;
        [SerializeField] private bool unlimitedDistance;

        public override bool CanMove(BoardState board,
            int fromX, int fromY, int targetX, int targetY, out string reason)
        {
            int dx = targetX - fromX;
            int dy = targetY - fromY;
            int absX = Math.Abs(dx);
            int absY = Math.Abs(dy);
            bool allowed =
                ((dx == 0 || dy == 0) && (directions & RangeDirection.Orthogonal) != 0) ||
                (absX == absY && (directions & RangeDirection.Diagonal) != 0);

            if (!allowed)
            {
                reason = "이동할 수 없는 방향입니다.";
                return false;
            }

            int distance = Math.Max(absX, absY);
            if (!unlimitedDistance && distance > maxDistance)
            {
                reason = "이동 가능한 거리를 초과했습니다.";
                return false;
            }

            int stepX = Math.Sign(dx);
            int stepY = Math.Sign(dy);
            for (int step = 1; step < distance; step++)
            {
                if (board.GetUnitIdAt(fromX + stepX * step, fromY + stepY * step).HasValue)
                {
                    reason = "이동 경로에 다른 유닛이 있습니다.";
                    return false;
                }
            }

            reason = string.Empty;
            return true;
        }
    }
}
