using System;
using UnityEngine;

namespace Refactor
{
    // 게임 시작 시 배치할 유닛 하나의 설정.
    [Serializable]
    public struct UnitPlacement
    {
        [SerializeField] private int ownerId;
        [SerializeField] private string cardId;
        [SerializeField, Min(1)] private int level;
        [SerializeField] private Vector2Int position;

        public int OwnerId => ownerId;
        public string CardId => cardId;
        public int Level => level;
        public Vector2Int Position => position;

        public UnitPlacement(int ownerId, string cardId, int level, Vector2Int position)
        {
            this.ownerId = ownerId;
            this.cardId = cardId;
            this.level = level;
            this.position = position;
        }
    }
}
