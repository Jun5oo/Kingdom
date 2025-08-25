using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class SpriteLoader
{
    // SpriteLoader: Sprite Image를 Addressable Asset을 통해서 가져옴 
    const string ICON_LABEL = "Icon";
    const string COIN_LABEL = "Coin"; 

    public List<AsyncOperationHandle> handle = new List<AsyncOperationHandle>();
    protected Dictionary<string, Sprite> spriteCache;
    public async UniTask Init()
    {
        spriteCache = new Dictionary<string, Sprite>();

        await PreLoadSpriteAsync(ICON_LABEL);
        await PreLoadSpriteAsync(COIN_LABEL); 
    }

    public async UniTask PreLoadSpriteAsync(string label)
    {
        // 1. key를 찾는다. 
        var locationHandle = Addressables.LoadResourceLocationsAsync(label, typeof(Sprite)); 
        var locations = await locationHandle.Task; 

        foreach(var location in locations)
        {
            var _handle = Addressables.LoadAssetAsync<Sprite>(location);
            var sprite = await _handle.Task;

            spriteCache[location.PrimaryKey] = sprite;
            handle.Add(_handle);
        }

        Addressables.Release(locationHandle); 
    }

    public async UniTask<Sprite> LoadSpriteAsync(string spriteID)
    {
        if (spriteCache.TryGetValue(spriteID, out Sprite cached))
            return cached;

        var handle = Addressables.LoadAssetAsync<Sprite>(spriteID);
        var sprite = await handle.ToUniTask(cancellationToken: default);

        if (sprite != null)
            spriteCache[spriteID] = sprite;
        else
            Debug.LogError($"{sprite}를 Load 하지 못했습니다.");

        Addressables.Release(handle);

        return sprite; 
    }

}
