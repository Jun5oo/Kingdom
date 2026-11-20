using System;
using System.Collections.Generic;

namespace Refactor
{
    public sealed class UnitState
    {
        // 같은 카드에서 생성된 유닛도 서로 다른 Id를 가진다.
        public int Id { get; }
        public string CardDefinitionId { get; }
        public int OwnerId { get; internal set; }
        public int CP { get; internal set; }
        public int MaxHP { get; internal set; }
        public int CurrentHP { get; internal set; }
        // 무덤처럼 다른 유닛이 죽은 자리에서 생겨났으면 그 유닛의 카드 ID. 부활할 때 쓴다.
        public string OriginCardId { get; }

        // 효과 인덱스별 이번 턴 사용 횟수.
        private readonly Dictionary<int, int> effectUsesThisTurn = new();
        // 붙은 순서대로 둔다. 같은 종류도 쌓인다(§5.3).
        private readonly List<Status> statuses = new();

        public IReadOnlyList<Status> Statuses => statuses;

        // 위치는 BoardState에서만, 레벨은 카드 정의에서만 보관한다.
        public UnitState(int id, string cardDefinitionId, int ownerId, int cp, int maxHP, string originCardId = null)
        {
            if (string.IsNullOrWhiteSpace(cardDefinitionId))
                throw new ArgumentException("카드 ID가 필요합니다.", nameof(cardDefinitionId));

            if (maxHP < 1)
                throw new ArgumentOutOfRangeException(nameof(maxHP), "최대 HP는 1 이상이어야 합니다.");

            Id = id;
            CardDefinitionId = cardDefinitionId;
            OwnerId = ownerId;
            CP = cp;
            MaxHP = maxHP;
            CurrentHP = maxHP;
            OriginCardId = originCardId;
        }

        // usesPerTurn이 0이면 무제한이다.
        public bool CanUseEffect(int effectIndex, int usesPerTurn)
        {
            effectUsesThisTurn.TryGetValue(effectIndex, out int uses);
            return usesPerTurn == 0 || uses < usesPerTurn;
        }

        internal void MarkEffectUsed(int effectIndex)
        {
            effectUsesThisTurn.TryGetValue(effectIndex, out int uses);
            effectUsesThisTurn[effectIndex] = uses + 1;
        }

        internal void ResetEffectUses() => effectUsesThisTurn.Clear();

        public bool HasStatus(StatusType type) => statuses.Exists(status => status.Type == type);

        internal void AddStatus(Status status) => statuses.Add(status);

        internal void RemoveStatus(Status status) => statuses.Remove(status);
    }
}
