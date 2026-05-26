using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이어 핸드의 Transform 앵커와 카드 목록을 보관하는 데이터 클래스.
/// HandManager에서 생성하며 카드 배치 계산에 사용하는 기준 Transform을 제공한다.
/// </summary>
public class PlayerHand
{
    private int playerID;

    private Transform hand;        // 핸드 중심 Transform
    private Transform handLeft;    // 핸드 왼쪽 경계
    private Transform handRight;   // 핸드 오른쪽 경계
    private Transform deck;        // 덱 위치
    private Transform cardParent;  // 카드 오브젝트의 부모 Transform
    private List<Card> handCard;   // 현재 핸드 카드 목록 (현재는 슬롯 배열로 대체됨)

    public Transform Hand { get { return hand; } }
    public Transform HandLeft { get { return handLeft; } }
    public Transform HandRight { get { return handRight; } }
    public Transform Deck { get { return deck; } }
    public Transform CardParent { get { return cardParent; } }
    public List<Card> HandCard { get { return handCard; } }

    public PlayerHand(int playerID, Transform hand, Transform handLeft, Transform handRight, Transform deck, Transform cardParent)
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
