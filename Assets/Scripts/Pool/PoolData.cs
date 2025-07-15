using System.Collections.Generic;
using UnityEngine;
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
