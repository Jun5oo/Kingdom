using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData
{
    public int playerID;

    public List<Card> handList;
    public List<Card> deckList;

    public Transform hand;
    public Transform handLeft;
    public Transform handRight;
    public Transform deck;

    public Transform cardParent;

    public Vector2Int kingPos; 

    public PlayerData(int playerID, Transform hand, Transform handLeft, Transform handRight, Transform deck, Transform cardParent)
    {
        this.playerID = playerID;
        this.hand = hand;
        this.handLeft = handLeft;
        this.handRight = handRight;
        this.deck = deck;

        this.cardParent = cardParent;

        handList = new List<Card>();
        deckList = new List<Card>();

        kingPos = -Vector2Int.one; 
    }
}
