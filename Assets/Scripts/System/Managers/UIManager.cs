using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("Notification UI")]
    [SerializeField] CanvasGroup notificationPanel;
    [SerializeField] TextMeshProUGUI notificationUI;


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
