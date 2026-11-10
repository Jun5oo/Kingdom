using System;

namespace Refactor
{
    public sealed class BoardState
    {
        public int Width { get; }
        public int Height { get; }

        // null은 빈 칸을 의미한다. Unity 객체 대신 유닛 ID만 저장한다.
        private readonly int?[,] occupants;

        public BoardState(int width, int height)
        {
            if (width <= 0)
                throw new ArgumentOutOfRangeException(nameof(width));
            if (height <= 0)
                throw new ArgumentOutOfRangeException(nameof(height));

            Width = width;
            Height = height;
            occupants = new int?[width, height];
        }

        public bool Contains(int x, int y)
        {
            return x >= 0 && x < Width && y >= 0 && y < Height;
        }

        public int? GetUnitIdAt(int x, int y)
        {
            if (x < 0 || x >= Width)
                throw new ArgumentOutOfRangeException(nameof(x));
            if (y < 0 || y >= Height)
                throw new ArgumentOutOfRangeException(nameof(y));

            return occupants[x, y];
        }

        // 보드의 기본 제약만 확인한다. 소환 범위 등의 규칙은 처리기에서 검사한다.
        internal bool TryPlace(int unitId, int x, int y)
        {
            if (!Contains(x, y) || occupants[x, y].HasValue)
                return false;

            if (TryGetPosition(unitId, out _, out _))
                return false;

            occupants[x, y] = unitId;
            return true;
        }

        public bool TryGetPosition(int unitId, out int x, out int y)
        {
            for (int column = 0; column < Width; column++)
            {
                for (int row = 0; row < Height; row++)
                {
                    if (occupants[column, row] != unitId)
                        continue;

                    x = column;
                    y = row;
                    return true;
                }
            }

            x = -1;
            y = -1;
            return false;
        }

        // GameStateから呼ぶ変更用メソッド。移動距離/コストはここでは扱わない。
        internal bool TryMove(int unitId, int x, int y)
        {
            if (!Contains(x, y) || occupants[x, y].HasValue ||
                !TryGetPosition(unitId, out int oldX, out int oldY))
                return false;

            occupants[oldX, oldY] = null;
            occupants[x, y] = unitId;
            return true;
        }

        internal bool TryRemove(int unitId)
        {
            if (!TryGetPosition(unitId, out int x, out int y))
                return false;

            occupants[x, y] = null;
            return true;
        }
    }
}
