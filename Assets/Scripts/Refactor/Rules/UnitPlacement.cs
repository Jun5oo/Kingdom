namespace Refactor
{
    // 게임 시작 시 보드에 미리 놓을 유닛 하나의 설정.
    // shortcut: 실제 규칙에는 없는 시작 배치다. 7단계에서 킹 배치 단계와 소환이 생기면 테스트 보드 세팅 전용으로 줄인다.
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
