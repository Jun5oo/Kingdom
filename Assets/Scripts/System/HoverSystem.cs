using UnityEngine;
using UnityEngine.EventSystems; 

/// <summary>
/// Hover를 처리하는 System 클래스 
/// </summary>

public class HoverSystem : MonoBehaviour, IHoverSystem
{
    private IHoverable currentHoverable = null;
    void Update()
    {
        // UI가 있는 경우 
        if (EventSystem.current.IsPointerOverGameObject())
        {
            ExitHover();
            return; 
        }

        // Input.mousePosition의 경우 screen position을 반환하기 때문에 world 좌표계 값으로 변경해줘야한다. 
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Collider를 탐지했을 경우 
        if (Physics.Raycast(ray, out hit, 100f))
        {
            if (hit.transform.gameObject.TryGetComponent<IHoverable>(out IHoverable hoverable))
            {
                // 새로운 IHoverable을 탐지했을 경우 
                if (hoverable != currentHoverable)
                {
                    // 기존의 IHoverable Exit
                    ExitHover();

                    // 새로 찾은 IHoverable 오브젝트가 hoverable한 경우 (카드의 경우, 선택되었거나, 카드가 움직이고 있거나, 액션을 하는 도중에는 onHover 상태가 되어서는 안됨) 
                    if (hoverable.IsHoverable())
                    {
                        EnterHover(hoverable);
                    }

                }
            }
        }

        // Collider를 탐지하지 못한 경우 
        else
        {
            // 기존 IHoverable 오브젝트가 드로우, 액션을 통해 움직여서 범위에서 벗어날 경우, ExitHover가 되면 갑자기 ExitHover 되어 이상하게 보이므로 IsHoverable 조건을 추가 
            if(currentHoverable != null && currentHoverable.IsHoverable())
                ExitHover();
        }
    }

    #region Hover
    public void EnterHover(IHoverable hoverable)
    {
        currentHoverable = hoverable;
        currentHoverable?.OnHover();
    }
    public void ExitHover()
    {
        currentHoverable?.OffHover();
        currentHoverable = null; 
    }
    #endregion 

    public IHoverable GetCurrentHoverable() => currentHoverable;


}
