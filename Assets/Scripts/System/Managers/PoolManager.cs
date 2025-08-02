using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    Dictionary<Type, PoolData> poolDictionary;

    [SerializeField] Canvas canvas; 

    PrefabLoader loader; 

    GameObject actionPrefab; // Get object from addressable 
    GameObject damagePrefab; // Get object from addressable 

    GameObject actionPool;
    GameObject damagePool; 

    public async UniTask InitAsync()
    {
        loader = ServiceLocator.Get<PrefabLoader>();

        actionPrefab = await loader.LoadPrefabAsync<ActionPopup>();
        damagePrefab = await loader.LoadPrefabAsync<DamagePopup>();

        actionPool = new GameObject("ActionPool", typeof(RectTransform));
        damagePool = new GameObject("DamagePool", typeof(RectTransform));

        actionPool.transform.SetParent(canvas.transform, true);
        damagePool.transform.SetParent(canvas.transform, true); 

        poolDictionary = new Dictionary<Type, PoolData>();

        RegisterPool<ActionPopup>(actionPrefab, actionPool.transform);
        RegisterPool<DamagePopup>(damagePrefab, damagePool.transform);
    }

    #region Pooling 
    public void RegisterPool<T>(GameObject prefab, Transform parent) where T : MonoBehaviour, IPoolable
    {
        poolDictionary[typeof(T)] = new PoolData(prefab, parent);
    }
    public T Pop<T>() where T : MonoBehaviour, IPoolable
    {
        if (poolDictionary.TryGetValue(typeof(T), out PoolData poolInfo))
        {
            GameObject obj = null;

            if (poolInfo.pool.Count == 0)
                obj = GameObject.Instantiate(poolInfo.prefab, poolInfo.parent);

            if (obj == null)
                obj = poolInfo.pool.Dequeue();

            obj.SetActive(true);

            if(obj.TryGetComponent<T>(out T component))
                return component; 
        }

        Debug.LogError($"No pool register for type {typeof(T)}");
        return null;
    }
    public void Push<T>(T component) where T : MonoBehaviour, IPoolable
    {
        if (poolDictionary.TryGetValue(typeof(T), out PoolData poolInfo))
        {
            component.gameObject.SetActive(false);
            poolInfo.pool.Enqueue(component.gameObject);
            component.transform.SetParent(poolInfo.parent, true);
        }

        else
            Debug.LogError($"No pool registered for type {typeof(T)}");
    }
    #endregion
}
