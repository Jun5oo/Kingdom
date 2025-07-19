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
        // 카드를 생성 
        GameObject cardObject = GameObject.Instantiate(cardPrefab);
        cardObject.gameObject.SetActive(false); 

        Card card = cardObject.GetComponent<Card>();
        card.Init(cardData, playerID); 
        
        return card;
    }
}
