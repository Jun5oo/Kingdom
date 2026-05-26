using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이어 손패(핸드)의 카드 배치와 슬롯 관리를 담당하는 매니저.
/// 최대 8슬롯 고정 배열(_localHand / _remoteHand)로 카드를 관리하며,
/// 슬롯 인덱스에 해당하는 Transform 위치로 카드를 이동시킨다.
/// </summary>
public class HandManager : MonoBehaviour
{
    PlayerManager playerManager;
    Dictionary<int, PlayerHand> playerHands; // 플레이어 ID → PlayerHand 매핑

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

    [SerializeField] GameObject[] localSlot;  // 로컬 플레이어 핸드 슬롯 Transform 배열 (최대 8)
    [SerializeField] GameObject[] remoteSlot; // 원격 플레이어 핸드 슬롯 Transform 배열 (최대 8)

    Card[] _localHand;  // 로컬 플레이어 핸드 배열 (null = 빈 슬롯)
    Card[] _remoteHand; // 원격 플레이어 핸드 배열 (null = 빈 슬롯)

    /// <summary> PlayerHand 인스턴스를 생성하고 핸드 배열을 초기화한다. </summary>
    public void Init()
    {
        this.playerManager = ServiceLocator.Get<PlayerManager>();

        playerHands = new Dictionary<int, PlayerHand>();

        PlayerHand local = new PlayerHand(playerManager.Local.PlayerID, localHand, localHandLeft, localHandRight, localDeck, localCardParent);
        PlayerHand remote = new PlayerHand(playerManager.Remote.PlayerID, remoteHand, remoteHandLeft, remoteHandRight, remoteDeck, remoteCardParent);

        playerHands[playerManager.Local.PlayerID] = local;
        playerHands[playerManager.Remote.PlayerID] = remote;

        _localHand = new Card[8];
        _remoteHand = new Card[8];
    }

    /// <summary> 카드를 해당 플레이어의 빈 슬롯에 배치하고 슬롯 위치로 이동시킨다. </summary>
    public void AddCardToHand(int playerID, Card card)
    {
        if (!playerHands.ContainsKey(playerID))
        {
            Debug.LogError("Invalid playerID");
            return; 
        }

        // card.transform.position = playerHands[playerID].Deck.position;
        card.transform.parent = playerHands[playerID].CardParent.transform;

        // card.gameObject.SetActive(true); 

        CardAlignmentOnBoardSide(playerID, card);
    }

    /// <summary> 카드를 핸드 슬롯 배열에서 제거한다(슬롯을 null로 비운다). </summary>
    public void RemoveCardFromHand(int playerID, Card card)
    {
        if (!playerHands.ContainsKey(playerID))
        {
            Debug.LogError("Invalid playerID");
            return;
        }

        RemoveFromSlot(playerID, card);
    }

    /// <summary> 해당 카드가 로컬 플레이어 소유인지 반환한다. </summary>
    public bool IsMyCard(Card card)
    {
        return card.OwnerID == playerManager.Local.PlayerID;
    }

    /// <summary> 해당 플레이어의 핸드 배열(null 포함 고정 크기 8)을 반환한다. </summary>
    public Card[] GetHandCards(int playerID)
    {
        if (!playerHands.ContainsKey(playerID))
        {
            Debug.LogError("Invalid playerID");
            return null; 
        }

        return playerID == playerManager.Local.PlayerID ? _localHand : _remoteHand;
    }

    /// <summary> null 슬롯을 제외한 실제 카드 목록을 List로 반환한다. </summary>
    public List<Card> GetHandCardsList(int playerID)
    {
        if (!playerHands.ContainsKey(playerID))
        {
            Debug.LogError("Invalid playerID");
            return null;
        }

        Card[] handCards = GetHandCards(playerID);
        List<Card> handList = new List<Card>();

        foreach (var card in handCards)
        {
            if (card != null)
                handList.Add(card);
        }

        return handList;
    }

    #region CardAlignment
    /// <summary>
    /// 핸드의 카드를 left~right 범위 내에서 균등하게 배치한다.
    /// 카드 수에 따라 lerp 비율을 계산하고 곡선 높이(EvaluateCurveValue)를 적용한다.
    /// </summary>
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
    /// <summary> 0→height→0 포물선 커브를 만들어 lerpValue 위치의 Z 오프셋을 반환한다. </summary>
    float EvaluateCurveValue(float height, float lerpValue)
    {
        AnimationCurve curve = new AnimationCurve();

        curve.AddKey(0, 0);
        curve.AddKey(0.5f, height);
        curve.AddKey(1, 0);

        return curve.Evaluate(lerpValue);
    }
    #endregion

    /// <summary>
    /// 카드를 핸드 배열에서 첫 번째 빈 슬롯에 등록하고 해당 슬롯 위치로 이동시킨다.
    /// 슬롯이 모두 찼으면(8장 초과) 경고 후 무시한다.
    /// </summary>
    public void CardAlignmentOnBoardSide(int playerID, Card card)
    {
        if (card == null)
            return; 

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
        float rotationX = playerID == playerManager.Local.PlayerID ? 90f : -90f;
        float rotationZ = playerID == playerManager.Local.PlayerID ? 0f : 180f;
        Quaternion rotation = Quaternion.Euler(rotationX, 0f, rotationZ);
        Vector3 scale = Vector3.one;

        PRS prs = new PRS(pos, rotation, scale);

        card.GetComponent<CardMovement>().MoveTransform(prs, 0f, false, () => { card.gameObject.SetActive(true); });
        hand[idx] = card;
    }

    /// <summary> 핸드 배열에서 해당 카드를 찾아 null로 비운다. </summary>
    public void RemoveFromSlot(int playerID, Card card)
    {
        Card[] hand = playerID == playerManager.Local.PlayerID ? _localHand : _remoteHand;

        if(hand == null)
        {
            Debug.LogWarning($"{playerID}의 패를 찾을 수 없습니다.");
            return; 
        }

        for(int i=0; i< hand.Length; i++)
        {
            if (hand[i] == card)
            {
                hand[i] = null;
                break; 
            }
        }
    }
}
