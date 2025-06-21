using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// UI를 관리하는 System 클래스 
/// </summary>

public class UISystem : MonoBehaviour, IUISystem
{
    Dictionary<Type, PoolInfo> poolDictionary;

    [Header("Card UI")]
    [SerializeField] CardUI cardUI;

    [Header("Action UI")]
    [SerializeField] GameObject actionUIPrefab;
    [SerializeField] Transform actionUIParent;

    [Header("Card Status UI")]
    [SerializeField] GameObject cardStatusUI;
    [SerializeField] Transform cardStatusUIParent;

    [Header("Damage Popup UI")]
    [SerializeField] GameObject damagePopupUI;
    [SerializeField] Transform damagePopupParent;

    [Header("PlayerHUD")]
    [SerializeField] PlayerHUD player1HUD;
    [SerializeField] PlayerHUD player2HUD; 

    void Awake()
    {
        poolDictionary = new Dictionary<Type, PoolInfo>();

        RegisterPool<ActionUI>(actionUIPrefab, actionUIParent);
        RegisterPool<CardStatusUI>(cardStatusUI, cardStatusUIParent);
        RegisterPool<DamagePopupUI>(damagePopupUI, damagePopupParent); 
    }

    #region CardUI
    public void DisplayUI(Card card)
    {
        cardUI.OnUpdate(card); 
        cardUI.gameObject.SetActive(true);
    }
    public void CloseUI()
    {
        cardUI.gameObject.SetActive(false); 
    }
    #endregion 

    public Transform GetActionUIParent()
    {
        return actionUIParent; 
    }

    #region Pooling 
    public void RegisterPool<T>(GameObject prefab, Transform parent) where T: MonoBehaviour, IPoolable
    {
        poolDictionary[typeof(T)] = new PoolInfo(prefab, parent); 
    }

    public GameObject Pop<T>() where T: MonoBehaviour, IPoolable
    {
        if (poolDictionary.TryGetValue(typeof(T), out PoolInfo poolInfo))
        {
            GameObject obj = null; 

            if (poolInfo.pool.Count == 0)
                obj = GameObject.Instantiate(poolInfo.prefab, poolInfo.parent);
      
            if (obj == null)
                obj = poolInfo.pool.Dequeue();

            obj.SetActive(true);

            return obj; 
        }

        Debug.LogError($"No pool register for type {typeof(T)}");
        return null; 
    }

    public void Push<T>(GameObject gameObject) where T: MonoBehaviour, IPoolable
    {
        if(poolDictionary.TryGetValue(typeof(T), out PoolInfo poolInfo))
        {
            gameObject.SetActive(false);
            poolInfo.pool.Enqueue(gameObject); 
        }

        else
            Debug.LogError($"No pool registered for type {typeof(T)}"); 
    }
    #endregion 

    public void SetHUD()
    {
        // Temp 
        CardSystem cardSystem = FindAnyObjectByType<CardSystem>();
        Card player1King = cardSystem.GetPlayerKing(0); 
        Card player2King = cardSystem.GetPlayerKing(1);

        player1HUD.Init(true, player1King.CP);
        player2HUD.Init(false, player2King.CP);

        player1HUD.gameObject.SetActive(true);
        player2HUD.gameObject.SetActive(true); 
    }

    public void OnUpdateHUD(int playerID, int damage)
    {
        PlayerHUD hud = playerID == 0 ? player1HUD: player2HUD;
        hud.OnUpdateCP(damage);
    }
}
