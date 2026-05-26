using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// IPoolable 컴포넌트를 타입별로 관리하는 오브젝트 풀 매니저.
/// Addressables로 프리팹을 비동기 로드한 뒤 타입별 풀을 등록하고,
/// Pop()으로 꺼내 쓰고 Push()로 반납한다. 풀이 비어 있으면 자동으로 새 인스턴스를 생성한다.
/// </summary>
public class PoolManager : MonoBehaviour
{
    Dictionary<Type, PoolData> poolDictionary; // 타입 → PoolData(프리팹·부모·큐)

    [SerializeField] Canvas canvas;

    PrefabLoader loader;

    GameObject actionPrefab; // Addressables에서 로드한 ActionPopup 프리팹
    GameObject damagePrefab; // Addressables에서 로드한 DamagePopup 프리팹

    GameObject actionPool;   // ActionPopup 풀 컨테이너
    GameObject damagePool;   // DamagePopup 풀 컨테이너

    /// <summary>
    /// Addressables로 팝업 프리팹을 비동기 로드하고 ActionPopup·DamagePopup 풀을 등록한다.
    /// Bootstrapper의 InitAsync 체인에서 호출된다.
    /// </summary>
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
    /// <summary> 타입 T에 대한 풀을 등록한다. </summary>
    public void RegisterPool<T>(GameObject prefab, Transform parent) where T : MonoBehaviour, IPoolable
    {
        poolDictionary[typeof(T)] = new PoolData(prefab, parent);
    }

    /// <summary>
    /// 풀에서 T 컴포넌트를 꺼내 활성화하여 반환한다.
    /// 풀이 비어 있으면 새 인스턴스를 생성한다.
    /// </summary>
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

    /// <summary> T 컴포넌트를 비활성화하고 풀에 반납한다. </summary>
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
