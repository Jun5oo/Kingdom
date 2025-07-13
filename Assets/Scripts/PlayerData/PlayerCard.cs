using System.Collections.Generic;
using UnityEngine;

public class PlayerCard 
{
    private int playerID;

    private Transform hand;
    private Transform handLeft; 
    private Transform handRight;
    private Transform deck;
    private Transform cardParent;
    
    private List<Card> handCard;
    
    public Transform Hand {  get { return hand; } }
    public Transform HandLeft { get { return handLeft; } }
    public Transform HandRight { get { return handRight; } }
    public Transform Deck { get { return deck; } }
    public Transform CardParent {  get { return cardParent; } } 
    public List<Card> HandCard { get {  return handCard; } }


    public void Init(int playerID, Transform hand, Transform handLeft, Transform handRight, Transform deck, Transform cardParent)
    {
        this.playerID = playerID;
        this.hand = hand;
        this.handLeft = handLeft;
        this.handRight = handRight;
        this.deck = deck;
        this.cardParent = cardParent;

        this.handCard = new List<Card>();
    }

    public void AddCard(Card card)
    {
        if (card == null)
            return; 

        handCard.Add(card);
    }

    public void RemoveCard(Card card)
    {
        handCard.Remove(card);
    }
}
