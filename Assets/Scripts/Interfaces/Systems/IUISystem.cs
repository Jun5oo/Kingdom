using UnityEngine;

public interface IUISystem
{
    public void DisplayUI();
    public void CloseUI();

    public void PushActionUI(GameObject obj);
    public GameObject PopActionUI();
    public GameObject GetActionUIParent(); 
}
