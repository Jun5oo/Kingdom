using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class PrefabLoader
{
    Dictionary<Type, string> prefabDictionary;
    Dictionary<Type, GameObject> prefabCache; 

    public PrefabLoader()
    {
        prefabDictionary = new Dictionary<Type, string>();

        prefabDictionary.Add(typeof(ActionPopup), "actionPrefab");
        prefabDictionary.Add(typeof(DamagePopup), "damagePrefab"); 

        prefabCache = new Dictionary<Type, GameObject>();
    }

    public async UniTask<GameObject> LoadPrefabAsync<T>() where T : MonoBehaviour
    {
        if(prefabCache.TryGetValue(typeof(T), out GameObject cached))
            return cached; 

        if (!prefabDictionary.TryGetValue(typeof(T), out string key)){
            Debug.LogError("No prefab was found");
            return null; 
        }

        GameObject prefab = await Addressables.LoadAssetAsync<GameObject>(key);
        prefabCache.Add(typeof(T), prefab); 
        return prefab; 
    }
}
