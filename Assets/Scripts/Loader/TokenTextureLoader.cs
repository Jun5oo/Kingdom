using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
public class TokenTextureLoader : TextureLoader
{

    Dictionary<string, Texture2D> artMaskCache;
    Dictionary<string, Texture2D> frameCache;

    public override void Init()
    {
        base.Init();

        artMaskCache = new Dictionary<string, Texture2D>();
        frameCache = new Dictionary<string, Texture2D>();
    }

    public async UniTask PreLoadTokenTexture(string label)
    {

        // Mask, Frame Preload를 따로 만들어야 할 것 같다. 
        // 1. key를 찾는다. 
        var locationHandle = Addressables.LoadResourceLocationsAsync(label, typeof(Texture2D));
        var locations = await locationHandle.Task;

        foreach (var location in locations)
        {
            var _handle = Addressables.LoadAssetAsync<Texture2D>(location);
            var sprite = await _handle.Task;

            artMaskCache[location.PrimaryKey] = sprite;
        }

        Addressables.Release(locationHandle);
    }

    public async UniTask<Texture2D> LoadArtMaskAsync(string type)
    {
        if (artMaskCache.TryGetValue(type, out Texture2D cached))
            return cached;

        string address = $"token_artmask_{type}";

        var handle = Addressables.LoadAssetAsync<Texture2D>(address);
        var texture = await handle.ToUniTask(cancellationToken: default);

        if (texture != null)
            artMaskCache[type] = texture;
        else
            Debug.LogError($"{texture}를 Load 할 수 없습니다.");

        Addressables.Release(handle);

        return texture;
    }
    public async UniTask<Texture2D> LoadFrameAsync(string type)
    {
        if (frameCache.TryGetValue(type, out Texture2D cached))
            return cached;

        string address = $"token_frame_{type}";

        var handle = Addressables.LoadAssetAsync<Texture2D>(address);
        var texture = await handle.ToUniTask(cancellationToken: default);


        if (texture != null)
            frameCache[type] = texture;

        else
            Debug.LogError($"{texture}를 Load 할 수 없습니다.");

        Addressables.Release(handle);

        return texture;
    }

    public override async UniTask<VisualTexture> LoadAllTextures(CardData cardData)
    {
        string cardID = $"{cardData.ID}";
        string type = cardData.Tag == UnitTag.King ? "king" : "normal";

        Texture2D art = await LoadArtAsync($"{cardID}");
        Texture2D artMask = await LoadArtMaskAsync(type);
        Texture2D frame = await LoadFrameAsync(type);

        return new VisualTexture(art, artMask, frame);
    }

}
