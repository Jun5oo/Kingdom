using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// CardData를 바탕으로 Card 오브젝트를 비동기 생성하는 팩토리.
/// Addressables로 카드 프리팹과 텍스처를 로드하여 Card와 CardView를 초기화한다.
/// </summary>
public class CardFactory
{
    GameObject cardPrefab;
    CardTextureLoader loader;
    PrefabLoader prefabLoader;

    /// <summary> 카드 프리팹을 Addressables로 비동기 로드하여 캐시한다. </summary>
    public async UniTask Init()
    {
        loader = ServiceLocator.Get<CardTextureLoader>();
        prefabLoader = ServiceLocator.Get<PrefabLoader>();

        cardPrefab = await prefabLoader.LoadPrefabAsync<Card>();

        Debug.Log("CardFactory Initialized");
    }

    /// <summary>
    /// CardData로 Card 오브젝트를 생성한다.
    /// 텍스처 로드 완료 후 CardView를 초기화하며, 생성 중에는 오브젝트를 비활성화 상태로 유지한다.
    /// </summary>
    public async UniTask<Card> CreateCardAsync(CardData cardData, int playerID)
    {
        GameObject cardObject = GameObject.Instantiate(cardPrefab);
        cardObject.gameObject.SetActive(false);

        if (cardObject.TryGetComponent<Card>(out Card card))
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
