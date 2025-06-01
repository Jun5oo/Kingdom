using UnityEngine;

/// <summary>
/// System에 필요한 객체를 주입하는 클래스 
/// </summary>

public class DependencyInjector : MonoBehaviour
{
    [SerializeField] GridSystem gridSystem; // Nothing to inject 
    [SerializeField] HoverSystem hoverSystem; // Nothing to inject 
    [SerializeField] ActionSystem actionSystem; // Nothing to inject
    [SerializeField] UISystem uiSystem; // Nothing to inject 

    [SerializeField] SelectionSystem selectionSystem; // Inject gridSystem, actionSystem
    [SerializeField] CardSystem cardSystem; // Inject gridSystem, uiSystem, selectionSystem, actionSystem

    void Start()
    {
        selectionSystem.Init(gridSystem, actionSystem);
        cardSystem.Init(gridSystem, uiSystem, selectionSystem, actionSystem); 
    }
}
