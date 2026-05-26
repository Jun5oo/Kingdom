using UnityEngine;

/// <summary>
/// 레이캐스트 히트 결과를 ISelectable로 변환하고 유효성을 검증하는 유틸리티.
/// GridCell을 클릭했을 때 그 위의 토큰을 자동으로 감지한다.
/// </summary>
public class SelectionResolver
{
    TokenManager tokenManager;
    ActionSystem actionSystem;

    public SelectionResolver()
    {
        tokenManager = ServiceLocator.Get<TokenManager>();
        actionSystem = ServiceLocator.Get<ActionSystem>();
    }

    /// <summary>
    /// 레이캐스트 히트 결과에서 ISelectable을 추출한다.
    /// 직접 ISelectable 컴포넌트가 있으면 반환하고,
    /// GridCell이면 해당 위치의 토큰에서 ISelectable을 찾아 반환한다.
    /// </summary>
    public ISelectable Resolve(RaycastHit hit)
    {
        if (hit.collider.TryGetComponent<ISelectable>(out ISelectable direct))
            return direct;

        if (hit.collider.TryGetComponent<GridCell>(out GridCell gridCell))
        {
            Vector2Int gridPos = gridCell.GetGridPosition();

            if (tokenManager.TryGetTokenFrom(gridPos, out Token token))
            {
                if (token.TryGetComponent<ISelectable>(out ISelectable indirect))
                    return indirect;
            }
        }

        return null;
    }

    /// <summary>
    /// ISelectable이 현재 선택 가능한 상태인지 검사한다.
    /// null이거나 액션 진행 중이거나 IsSelectable()이 false이면 유효하지 않다.
    /// </summary>
    public bool IsValid(ISelectable selectable)
    {
        if (selectable == null)
            return false;

        if (actionSystem.IsActionInProgress())
            return false;

        if (!selectable.IsSelectable())
            return false;

        if (selectable.BaseObject == null)
            return false;

        return true;
    }
}
