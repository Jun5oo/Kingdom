
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseObject : MonoBehaviour 
{
    private CardData cardData;

    public virtual void Init(CardData cardData)
    {
        this.cardData = cardData; 
    }

    public virtual CardData Data { get { return cardData; } }
    public virtual int ID { get { return cardData.ID; } }
    public virtual string Name { get { return cardData.Name; } }
    public virtual string Description { get { return cardData.Description; } }
    public virtual Race Race { get { return cardData.Race; } }
    public virtual List<ActionType> Actions { get { return cardData.Actions; } }

    public abstract int OwnerID { get; } 
}

