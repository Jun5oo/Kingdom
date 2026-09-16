using System;

namespace Refactor
{
    public sealed class UnitState
    {
        // 같은 카드에서 생성된 유닛도 서로 다른 Id를 가진다.
        public int Id { get; }
        public string CardDefinitionId { get; }
        public int OwnerId { get; internal set; }
        public int CP { get; internal set; }
        public int Level { get; internal set; }

        // 위치는 BoardState에서만 보관한다.
        public UnitState(int id, string cardDefinitionId, int ownerId, int cp, int level = 1)
        {
            if (string.IsNullOrWhiteSpace(cardDefinitionId))
                throw new ArgumentException("카드 ID가 필요합니다.", nameof(cardDefinitionId));

            if (level < 1)
                throw new ArgumentOutOfRangeException(nameof(level), "등급은 1 이상이어야 합니다.");

            Id = id;
            CardDefinitionId = cardDefinitionId;
            OwnerId = ownerId;
            CP = cp;
            Level = level;
        }
    }
}
