using System;

namespace Refactor
{
    [Serializable]
    public abstract class MovementRule
    {
        // Validator가 범위, 제자리 이동, 도착 칸 점유를 먼저 검사한다.
        public abstract bool CanMove(BoardState board,
            int fromX, int fromY, int targetX, int targetY, out string reason);
    }
}
