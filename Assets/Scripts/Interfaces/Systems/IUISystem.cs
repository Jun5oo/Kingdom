using UnityEngine;

public interface IUISystem
{
    public void DisplayUI(Card card);
    public void CloseUI();
    public GameObject Pop<T>() where T : MonoBehaviour, IPoolable;
    public void Push<T>(GameObject gameObject) where T : MonoBehaviour, IPoolable;
    public Transform GetActionUIParent();
}
