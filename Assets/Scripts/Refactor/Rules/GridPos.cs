using Newtonsoft.Json;

namespace Refactor
{
    // 보드 칸 좌표. 규칙 코드는 Unity의 Vector2Int 대신 이 타입을 쓴다.
    public readonly struct GridPos
    {
        public int X { get; }
        public int Y { get; }

        // 구조체는 기본 생성자가 있어 Newtonsoft가 이 생성자를 쓰지 않으므로 지정한다. 없으면 JSON 값이 0으로 남는다.
        [JsonConstructor]
        public GridPos(int x, int y)
        {
            X = x;
            Y = y;
        }

        public override string ToString() => $"({X}, {Y})";
    }
}
