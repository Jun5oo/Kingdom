using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

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
