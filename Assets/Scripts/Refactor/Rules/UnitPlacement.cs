namespace Refactor
{
    // 게임 시작 시 배치할 유닛 하나의 설정.
    public readonly struct UnitPlacement
    {
        public int OwnerId { get; }
        public string CardId { get; }
        public int Level { get; }
        public GridPos Position { get; }

        public UnitPlacement(int ownerId, string cardId, int level, GridPos position)
        {
            OwnerId = ownerId;
            CardId = cardId;
            Level = level;
            Position = position;
        }
    }
}
