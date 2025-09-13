using System.Collections.Generic;
using UnityEngine;

public struct ObjectContext
{
    // 이벤트가 발생했을 때, 해당 이벤트에 관련된 오브젝트에 대한 정보. 

    public BaseObject baseObject;
    public int ownerID; 

    public CardData objectData;
    public Vector2Int gridPosition;

    public CardData parentData;
    public List<CardData> sourceData; 
}
