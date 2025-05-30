using System.Collections.Generic;
using UnityEngine;

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
