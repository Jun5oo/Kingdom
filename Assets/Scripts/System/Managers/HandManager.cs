using System.Collections.Generic;
using UnityEngine;

public class HandManager : MonoBehaviour
{
    PlayerManager playerManager;
    Dictionary<int, PlayerHand> playerHands; 

    [Header("Local")]
    [SerializeField] Transform localHand; 
    [SerializeField] Transform localHandLeft;
    [SerializeField] Transform localHandRight;
    [SerializeField] Transform localDeck;
    [SerializeField] Transform localCardParent; 

    [Header("Remote")]
    [SerializeField] Transform remoteHand;
    [SerializeField] Transform remoteHandLeft;
    [SerializeField] Transform remoteHandRight;
    [SerializeField] Transform remoteDeck;
    [SerializeField] Transform remoteCardParent;

    // Test 
    /*
    [SerializeField] List<GameObject> localSlot;
    [SerializeField] List<GameObject> remoteSlot;
    List<Card> _localHand;
    List<Card> _remoteHand;

    Card[]__localHand;
    Card[] __remoteHand; 
    */
    [SerializeField] GameObject[] localSlot;
    [SerializeField] GameObject[] remoteSlot;

    Card[] _localHand;
    Card[] _remoteHand;

    public void Init()
    {
        this.playerManager = ServiceLocator.Get<PlayerManager>();

        playerHands = new Dictionary<int, PlayerHand>();

        PlayerHand local = new PlayerHand(playerManager.Local.PlayerID, localHand, localHandLeft, localHandRight, localDeck, localCardParent);
        PlayerHand remote = new PlayerHand(playerManager.Remote.PlayerID, remoteHand, remoteHandLeft, remoteHandRight, remoteDeck, remoteCardParent);
        
        playerHands[playerManager.Local.PlayerID] = local; 
        playerHands[playerManager.Remote.PlayerID] = remote;

        // Test
        /*
        _localHand = new List<Card>(); 
        _remoteHand = new List<Card>();
        */

        _localHand = new Card[8]; 
        _remoteHand = new Card[8];  
    }
    public void AddCardToHand(int playerID, Card card)
    {
        if (!playerHands.ContainsKey(playerID))
        {
            Debug.LogError("Invalid playerID");
            return; 
        }

        card.transform.position = playerHands[playerID].Deck.position;
        card.transform.parent = playerHands[playerID].CardParent.transform;

        card.gameObject.SetActive(true); 

        CardAlignmentOnBoardSide(playerID, card);
  
        /*
        PlayerHand playerHand = playerHands[playerID];
        playerHand.AddCard(card);
        CardAlignment(playerHand.HandCard, playerHand.Hand, playerHand.HandLeft, playerHand.HandRight, playerHand.HandCard.Count, playerID);
        */ 
    }
    public void RemoveCardFromHand(int playerID, Card card)
    {
        if (!playerHands.ContainsKey(playerID))
        {
            Debug.LogError("Invalid playerID");
            return;
        }

        RemoveFromSlot(playerID, card); 
        
        /*
        PlayerHand playerHand = playerHands[playerID];
        playerHand.RemoveCard(card); 

        CardAlignment(playerHand.HandCard, playerHand.Hand, playerHand.HandLeft, playerHand.HandRight, playerHand.HandCard.Count, playerID); 
        */
    }
    public bool IsMyCard(Card card)
    {
        return card.OwnerID == playerManager.Local.PlayerID;
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

        for (int i = 0; i < cardCount; i++)
        {
            float posX = Mathf.Lerp(left.position.x, right.position.x, cardObjLerpX[i]);
            float posY = hand.transform.position.y + 0.01f * i;
            float posZ = hand.transform.position.z + EvaluateCurveValue(height, cardObjLerpX[i]);

            float rotationX = playerID == playerManager.Local.PlayerID ? 90f : -90f;
            float rotationY = Mathf.LerpAngle(left.eulerAngles.y, right.eulerAngles.y, cardObjLerpX[i]);
            float rotationZ = playerID == playerManager.Local.PlayerID ? 0f : 180f; 
            Quaternion rotation = Quaternion.Euler(rotationX, rotationY, rotationZ);
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

    public void CardAlignmentOnBoardSide(int playerID, Card card)
    {
        /*
        List<GameObject> slot = playerID == playerManager.Local.PlayerID ? localSlot : remoteSlot;
        List<Card> hand = playerID == playerManager.Local.PlayerID ? _localHand : _remoteHand;
        */

        GameObject[] slot = playerID == playerManager.Local.PlayerID ? localSlot : remoteSlot;
        Card[] hand = playerID == playerManager.Local.PlayerID ? _localHand : _remoteHand;

        int idx = -1; 

        for(int i=0; i<hand.Length; i++)
        {
            if (hand[i] == null)
            {
                idx = i;
                break; 
            }
        }

        if(idx == -1)
        {
            Debug.Log("카드는 8장을 초과할 수 없습니다.");
            return; 
        }

        Vector3 pos = slot[idx].transform.position;
        float rotationX = 90f;
        float rotationZ = playerID == playerManager.Local.PlayerID ? 0f : 180f;
        Quaternion rotation = Quaternion.Euler(rotationX, 0f, rotationZ);
        Vector3 scale = Vector3.one;

        PRS prs = new PRS(pos, rotation, scale);

        card.GetComponent<CardMovement>().MoveTransform(prs, 0f, false);
        hand[idx] = card; 
    }
    public void RemoveFromSlot(int playerID, Card card)
    {
        Card[] hand = playerID == playerManager.Local.PlayerID ? _localHand : _remoteHand;

        for(int i=0; i< hand.Length; i++)
        {
            if (hand[i] == card)
            {
                hand[i] = null;
                break; 
            }
        }

        Debug.Log("제거하려는 카드를 찾을 수 없습니다. "); 
        // Destroy(card); 
    }
}
