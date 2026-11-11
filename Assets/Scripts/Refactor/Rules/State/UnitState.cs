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
        public int Level { get; internal set; }

        // 효과 인덱스별 이번 턴 사용 횟수. 레벨이 바뀌면 효과 목록이 교체되므로 함께 초기화한다(§6.4).
        private readonly Dictionary<int, int> effectUsesThisTurn = new();

        // 위치는 BoardState에서만 보관한다.
        public UnitState(int id, string cardDefinitionId, int ownerId, int cp, int maxHP, int level = 1)
        {
            if (string.IsNullOrWhiteSpace(cardDefinitionId))
                throw new ArgumentException("카드 ID가 필요합니다.", nameof(cardDefinitionId));

            if (level < 1)
                throw new ArgumentOutOfRangeException(nameof(level), "등급은 1 이상이어야 합니다.");

            if (maxHP < 1)
                throw new ArgumentOutOfRangeException(nameof(maxHP), "최대 HP는 1 이상이어야 합니다.");

            Id = id;
            CardDefinitionId = cardDefinitionId;
            OwnerId = ownerId;
            CP = cp;
            MaxHP = maxHP;
            CurrentHP = maxHP;
            Level = level;
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
    }
}
