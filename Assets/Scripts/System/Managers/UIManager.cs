using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    Dictionary<Type, PoolData> poolDictionary;

    [Header("Card UI")]
    [SerializeField] PreviewUI previewUI;

    [Header("Action UI")]
    [SerializeField] GameObject actionUIPrefab;
    [SerializeField] Transform actionUILayout;
    public Transform ActionUILayout { get { return actionUILayout; } }

    [Header("Damage Popup UI")]
    [SerializeField] GameObject damagePopupUI;
    [SerializeField] Transform damagePopupParent;

    [Header("PlayerHUD")]
    [SerializeField] PlayerHUD localHUD;
    [SerializeField] PlayerHUD remoteHUD;

    [Header("Notification UI")]
    [SerializeField] CanvasGroup notificationPanel;
    [SerializeField] TextMeshProUGUI notificationUI;

    void Awake()
    {
        poolDictionary = new Dictionary<Type, PoolData>();

        RegisterPool<ActionUI>(actionUIPrefab, actionUILayout);
        RegisterPool<DamagePopupUI>(damagePopupUI, damagePopupParent);
    }

    #region CardUI
    public void DisplayUI(Entity entity)
    {
        previewUI.OnUpdate(entity); 
        previewUI.gameObject.SetActive(true);
    }
    public void CloseUI() => previewUI.gameObject.SetActive(false); 
    #endregion 

    #region Pooling 
    public void RegisterPool<T>(GameObject prefab, Transform parent) where T: MonoBehaviour, IPoolable
    {
        poolDictionary[typeof(T)] = new PoolData(prefab, parent); 
    }
    public GameObject Pop<T>() where T: MonoBehaviour, IPoolable
    {
        if (poolDictionary.TryGetValue(typeof(T), out PoolData poolInfo))
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
        if(poolDictionary.TryGetValue(typeof(T), out PoolData poolInfo))
        {
            gameObject.SetActive(false);
            poolInfo.pool.Enqueue(gameObject); 
        }

        else
            Debug.LogError($"No pool registered for type {typeof(T)}"); 
    }
    #endregion

    #region DamagePopup
    public void ShowDamagePopup(int damage, Vector3 position, bool flag = false)
    {
        StartCoroutine(DamagePopup(damage, position)); 
    }
    IEnumerator DamagePopup(int damage, Vector3 position)
    {
        GameObject damagePopup = Pop<DamagePopupUI>();
        damagePopup.transform.SetParent(damagePopupParent, false);
        damagePopup.transform.position = Camera.main.WorldToScreenPoint(position);

        damagePopup.GetComponent<DamagePopupUI>().Init(damage);

        yield return new WaitForSeconds(2f);

        Push<DamagePopupUI>(damagePopup); 
    }
    #endregion

    #region HUD 
    public void SetHUD(Player playerData, Token kingToken)
    {
        PlayerHUD hud = null;

        if (playerData.IsLocal)
            hud = localHUD;
        else
            hud = remoteHUD;

        hud.Init(playerData, kingToken); 
        
    }
    public void OnActiveHUD()
    {
        localHUD.gameObject.SetActive(true);
        remoteHUD.gameObject.SetActive(true);
    }
    #endregion

    #region Notification
    Coroutine notificationRoutine; 
    
    public void OnNotification(string message, Action callback = null)
    {
        if (notificationRoutine != null)
        {
            StopCoroutine(notificationRoutine); 
            notificationRoutine = null;
        }

        notificationRoutine = StartCoroutine(NotificationRoutine(message, callback)); 
    }

    IEnumerator NotificationRoutine(string message, Action callback = null)
    {
        float fadeDuration = 0.3f;
        float displayDuration = 1.5f;

        notificationUI.text = message;

        notificationPanel.alpha = 0f; 
        notificationPanel.gameObject.SetActive(true);

        yield return FadeCanvasGroup(notificationPanel, 0f, 1f, fadeDuration);
        yield return new WaitForSeconds(displayDuration);
        yield return FadeCanvasGroup(notificationPanel, 1f, 0f, fadeDuration);
        
        notificationPanel.gameObject.SetActive(false);
        callback?.Invoke(); 
    }
    #endregion

    IEnumerator FadeCanvasGroup(CanvasGroup canvasGroup, float from, float to, float duration)
    {
        float time = 0f; 
        
        while(time < duration)
        {
            time += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(from, to, time / duration);
            yield return null; 
        }

        canvasGroup.alpha = to; 
    }
}
