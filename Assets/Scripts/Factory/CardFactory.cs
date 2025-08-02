using Cysharp.Threading.Tasks;
using UnityEngine;

public class CardFactory 
{
    GameObject cardPrefab;
    CardTextureLoader loader;
    PrefabLoader prefabLoader; 

    public async UniTask Init()
    {
        loader = ServiceLocator.Get<CardTextureLoader>();
        prefabLoader = ServiceLocator.Get<PrefabLoader>();

        cardPrefab = await prefabLoader.LoadPrefabAsync<Card>();

        Debug.Log("CardFactory Initialized");
    }
    public async UniTask<Card> CreateCardAsync(CardData cardData, int playerID)
    {
        GameObject cardObject = GameObject.Instantiate(cardPrefab);
        cardObject.gameObject.SetActive(false); 

        if(cardObject.TryGetComponent<Card>(out Card card))
        {
            card.Init(cardData, playerID);

            VisualTexture textures = await loader.LoadAllTextures(cardData);
            Debug.Log("CardTexture Load Complete");

            if (card.TryGetComponent<CardView>(out CardView cardView))
                cardView.Init(textures);
        }

        else
            Debug.LogError("Cannot found Card component from Card prefab");

        return card;
    }

}
