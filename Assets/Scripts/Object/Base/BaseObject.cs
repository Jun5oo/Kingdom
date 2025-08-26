using UnityEngine;

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

