using UnityEngine;

public enum CardState
{
    Idle, 
    Hovered, 
    Selected
}

public class Card : MonoBehaviour
{
    CardData cardData;
    CardState cardState; 

    public void Init(CardData cardData)
    {
        this.cardData = cardData;
        this.cardState = CardState.Idle; 
    }
}
