using System;
using System.Collections;
using TMPro;
using UnityEngine;

/// <summary>
/// 화면 알림 UI(승리/패배 등)의 페이드 인·아웃 표시를 관리하는 매니저.
/// 동시에 OnNotification이 두 번 호출되면 기존 코루틴을 중단하고 새 알림으로 교체한다.
/// </summary>
public class UIManager : MonoBehaviour
{
    [Header("Notification UI")]
    [SerializeField] CanvasGroup notificationPanel;
    [SerializeField] TextMeshProUGUI notificationUI;

    #region Notification
    Coroutine notificationRoutine;

    /// <summary>
    /// 알림 메시지를 페이드 인·아웃 애니메이션과 함께 표시한다.
    /// 이미 표시 중인 알림이 있으면 중단하고 새 메시지로 교체한다.
    /// 표시가 완료되면 callback을 호출한다.
    /// </summary>
    public void OnNotification(string message, Action callback = null)
    {
        if (notificationRoutine != null)
        {
            StopCoroutine(notificationRoutine);
            notificationRoutine = null;
        }

        notificationRoutine = StartCoroutine(NotificationRoutine(message, callback));
    }

    /// <summary> 페이드 인(0.3s) → 표시(1.5s) → 페이드 아웃(0.3s) 순으로 알림을 출력한다. </summary>
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

    /// <summary> CanvasGroup의 alpha를 duration 동안 from에서 to로 선형 보간한다. </summary>
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
