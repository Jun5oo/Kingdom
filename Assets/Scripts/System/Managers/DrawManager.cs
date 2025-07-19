using UnityEngine;

public class DrawManager
{
    PlayerHandManager handManager; 
    DeckManager deckManager; 
    CardFactory cardFactory; 

    public void Init()
    {
        handManager = ServiceLocator.Get<PlayerHandManager>();
        deckManager = ServiceLocator.Get<DeckManager>();
        cardFactory = ServiceLocator.Get<CardFactory>();    
    }
    public Card Draw(int playerID)
    {
        // 랜덤 카드 드로우 
        CardData cardData = deckManager.GetCardData(playerID);
        
        if(cardData == null)
        {
            Debug.Log("덱에서부터 카드를 가져올 수 없습니다."); 
            return null;
        }

        return cardFactory.CreateCard(cardData, playerID); 
    }
    public Card DrawKing(int playerID)
    {
        CardData cardData = deckManager.GetKingCardData(playerID); 

        if(cardData == null)
        {
            Debug.Log("덱에서부터 왕 카드를 가져올 수 없습니다.");
            return null; 
        }

        return cardFactory.CreateCard(cardData, playerID); 
            
    }
}
