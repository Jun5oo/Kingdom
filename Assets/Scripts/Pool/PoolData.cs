using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 단일 프리팹 타입의 풀 상태(프리팹, 부모 Transform, 대기 큐)를 보관하는 데이터 클래스.
/// PoolManager가 타입별로 이 인스턴스를 관리한다.
/// </summary>
public class PoolData
{
    public GameObject prefab;
    public Transform parent;
    public Queue<GameObject> pool; 

    public PoolData(GameObject prefab, Transform parent)
    {
        this.prefab = prefab;
        this.parent = parent;

        this.pool = new Queue<GameObject>();
    }
}
