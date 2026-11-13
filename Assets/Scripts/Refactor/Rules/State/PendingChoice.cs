using System.Collections.Generic;

namespace Refactor
{
    // 선택 대기 내용: 누가, 어떤 후보 중에서, 몇 개를 고르는지(§2.3).
    public sealed class PendingChoice
    {
        public int PlayerId { get; }
        // 유닛 ID. 필드에 나온 순서로 정렬되어 있다.
        public IReadOnlyList<int> Candidates { get; }
        public int Count { get; }

        public PendingChoice(int playerId, IReadOnlyList<int> candidates, int count)
        {
            PlayerId = playerId;
            Candidates = candidates;
            Count = count;
        }
    }
}
