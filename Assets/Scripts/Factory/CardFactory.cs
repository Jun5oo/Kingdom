using Cysharp.Threading.Tasks;
using UnityEngine;

public class CardFactory 
{
    GameObject cardPrefab;
    TextureLoader textureLoader;

    public void Init(GameObject prefab)
    {
        cardPrefab = prefab; 
        textureLoader = ServiceLocator.Get<TextureLoader>();  
    } 
    public async UniTask<Card> CreateCardAsync(CardData cardData, int playerID)
    {
        GameObject cardObject = GameObject.Instantiate(cardPrefab);
        cardObject.gameObject.SetActive(false); 

        Card card = cardObject.GetComponent<Card>();
        card.Init(cardData, playerID);

        VisualTexture textures = await textureLoader.LoadAllTextures(cardData); 

        if(card.TryGetComponent<CardView>(out CardView cardView))
            cardView.Init(textures); 

        return card;
    }

}
