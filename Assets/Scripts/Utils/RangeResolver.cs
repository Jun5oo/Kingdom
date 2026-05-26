using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// RangeType과 range 값을 Vector2Int 오프셋 목록으로 변환하는 유틸리티.
/// Straight(상하좌우), Bishop(대각), Queen(8방향)에 range만큼 배수 확장한다.
/// Token.Init()에서 공격/이동 범위 벡터를 초기화할 때 사용된다.
/// </summary>
public class RangeResolver
{
    Dictionary<RangeType, List<Vector2Int>> rangeDict; // RangeType → 단위 방향 벡터 목록

    public RangeResolver()
    {
        rangeDict = new Dictionary<RangeType, List<Vector2Int>>();
        rangeDict.Add(RangeType.None, new List<Vector2Int>());
        rangeDict.Add(RangeType.Straight, new List<Vector2Int>
        {
            new Vector2Int(-1, 0),
            new Vector2Int(1, 0),
            new Vector2Int(0, -1),
            new Vector2Int(0, 1),
        });
        rangeDict.Add(RangeType.Bishop, new List<Vector2Int>()
        {
            new Vector2Int(-1, -1),
            new Vector2Int(-1, 1),
            new Vector2Int(1, -1),
            new Vector2Int(1, 1),
        });
        rangeDict.Add(RangeType.Queen, new List<Vector2Int>()
        {
            new Vector2Int(-1, 0),
            new Vector2Int(1, 0),
            new Vector2Int(0, -1),
            new Vector2Int(0, 1),
            new Vector2Int(-1, -1),
            new Vector2Int(-1, 1),
            new Vector2Int(1, -1),
            new Vector2Int(1, 1),
        });
    }

    /// <summary>
    /// rangeType의 단위 벡터를 1부터 range까지 배수 확장한 전체 오프셋 목록을 반환한다.
    /// range=1이면 인접 셀만, range=2이면 2칸까지 포함된다.
    /// </summary>
    public List<Vector2Int> Resolve(RangeType rangeType, int range)
    {
        if (range <= 0 || rangeType == RangeType.None)
            return new List<Vector2Int>();

        if (!rangeDict.TryGetValue(rangeType, out List<Vector2Int> vectors))
        {
            Debug.Log($"{rangeType}의 Vector2Int 리스트를 찾을 수 없습니다.");
            return new List<Vector2Int>();
        }

        List<Vector2Int> results = new List<Vector2Int>(vectors);

        for (int i = 2; i <= range; i++)
        {
            foreach (var v in rangeDict[rangeType])
                results.Add(v * i);
        }

        return results;
    }
}
