namespace Refactor
{
    // 보드 칸 좌표. 규칙 코드는 Unity의 Vector2Int 대신 이 타입을 쓴다.
    public readonly struct GridPos
    {
        public int X { get; }
        public int Y { get; }

        public GridPos(int x, int y)
        {
            X = x;
            Y = y;
        }

        public override string ToString() => $"({X}, {Y})";
    }
}
