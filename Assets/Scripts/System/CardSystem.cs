using System.Collections.Generic;
using UnityEngine;

public class CardSystem : MonoBehaviour, ICardSystem
{
    IGridSystem gridSystem;
    IUISystem uiSystem;
    ISelectionSystem selectionSystem;
    IActionSystem actionSystem;

    // List<Card> deckList; 
    List<Card> handList;
    List<Card> enemyHandList;

    Dictionary<int, PlayerData> players; 

    [SerializeField] GameObject cardPrefab;

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

    const int playerID = 0;
    const int enemyID = 1; 

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            DrawCard(playerID);
        if (Input.GetKeyDown(KeyCode.E))
            DrawCard(enemyID);
    }

    public void Init(IGridSystem gridSystem, IUISystem uiSystem, ISelectionSystem selectionSystem, IActionSystem actionSystem)
    {
        this.gridSystem = gridSystem;
        this.uiSystem = uiSystem;
        this.selectionSystem = selectionSystem;
        this.actionSystem = actionSystem;

        handList = new List<Card>();
       
        players = new Dictionary<int, PlayerData>();

        PlayerData player = new PlayerData(playerID, hand, handLeftTransform, handRightTransform, deckTransform, cardParent);
        PlayerData enemy = new PlayerData(enemyID, enemyHand, enemyHandLeftTransform, enemyHandRightTransform, enemyDeckTransform, enemyCardParent);

        players.Add(playerID, player);
        players.Add(enemyID, enemy); 
    }

    public void DrawCard(int playerID)
    {
        selectionSystem?.OnExitSelected(); 

        GameObject cardObject = CreateCard(playerID); 

        Card card = null;

        if (cardObject.TryGetComponent<Card>(out card))
            AddCardToHand(playerID, card);

    }

    public GameObject CreateCard(int playerID)
    {
        // 현재는 CreateCard는 단순히 CardPrefab만을 생성하지만, 추후에는 deckList로부터 Card 객체를 가져온 후에 저장되어있는 cardData를 가져올 예정 
        CardData cardData = null;

        if (!players.ContainsKey(playerID))
        {
            Debug.LogError("Invalid playerID");
            return null; 
        }

        Transform deckTransform = players[playerID].deck;
        Transform cardParent = players[playerID].cardParent;

        bool isMyCard = playerID == 0 ? true : false; 

        GameObject cardObject = GameObject.Instantiate(cardPrefab, deckTransform.position, Quaternion.identity, cardParent);
        cardObject.name = players[playerID].handList.Count.ToString(); 
        Card card = cardObject?.GetComponent<Card>();
        card.Init(uiSystem, gridSystem, actionSystem, isMyCard, cardData);

        // 여기서 Card에게 Event를 붙여서 카드가 Summon 되면 RemoveCardFromHand를 작동 

        return cardObject; 
    }

    public void AddCardToHand(int playerID, Card card)
    {
        if (!players.ContainsKey(playerID))
        {
            Debug.LogError("Invalid playerID");
            return; 
        }

        PlayerData player = players[playerID];
        player.handList.Add(card);
        
        CardAlignment(ref player.handList, player.hand, player.handLeft, player.handRight, player.handList.Count, playerID);
    }
    public void RemoveCardFromHand(int playerID, Card card)
    {
        if (!players.ContainsKey(playerID))
        {
            Debug.LogError("Invalid playerID");
            return;
        }

        PlayerData player = players[playerID];
        player.handList.Remove(card);

        CardAlignment(ref player.handList, player.hand, player.handLeft, player.handRight, player.handList.Count, playerID); 
    }

    #region CardAlignment 
    void CardAlignment(ref List<Card> handList, Transform hand, Transform left, Transform right, int cardCount, int playerID)
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
            float posY = hand.transform.position.y + (0.01f) * i;
            float posZ = hand.transform.position.z + EvaluateCurveValue(height, cardObjLerpX[i]);

            float rotationX = handList[i].gameObject.transform.rotation.x;
            float rotationY = Mathf.LerpAngle(left.eulerAngles.y, right.eulerAngles.y, cardObjLerpX[i]);
            float rotationZ = 180f;

            Quaternion rotation = Quaternion.Euler(rotationX, rotationY, rotationZ);
            handList[i].gameObject.GetComponent<CardMovement>().MoveTransform(new PRS(new Vector3(posX, posY, posZ), rotation, Vector3.one), 0.5f);
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
    #endregion 
}
