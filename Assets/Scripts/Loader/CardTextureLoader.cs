using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
/// <summary>
/// 카드 전용 텍스처 로더. Art, ArtMask, Frame, Background, Back 5종을 Addressables로 로드하고 캐싱한다.
/// 주소 규칙: card_art_{id}, card_artmask_{race}_{type}, card_frame_{race}_{type}, card_bg_{race}, card_back_{back}.
/// </summary>
public class CardTextureLoader : TextureLoader
{
    Dictionary<(string race, string type), Texture2D> artMaskCache;
    Dictionary<(string race, string type), Texture2D> frameCache;

    Dictionary<string, Texture2D> backgroundCache;
    Dictionary<string, Texture2D> backCache;

    public override void Init()
    {
        base.Init();

        artMaskCache = new Dictionary<(string race, string type), Texture2D>() { };
        frameCache = new Dictionary<(string race, string type), Texture2D>() { };
        backgroundCache = new Dictionary<string, Texture2D>() { };
        backCache = new Dictionary<string, Texture2D> { };
    }

    public async UniTask<Texture2D> LoadArtMaskAsync(string race, string type)
    {
        if (artMaskCache.TryGetValue((race, type), out Texture2D cached))
            return cached;

        string address = $"card_artmask_{race}_{type}";

        var handle = Addressables.LoadAssetAsync<Texture2D>(address);
        var texture = await handle.ToUniTask(cancellationToken: default);

        if (texture != null)
            artMaskCache[(race, type)] = texture;
        else
            Debug.LogError($"{texture}를 Load하지 못했습니다.");

        Addressables.Release(handle);

        return texture;
    }
    public async UniTask<Texture2D> LoadFrameAsync(string race, string type)
    {
        if (frameCache.TryGetValue((race, type), out Texture2D cached))
            return cached;

        string address = $"card_frame_{race}_{type}";
        var handle = Addressables.LoadAssetAsync<Texture2D>(address);
        var texture = await handle.ToUniTask(cancellationToken: default);

        if (texture != null)
            frameCache[(race, type)] = texture;
        else
            Debug.LogError($"{texture}를 Load하지 못했습니다.");

        Addressables.Release(handle);

        return texture;
    }
    public async UniTask<Texture2D> LoadCardBackgroundAsync(string race)
    {
        if (backgroundCache.TryGetValue(race, out Texture2D cached))
            return cached;

        string address = $"card_bg_{race}";
        var handle = Addressables.LoadAssetAsync<Texture2D>(address);
        var texture = await handle.ToUniTask(cancellationToken: default);
        if (texture != null)
            backgroundCache[race] = texture;
        else
            Debug.LogError($"{texture}를 Load하지 못했습니다.");

        Addressables.Release(handle);

        return texture;
    }
    public async UniTask<Texture2D> LoadCardBackAsync(string back)
    {
        if (backCache.TryGetValue(back, out Texture2D cached))
            return cached;

        string address = $"card_back_{back}";
        var handle = Addressables.LoadAssetAsync<Texture2D>(address);
        var texture = await handle.ToUniTask(cancellationToken: default);
        if (texture != null)
            backCache[back] = texture;
        else
            Debug.LogError($"{texture}를 Load하지 못했습니다.");

        Addressables.Release(handle);

        return texture;
    }

    public override async UniTask<VisualTexture> LoadAllTextures(CardData cardData)
    {
        string cardID = cardData.ID;
        string race = cardData.Race.ToString().ToLower();
        string type = cardData.Tag == UnitTag.King ? "king" : "normal";

        Debug.Log($"{cardID}, {race}, {type}");

        Texture2D art = await LoadArtAsync(cardID);
        Texture2D artMask = await LoadArtMaskAsync(race, type);
        Texture2D frame = await LoadFrameAsync(race, type);
        Texture2D background = await LoadCardBackgroundAsync(race);
        Texture2D back = await LoadCardBackAsync("default");

        return new VisualTexture(art, artMask, frame, background, back);
    }

}
