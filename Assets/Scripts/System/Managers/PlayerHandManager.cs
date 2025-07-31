using System.Collections.Generic;
using UnityEngine;

public class PlayerHandManager : MonoBehaviour
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

    public void Init()
    {
        this.playerManager = ServiceLocator.Get<PlayerManager>();

        playerHands = new Dictionary<int, PlayerHand>();

        PlayerHand local = new PlayerHand(playerManager.Local.PlayerID, localHand, localHandLeft, localHandRight, localDeck, localCardParent);
        PlayerHand remote = new PlayerHand(playerManager.Remote.PlayerID, remoteHand, remoteHandLeft, remoteHandRight, remoteDeck, remoteCardParent);
        
        playerHands[playerManager.Local.PlayerID] = local; 
        playerHands[playerManager.Remote.PlayerID] = remote;
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

        PlayerHand playerHand = playerHands[playerID];
        playerHand.AddCard(card);
        CardAlignment(playerHand.HandCard, playerHand.Hand, playerHand.HandLeft, playerHand.HandRight, playerHand.HandCard.Count, playerID);
    }
    public void RemoveCardFromHand(int playerID, Card card)
    {
        if (!playerHands.ContainsKey(playerID))
        {
            Debug.LogError("Invalid playerID");
            return;
        }

        PlayerHand playerHand = playerHands[playerID];
        playerHand.RemoveCard(card); 

        CardAlignment(playerHand.HandCard, playerHand.Hand, playerHand.HandLeft, playerHand.HandRight, playerHand.HandCard.Count, playerID); 
    }
    public bool IsMyCard(Card card)
    {
        return card.OwnerPlayerID == playerManager.Local.PlayerID;
    }

    public List<Card> GetHandCards(int playerID)
    {
        if (!playerHands.ContainsKey(playerID))
        {
            Debug.LogError("Invalid playerID");
            return null; 
        }

        return playerHands[playerID].HandCard;
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
}
