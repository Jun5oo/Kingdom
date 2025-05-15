using UnityEngine;
public class ActionSystem : MonoBehaviour
{
    [SerializeField] IAction action;
    [SerializeField] GameObject cardPrefab; 
    [SerializeField] GridSystem gridSystem; 

    private void Start()
    {
        action = new SummonTest(gridSystem, cardPrefab); 
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1))
            CancelAction(); 
    }

    public void OnClick()
    {
        if (!action.IsValid())
        {
            Debug.Log("Action invalid!");
            return; 
        }

        EnterAction(); 
    }

    public void EnterAction()
    {
        action.Enter(); 
    }

    public void CancelAction()
    {
        action.Exit(); 
    }
}
