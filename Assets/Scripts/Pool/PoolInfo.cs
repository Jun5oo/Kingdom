using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 풀링 데이터 클래스 
/// </summary>

public class PoolInfo 
{
    public GameObject prefab;
    public Transform parent;
    public Queue<GameObject> pool; 

    public PoolInfo(GameObject prefab, Transform parent)
    {
        this.prefab = prefab;
        this.parent = parent;

        this.pool = new Queue<GameObject>();
    }
}
