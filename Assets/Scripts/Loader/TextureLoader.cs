using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
public abstract class TextureLoader
{
    protected Dictionary<string, Texture2D> artCache;
    public virtual void Init()
    {
        artCache = new Dictionary<string, Texture2D> ();
    }

    public virtual string CardAddress(string cardID) => $"card_art_{cardID}"; 
    public virtual async UniTask<Texture2D> LoadArtAsync(string cardID)
    {
        if (artCache.TryGetValue(cardID, out Texture2D cached))
            return cached; 

        string address = CardAddress(cardID);
        var texture = await Addressables.LoadAssetAsync<Texture2D>(address).ToUniTask();
        artCache[cardID] = texture; 
        return texture; 
    }
    public abstract UniTask<VisualTexture> LoadAllTextures(CardData cardData); 
}
