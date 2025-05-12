using UnityEngine;
using UnityEngine.UI;

public class SelectionSystem : MonoBehaviour
{
    ISelectable currentSelectable = null;

    void Update()
    {
        // HoverSystem과 SelectionSystem에 중복되는 코드가 존재. InputSystem script 고려 
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            OnExitSelected();
            return; 
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 50f) && Input.GetKeyDown(KeyCode.Mouse0))
        {
            ISelectable selectable;

            if (hit.transform.gameObject.TryGetComponent<ISelectable>(out selectable))
            {
                // selectable 객체가 IsSelectable을 통해서 현재 선택가능한 상황인지 확인 
                // 코스트가 부족하거나, 이미 Selected 된 상황이거나 
                if(currentSelectable != selectable && selectable.IsSelectable())
                {
                    OnExitSelected(); 
                    currentSelectable = selectable;
                    OnEnterSelected(); 
                }
            }
        }
    }

    void OnEnterSelected()
    {
        currentSelectable?.OnSelected();
    }

    void OnExitSelected()
    {
        currentSelectable?.OnDeselected(); 
    }
}
