using System;
using UnityEngine;

namespace Refactor
{
    [Flags]
    public enum RangeDirection
    {
        None = 0,
        Orthogonal = 1, // 상하좌우
        Diagonal = 2    // 대각선
    }

    // 범위의 모양만 정의한다. 장애물/관통 등의 규칙은 별도로 처리한다.
    [Serializable]
    public sealed class RangeData
    {
        [SerializeField] private RangeDirection directions;
        [SerializeField, Min(0)] private int maxDistance;

        public RangeDirection Directions => directions;
        public int MaxDistance => maxDistance;
    }
}
