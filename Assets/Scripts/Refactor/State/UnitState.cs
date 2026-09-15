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

        // 위치는 BoardState에서만 보관한다.
        public UnitState(int id, string cardDefinitionId, int ownerId, int cp)
        {
            if (string.IsNullOrWhiteSpace(cardDefinitionId))
                throw new ArgumentException("카드 ID가 필요합니다.", nameof(cardDefinitionId));

            Id = id;
            CardDefinitionId = cardDefinitionId;
            OwnerId = ownerId;
            CP = cp;
        }
    }
}
