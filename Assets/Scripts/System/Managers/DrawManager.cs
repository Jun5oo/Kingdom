using Cysharp.Threading.Tasks;
using UnityEngine;

public class DrawManager
{
    DeckManager deckManager; 
    CardFactory cardFactory; 

    public void Init()
    {
        deckManager = ServiceLocator.Get<DeckManager>();
        cardFactory = ServiceLocator.Get<CardFactory>();    
    }
    public async UniTask<Card> Draw(int playerID)
    {
        Debug.Log($"[DrawManager] Draw({playerID}) 호출됨");
        // 랜덤 카드 드로우 
        CardData cardData = deckManager.GetCardData(playerID);
        
        if(cardData == null)
        {
            Debug.Log("덱에서부터 카드를 가져올 수 없습니다."); 
            return null;
        }

        Card card = await cardFactory.CreateCardAsync(cardData, playerID);
        return card; 
    }
    public async UniTask<Card> DrawKing(int playerID)
    {
        CardData cardData = deckManager.GetKingCardData(playerID); 

        if(cardData == null)
        {
            Debug.Log("덱에서부터 왕 카드를 가져올 수 없습니다.");
            return null; 
        }

        Card card = await cardFactory.CreateCardAsync(cardData, playerID);
        return card; 
    }
}
