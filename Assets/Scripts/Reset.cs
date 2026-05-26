using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

/// <summary>
/// Title 씬으로 돌아가는 리셋 버튼 컴포넌트. 호버 시 1.1× 스케일 애니메이션을 적용한다.
/// </summary>
public class Reset : MonoBehaviour, IPointerExitHandler, IPointerEnterHandler
{
    const string Title_SCENE = "Title";
    public void ResetGame()
    {
        SceneManager.LoadScene(Title_SCENE); 
    }

    public void OnPointerEnter(PointerEventData eventData) => transform.localScale = Vector3.one * 1.1f;
    public void OnPointerExit(PointerEventData eventData) => transform.localScale = Vector3.one;

}
