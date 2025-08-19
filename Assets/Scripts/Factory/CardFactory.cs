using Cysharp.Threading.Tasks;
using UnityEngine;

public class CardFactory 
{
    // 카드데이터를 바탕으로 카드오브젝트를 생성하는 클래스 
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

            if (card.TryGetComponent<CardView>(out CardView cardView))
                cardView.Init(textures, cardData);
        }

        else
            Debug.LogError("Cannot found Card component from Card prefab");

        return card;
    }

}
