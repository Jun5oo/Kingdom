using UnityEngine;

/// <summary>
/// Card와 Token의 공통 기반 클래스.
/// CardData(스탯·종족·이름 등)와 소유자 ID를 보관하고 기본 프로퍼티를 제공한다.
/// </summary>
public abstract class BaseObject : MonoBehaviour
{
    private CardData cardData;
    private int ownerID;

    public virtual void Init(CardData cardData, int playerID)
    {
        this.cardData = cardData;
        this.ownerID = playerID;
    }

    public virtual CardData Data { get { return cardData; } }
    public virtual string Name { get { return cardData.CardName; } }
    public virtual string Description { get { return cardData.Description; } }
    public virtual Race Race { get { return cardData.Race; } }
    public virtual int OwnerID { get { return ownerID; } }
}

