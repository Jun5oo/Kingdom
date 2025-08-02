using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class PrefabLoader
{
    // Prefab을 Addressable을 통해서 가져오는 클래스 
    // 가져오기 위해서는 미리 PrefabLoader 생성자에서 등록해줄 필요가 있다. 

    // 오브젝트 타입을 통해서 Addressable Asset 키를 찾는 딕셔너리 
    Dictionary<Type, string> prefabDictionary;

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

        typeCache = new Dictionary<Type, GameObject>();
        stringCache = new Dictionary<string, GameObject>(); 
    }

    // MonoBehaviour 타입의 오브젝트 Prefab만 찾을 수 있음. 
    public async UniTask<GameObject> LoadPrefabAsync<T>() where T : MonoBehaviour
    {
        if(typeCache.TryGetValue(typeof(T), out GameObject cached))
            return cached; 

        if (!prefabDictionary.TryGetValue(typeof(T), out string key)){
            Debug.LogError("No prefab was found");
            return null; 
        }

        GameObject prefab = await Addressables.LoadAssetAsync<GameObject>(key);
        typeCache.Add(typeof(T), prefab); 
        return prefab; 
    }

    // MonoBehaviour 타입의 오브젝트가 아닌 Prefab을 찾기 위한 함수 
    public async UniTask<GameObject> LoadPrefabAsync(string addressKey)
    {
        if (stringCache.TryGetValue(addressKey, out GameObject cached))
            return cached;

        GameObject prefab = await Addressables.LoadAssetAsync<GameObject>(addressKey); 
        stringCache.Add(addressKey, prefab);
        return prefab; 
    }
}
