using System.Collections.Generic;
using UnityEngine;

public class RangeResolver
{
    Dictionary<RangeType, List<Vector2Int>> rangeDict; 

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
        
        for(int i=2; i<=range; i++)
        {
            foreach(var v in rangeDict[rangeType])
                results.Add(v * i); 
        }

        return results; 
    }
}
