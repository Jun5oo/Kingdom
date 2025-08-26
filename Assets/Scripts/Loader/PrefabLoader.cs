using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.InputSystem;
using UnityEngine.ResourceManagement.AsyncOperations;

public class PrefabLoader
{
    // Prefab을 Addressable을 통해서 가져오는 클래스 
    // 가져오기 위해서는 미리 PrefabLoader 생성자에서 등록해줄 필요가 있다. 

    // 오브젝트 타입을 통해서 Addressable Asset 키를 찾는 딕셔너리 
    Dictionary<Type, string> prefabDictionary;

    Dictionary<string, AsyncOperationHandle<GameObject>> keyHandles; 

    // 오브젝트 타입을 통해 Prefab을 찾는 캐시 
    Dictionary<Type, GameObject> typeCache;
    // Addressable Asset key를 통해서 Prefab을 찾는 캐시 
    Dictionary<string, GameObject> stringCache;

    public PrefabLoader()
    {
        prefabDictionary = new Dictionary<Type, string>();

        // 이곳에 미리 Prefab을 등록해줘야 사용이 가능하다. 다만 이는 MonoBehaviour 타입을 가지는 프리팹만 가능하다. (MonoBehaviour 타입이 아닌 경우 밑 LoadPrefabAsync(string) 함수를 사용하고 Address key를 사용해야한다.) 
        prefabDictionary.Add(typeof(Card), "cardPrefab");
        prefabDictionary.Add(typeof(Token), "tokenPrefab");

        prefabDictionary.Add(typeof(ActionPopup), "actionPrefab");
        prefabDictionary.Add(typeof(DamagePopup), "damagePrefab");

        prefabDictionary.Add(typeof(NumberStatusPresenter), "numberStatusPrefab");
        prefabDictionary.Add(typeof(BarStatusPresenter), "barStatusPrefab");

        typeCache = new Dictionary<Type, GameObject>();
        stringCache = new Dictionary<string, GameObject>();

        keyHandles = new Dictionary<string, AsyncOperationHandle<GameObject>>(); 
    }

    // MonoBehaviour 타입의 오브젝트 Prefab만 찾을 수 있음. 
    public async UniTask<GameObject> LoadPrefabAsync<T>() where T : MonoBehaviour
    {
        if (typeCache.TryGetValue(typeof(T), out GameObject cached))
            return cached;

        if (!prefabDictionary.TryGetValue(typeof(T), out string key))
        {
            Debug.LogError("No prefab was found");
            return null;
        }

        var handle = Addressables.LoadAssetAsync<GameObject>(key);
        var prefab = await handle.ToUniTask(cancellationToken: default);

        if (prefab != null)
            typeCache.Add(typeof(T), prefab);
        else
            Debug.LogError($"{prefab}을 Load할 수 없습니다.");

        keyHandles.Add(key, handle); 
        // Addressables.Release(handle);

        return prefab;
    }

    // MonoBehaviour 타입의 오브젝트가 아닌 Prefab을 찾기 위한 함수 
    public async UniTask<GameObject> LoadPrefabAsync(string addressKey)
    {
        if (stringCache.TryGetValue(addressKey, out GameObject cached))
            return cached;

        var handle = Addressables.LoadAssetAsync<GameObject>(addressKey);
        var prefab = await handle.ToUniTask(cancellationToken: default);

        if (prefab != null)
            stringCache.Add(addressKey, prefab);

        else
            Debug.LogError($"{prefab}을 Load할 수 없습니다.");

        keyHandles.Add(addressKey, handle);

        // Addressables.Release(handle);

        return prefab;
    }

    public void ReleaseAll()
    {
        foreach(var k in keyHandles)
        {
            if (k.Value.IsValid())
                Addressables.Release(k.Value);
        }

        keyHandles.Clear();
        typeCache.Clear();
        stringCache.Clear(); 
    }
}
