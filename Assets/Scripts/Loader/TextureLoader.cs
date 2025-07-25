using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
public class TextureLoader
{
    Dictionary<string, Texture2D> artCache;
    Dictionary<(string race, string type), Texture2D> artMaskCache; 
    Dictionary<(string race, string type), Texture2D> frameCache;
    Dictionary<string, Texture2D> backgroundCache;
    Dictionary<string, Texture2D> backCache; 

    public void Init()
    {
        artCache = new Dictionary<string, Texture2D> ();
        artMaskCache = new Dictionary<(string race, string type), Texture2D>() { };
        frameCache = new Dictionary<(string race, string type), Texture2D> () { };
        backgroundCache = new Dictionary<string, Texture2D>() { };
        backCache = new Dictionary<string, Texture2D> { };
    }

    public string CardAddress(string cardID) => $"card_art_{cardID}"; 
    public async UniTask<Texture2D> LoadArtAsync(string cardID)
    {
        if (artCache.TryGetValue(cardID, out Texture2D cached))
            return cached; 

        string address = CardAddress(cardID);
        var texture = await Addressables.LoadAssetAsync<Texture2D>(address).ToUniTask();
        artCache[cardID] = texture; 
        return texture; 
    }
    public async UniTask<Texture2D> LoadArtMaskAsync(string race, string type)
    {
        if (artMaskCache.TryGetValue((race, type), out Texture2D cached))
            return cached;

        string address = $"card_artmask_{race}_{type}";
        var texture = await Addressables.LoadAssetAsync<Texture2D>(address).ToUniTask();
        artMaskCache[(race, type)] = texture;
        return texture; 
    }
    public async UniTask<Texture2D> LoadFrameAsync(string race, string type)
    {
        if (frameCache.TryGetValue((race, type), out Texture2D cached))
            return cached;

        string address = $"card_frame_{race}_{type}";
        var texture = await Addressables.LoadAssetAsync<Texture2D>(address).ToUniTask();
        frameCache[(race, type)] = texture;
        return texture; 
    }
    public async UniTask<Texture2D> LoadCardBackgroundAsync(string race)
    {
        if (backgroundCache.TryGetValue(race, out Texture2D cached))
            return cached;

        string address = $"card_bg_{race}";
        var texture = await Addressables.LoadAssetAsync<Texture2D>(address).ToUniTask();
        backgroundCache[race] = texture;

        return texture; 
    }
    public async UniTask<Texture2D> LoadCardBackAsync(string back)
    {
        if (backCache.TryGetValue(back, out Texture2D cached))
            return cached;

        string address = $"card_back_{back}";
        var texture = await Addressables.LoadAssetAsync<Texture2D>(address).ToUniTask();
        backCache[back] = texture;

        return texture; 
    }
    public async UniTask<VisualTexture> LoadAllTextures(CardData cardData)
    {
        string cardID = $"{cardData.ID}";
        string race = null;
        string type = null; 

        switch (cardData.Race)
        {
            case Race.Undead:
                race = "undead";
                break;
            case Race.Celestial:
                race = "celestial";
                break;
            default:
                race = "default";
                break; 
        }

        if (cardData is UnitCardData unitData)
            type = unitData.IsKing ? "king" : "normal";  

        Texture2D art = await LoadArtAsync($"{cardID}");
        Texture2D artMask = await LoadArtMaskAsync(race, type); 
        Texture2D frame = await LoadFrameAsync(race, type);
        Texture2D background = await LoadCardBackgroundAsync(race);
        Texture2D back = await LoadCardBackAsync("default");
     
        return new VisualTexture(art, artMask, frame, background, back); 
    }
    
}
