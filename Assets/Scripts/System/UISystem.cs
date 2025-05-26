using System.Collections.Generic;
using UnityEngine;

public class UISystem : MonoBehaviour, IUISystem
{
    [Header("Card UI")]
    [SerializeField] GameObject cardUI;

    [Header("Action UI")]
    [SerializeField] GameObject actionUIPrefab;
    [SerializeField] GameObject actionUIParent;

    Queue<GameObject> actionUIQueue;

    void Awake()
    {
        actionUIQueue = new Queue<GameObject>(); 
    }

    #region CardUI
    public void DisplayUI()
    {
        cardUI.SetActive(true); 
    }
    public void CloseUI()
    {
        cardUI.SetActive(false); 
    }
    #endregion 

    #region Pooling 
    public GameObject PopActionUI()
    {
        if(actionUIQueue.Count <= 0)
            actionUIQueue.Enqueue(CreateActionUI());

        GameObject button = actionUIQueue.Dequeue();
        button.SetActive(true); 

        return button; 
    }
    public void PushActionUI(GameObject button)
    {
        button.SetActive(false); 
        actionUIQueue.Enqueue(button);
    }
    public GameObject CreateActionUI()
    {
        GameObject obj = Instantiate(actionUIPrefab, Vector3.zero, Quaternion.identity, actionUIParent.transform);
        obj.SetActive(false);

        return obj; 
    }
    #endregion 

    public GameObject GetActionUIParent()
    {
        return actionUIParent; 
    }
}
