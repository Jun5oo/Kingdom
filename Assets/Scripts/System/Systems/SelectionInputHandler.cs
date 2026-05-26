using System;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 마우스 입력을 처리하고 레이캐스트로 클릭 대상을 감지하는 입력 핸들러.
/// SelectionSystem의 Update에서 매 프레임 호출된다.
/// </summary>
public class SelectionInputHandler
{
    SelectionResolver resolver;
    Action<ISelectable> onSelect; // 감지된 ISelectable을 SelectionSystem에 전달하는 콜백

    public SelectionInputHandler(SelectionResolver resolver, Action<ISelectable> onSelect)
    {
        this.resolver = resolver;
        this.onSelect = onSelect;
    }

    /// <summary>
    /// 매 프레임 마우스 입력을 처리한다.
    /// UI 위에서의 클릭은 무시하고, 우클릭은 선택 해제(null), 좌클릭은 레이캐스트 후 오브젝트 감지를 수행한다.
    /// </summary>
    public void Update()
    {
        // UI가 클릭된 경우 게임 오브젝트 선택을 무시한다.
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            onSelect?.Invoke(null); // 우클릭: 선택 해제
            return;
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 300f))
            {
                ISelectable selectable = resolver.Resolve(hit);
                onSelect?.Invoke(selectable);
            }
        }
    }
}
