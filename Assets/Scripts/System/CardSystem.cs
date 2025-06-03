using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 카드를 관리하는 시스템 클래스 
/// </summary>

public class CardSystem : MonoBehaviour, ICardSystem
{
    IGridSystem gridSystem;
    IUISystem uiSystem;
    ISelectionSystem selectionSystem;
    IActionSystem actionSystem;
 
    List<Card> handList;
    List<Card> enemyHandList;

    // Test 
    [SerializeField] List<CardData> deckList;

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

    private Card playerKing;
    private Card enemyKing;

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
        // 현재는 테스트 용으로 deckList에 세 가지 종류의 카드만을 넣고 랜덤 생성. 나중에는 자신이 구성한 덱의 카드 데이터들을 가지고 순차적으로 생성예정 (셔플 함수도 구현예정) 
        CardData cardData = deckList[Random.Range(0, deckList.Count)]; 

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
        card.GetComponent<CardView>().Init(card); 

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
        float heightBuffer = playerID == 0 ? 0.3f : 0.1f; 

        for (int i = 0; i < cardCount; i++)
        {
            float posX = Mathf.Lerp(left.position.x, right.position.x, cardObjLerpX[i]);
            float posY = hand.transform.position.y + (heightBuffer) * i;
            float posZ = hand.transform.position.z + EvaluateCurveValue(height, cardObjLerpX[i]);

            float rotationX = handList[i].gameObject.transform.rotation.x;
            float rotationY = Mathf.LerpAngle(left.eulerAngles.y, right.eulerAngles.y, cardObjLerpX[i]);
            float rotationZ = playerID == 0 ? 180f : 0f; 
            Quaternion rotation = Quaternion.Euler(rotationX, rotationY, rotationZ);

            Vector3 scale = playerID == 0 ? Vector3.one : Vector3.one * 2; 

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

    #region Creation King 
    [SerializeField] CardData undeadKing;
    [SerializeField] CardData angelKing; 

    private Card CreateKing(int playerID)
    {
        GameObject kingObj = Instantiate(cardPrefab, Vector3.zero, Quaternion.identity, players[playerID].cardParent);
        kingObj.SetActive(false); 
        kingObj.tag = "King";
        Card card = kingObj.GetComponent<Card>();
        CardData kingData = playerID == 0 ? undeadKing : angelKing; 

        bool isMyCard = playerID == 0;

        card.Init(uiSystem, gridSystem, actionSystem, isMyCard, kingData);

        return card;
    }

    public void SummonKing(Card kingCard)
    {
        if (kingCard == null)
        {
            Debug.LogError("King card is not assigned.");
            return;
        }

        KingSummonAction summonAction = new KingSummonAction(gridSystem, actionSystem, kingCard);

        if (summonAction.IsValid())
        {
            actionSystem.EnterAction(summonAction);
        }
    }
    #endregion 

    public Card GetPlayerKing(int playerID)
    {
        return playerID == 0 ? playerKing : enemyKing;
    }
}
