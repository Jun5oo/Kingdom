using UnityEngine;

public class CardFactory 
{
    GameObject cardPrefab;
    
    public void Init(GameObject prefab)
    {
        cardPrefab = prefab; 
    } 
    public Card CreateCard(CardData cardData, int playerID)
    {
        GameObject cardObject = GameObject.Instantiate(cardPrefab); 
        Card card = cardObject.GetComponent<Card>();
        card.Init(cardData, playerID); 
        
        return card;
    }
}
