using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
/// <summary>
/// Addressables 기반 텍스처 로더 기반 클래스. artCache로 아트 텍스처를 캐싱하며
/// LoadAllTextures()는 서브클래스에서 종류별 텍스처를 조합해 VisualTexture를 반환한다.
/// </summary>
public abstract class TextureLoader
{
    protected Dictionary<string, Texture2D> artCache;
    public virtual void Init()
    {
        artCache = new Dictionary<string, Texture2D>();
    }

    public virtual string CardAddress(string cardID) => $"card_art_{cardID}";
    public virtual async UniTask<Texture2D> LoadArtAsync(string cardID)
    {
        if (artCache.TryGetValue(cardID, out Texture2D cached))
            return cached;

        string address = CardAddress(cardID);
        var handle = Addressables.LoadAssetAsync<Texture2D>(address);
        var texture = await handle.ToUniTask(cancellationToken: default);

        if (texture != null)
            artCache[cardID] = texture;
        else
            Debug.LogError($"{texture}를 Load하지 못했습니다.");

        Addressables.Release(handle);

        return texture;
    }
    public abstract UniTask<VisualTexture> LoadAllTextures(CardData cardData);
}
