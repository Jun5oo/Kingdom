using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

/// <summary>
/// Addressables 기반 스프라이트 로더. Init() 시 "Icon"/"Coin" 레이블의 스프라이트를 일괄 프리로드하고
/// spriteCache에 캐싱한다. ActionPopup 아이콘 등에서 LoadSpriteAsync(id)로 사용한다.
/// </summary>
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

        foreach (var location in locations)
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
