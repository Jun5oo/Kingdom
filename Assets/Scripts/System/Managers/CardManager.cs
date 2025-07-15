using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    CardFactory cardFactory; 

    Dictionary<Race, CardData> kingCards;
    Dictionary<Race, List<CardData>> RaceDecks;

    [SerializeField] CardData undeadKing;
    [SerializeField] CardData celestialKing; 
    
    [SerializeField] List<CardData> undeadDeck;
    [SerializeField] List<CardData> celestialDeck;

    [SerializeField] GameObject cardPrefab;

    PlayerManager playerManager;
    
    int localPlayerID;
    int remotePlayerID;

    List<CardData> localPlayerDeck;
    List<CardData> remotePlayerDeck;

    Dictionary<int, PlayerCard> playerCards; 

    [Header("Player")]
    [SerializeField] Transform hand; 
    [SerializeField] Transform handLeftTransform;
    [SerializeField] Transform handRightTransform;
    [SerializeField] Transform deckTransform;
    [SerializeField] Transform cardParent;

    [Header("Enemy")]
    [SerializeField] Transform enemyHand;
    [SerializeField] Transform enemyHandLeftTransform;
    [SerializeField] Transform enemyHandRightTransform;
    [SerializeField] Transform enemyDeckTransform;
    [SerializeField] Transform enemyCardParent; 

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
            DrawCard(localPlayerID);
        if (Input.GetKeyDown(KeyCode.S))
            DrawCard(remotePlayerID); 
    }

    public void Init(PlayerManager playerManager, CardFactory cardFactory)
    {
        this.playerManager = playerManager; 
        this.cardFactory = cardFactory; 

        kingCards = new Dictionary<Race, CardData>();
        kingCards[Race.Celestial] = celestialKing;
        kingCards[Race.Undead] = undeadKing;

        RaceDecks = new Dictionary<Race, List<CardData>>(); 
        RaceDecks[Race.Celestial] = celestialDeck;
        RaceDecks[Race.Undead] = undeadDeck; 

        localPlayerID = playerManager.LocalPlayerData.PlayerID;
        remotePlayerID = playerManager.RemotePlayerData.PlayerID;

        PlayerCard localCard = new PlayerCard(); 
        PlayerCard remoteCard = new PlayerCard();

        localCard.Init(localPlayerID, hand, handLeftTransform, handRightTransform, deckTransform, cardParent);
        remoteCard.Init(remotePlayerID, enemyHand, enemyHandLeftTransform, enemyHandRightTransform, enemyDeckTransform, enemyCardParent); 

        playerCards = new Dictionary<int, PlayerCard>();
        playerCards[localPlayerID] = localCard;
        playerCards[remotePlayerID] = remoteCard;
    }
    public void DrawCard(int playerID)
    {
        Card card = CreateCard(playerID); 

        if (card != null)
            AddCardToHand(playerID, card);
    }
    public void DrawCard(int playerID, Card card)
    {
        if (card != null)
            AddCardToHand(playerID, card); 
    }
    public Card DrawKingCard(int playerID)
    {
        Card card = CreateKingCard(playerID);

        if (card != null)
            AddCardToHand(playerID, card);

        return card; 
    }
    private Card CreateCard(int playerID)
    {
        if (!playerManager.PlayerDict.ContainsKey(playerID))
        {
            Debug.LogError("Invalid playerID");
            return null; 
        }

        Race race = playerManager.PlayerDict[playerID].Race;
        CardData cardData = RaceDecks[race][Random.Range(0, RaceDecks[race].Count)]; 
        Card card = cardFactory.CreateCard(cardData, playerID);

        return card; 
    }
    private Card CreateKingCard(int playerID)
    {
        Race race = playerManager.PlayerDict[playerID].Race; 
        Card card = cardFactory.CreateCard(kingCards[race], playerID);
        return card; 
    }
    public void AddCardToHand(int playerID, Card card)
    {
        if (!playerCards.ContainsKey(playerID))
        {
            Debug.LogError("Invalid playerID");
            return; 
        }

        PlayerCard playerCard = playerCards[playerID];
        playerCard.AddCard(card);

        card.transform.position = playerCard.Deck.position;
        card.transform.parent = playerCard.CardParent; 

        CardAlignment(playerCard.HandCard, playerCard.Hand, playerCard.HandLeft, playerCard.HandRight, playerCard.HandCard.Count, playerID);
    }
    public void RemoveCardFromHand(int playerID, Card card)
    {
        if (!playerCards.ContainsKey(playerID))
        {
            Debug.LogError("Invalid playerID");
            return;
        }

        PlayerCard playerCard = playerCards[playerID];
        playerCard.RemoveCard(card); 

        CardAlignment(playerCard.HandCard, playerCard.Hand, playerCard.HandLeft, playerCard.HandRight, playerCard.HandCard.Count, playerID); 
    }
    public bool IsMyCard(Card card)
    {
        return card.OwnerPlayerID == playerManager.LocalPlayerData.PlayerID;
    }

    #region CardAlignment 
    void CardAlignment(List<Card> handList, Transform hand, Transform left, Transform right, int cardCount, int playerID)
    {
        float[] cardObjLerpX = new float[cardCount];

        switch (cardCount)
        {
            case 1:
                cardObjLerpX = new float[] { 0.5f };
                break;
            case 2:
                cardObjLerpX = new float[] { 0.4f, 0.6f };
                break;
            case 3:
                cardObjLerpX = new float[] { 0.3f, 0.5f, 0.7f };
                break;
            default:
                float interval = 1f / (cardCount + 1);
                for (int i = 0; i < cardCount; i++)
                    cardObjLerpX[i] = (i + 1) * interval;
                break;
        }
        
        float height = playerID == 0 ? 0.5f : -0.5f;
        float heightBuffer = playerID == 0 ? 0.3f : 0.1f; 

        for (int i = 0; i < cardCount; i++)
        {
            float posX = Mathf.Lerp(left.position.x, right.position.x, cardObjLerpX[i]);
            float posY = hand.transform.position.y + (heightBuffer) * i;
            float posZ = hand.transform.position.z + EvaluateCurveValue(height, cardObjLerpX[i]);

            float rotationX = playerID == playerManager.LocalPlayerData.PlayerID ? 90f : -90f;
            float rotationY = Mathf.LerpAngle(left.eulerAngles.y, right.eulerAngles.y, cardObjLerpX[i]);
            float rotationZ = playerID == playerManager.LocalPlayerData.PlayerID ? 0f : 180f; 
            Quaternion rotation = Quaternion.Euler(rotationX, rotationY, rotationZ);
            //Vector3 scale = playerID == playerManager.LocalPlayerData.PlayerID ? Vector3.one : Vector3.one * 2; 
            Vector3 scale = Vector3.one;
            handList[i].gameObject.GetComponent<CardMovement>().MoveTransform(new PRS(new Vector3(posX, posY, posZ), rotation, scale), 0.5f);
        }
    }
    float EvaluateCurveValue(float height, float lerpValue)
    {
        // x가 0~1, 높이가 0.5 
        AnimationCurve curve = new AnimationCurve();

        curve.AddKey(0, 0);
        curve.AddKey(0.5f, height);
        curve.AddKey(1, 0);

        return curve.Evaluate(lerpValue);
    }
    #endregion
}
