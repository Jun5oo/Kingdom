using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DrawTest : MonoBehaviour
{
    [SerializeField] GameObject card;
    [SerializeField] GameObject deck;

    [SerializeField] GameObject center; 
    [SerializeField] GameObject left;
    [SerializeField] GameObject right;

    [SerializeField] Transform parent; 

    List<GameObject> myHandList;

    void Awake()
    {
        myHandList = new List<GameObject>(); 
    }

    public void OnClick()
    {
        GameObject newlyCreated = OnCreateCard();
        OnDrawCard(newlyCreated);
    }

    GameObject OnCreateCard()
    {
        GameObject cardObj = GameObject.Instantiate(card);
        cardObj.transform.position = deck.transform.position;
        cardObj.transform.parent = parent; 
        return cardObj; 
        
    }
    
    void OnDrawCard(GameObject card)
    {
        myHandList.Add(card);
        CardAlignment(ref myHandList, left.transform, right.transform, myHandList.Count); 
    }

    void CardAlignment(ref List<GameObject> handList, Transform left, Transform right, int cardCount)
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
                for(int i=0; i<cardCount; i++)
                    cardObjLerpX[i] = (i+1) * interval; 
                break; 
        }

        for(int i=0; i < cardCount; i++)
        {
            float posX = Mathf.Lerp(left.position.x, right.position.x, cardObjLerpX[i]);
            float posY = center.transform.position.y + (i * 0.01f);
            float posZ = center.transform.position.z + EvaluateCurveValue(0.5f, cardObjLerpX[i]);

            float rotationX = handList[i].transform.rotation.x; 
            float rotationY = Mathf.LerpAngle(left.eulerAngles.y, right.eulerAngles.y, cardObjLerpX[i]);
            float rotationZ = 180f;

            Quaternion rotation = Quaternion.Euler(rotationX, rotationY, rotationZ); 
            handList[i].GetComponent<CardMovement>().MoveTransform(new PRS(new Vector3(posX, posY, posZ), rotation, Vector3.one), 0.5f);
        }
    }

    float EvaluateCurveValue(float height, float lerpValue)
    {
        // x가 0부터 1이고 높이가 0.5인 곡선 
        AnimationCurve curve = new AnimationCurve();
        
        curve.AddKey(0, 0);
        curve.AddKey(0.5f, height);
        curve.AddKey(1, 0);

        return curve.Evaluate(lerpValue); 
    }
}
