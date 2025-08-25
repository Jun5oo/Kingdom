using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class _CardDatabase 
{
    Dictionary<string, AssetReferenceT<CardData>> index;
    Dictionary<string, CardData> cache;

    public _CardDatabase(CardIndex index)
    {
        this.index = new Dictionary<string, AssetReferenceT<CardData>>();
        this.cache = new Dictionary<string, CardData>();

        foreach(var entry in index.entries)
        {
            if (entry != null /* && entry.cardID != null */)
                this.index[entry.cardID] = entry.dataReference;
        }
    }

    public bool TryGetCached(string id, out CardData data) => cache.TryGetValue(id, out data);

    public UniTask<CardData> GetAsync(string id)
    {
        if (cache.TryGetValue(id, out CardData data))
            return UniTask.FromResult(data); 
        
        if(!index.TryGetValue(id, out AssetReferenceT<CardData> dataReference))
        {
            Debug.LogError($"Index릍 통해 {id} 카드를 찾을 수 없습니다.");
            return UniTask.FromResult<CardData>(null); 
        }

        var cardData = LoadCardDataAsync(dataReference);
        return cardData;
    }
   
    public async UniTask<CardData> LoadCardDataAsync(AssetReferenceT<CardData> dataRef)
    {
        AsyncOperationHandle<CardData> handle = dataRef.LoadAssetAsync<CardData>();
        await handle.Task;

        var data = handle.Result; 
        if(!cache.ContainsKey(data.ID))
            cache[data.ID] = data;

        return data; 
    }

    public void ReleaseAll()
    {
        foreach(var c in cache)
        {
            Addressables.Release(c); 
        }

        cache.Clear(); 
    }
}

