using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class SpriteLoader
{
    // SpriteLoader: Sprite Image를 Addressable Asset을 통해서 가져옴 
    const string ICON_LABEL = "Icon";

    public List<AsyncOperationHandle> handle = new List<AsyncOperationHandle>();
    protected Dictionary<string, Sprite> spriteCache;
    public async UniTask Init()
    {
        spriteCache = new Dictionary<string, Sprite>();

        await PreLoadSpriteAsync(ICON_LABEL); 
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

        var sprite = await Addressables.LoadAssetAsync<Sprite>(spriteID).ToUniTask();
        spriteCache[spriteID] = sprite;
        return sprite; 
    }

}
