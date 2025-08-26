using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class DivineShieldAction : IAction
{
    ActionType actionType;
    HighlightLayer highlightLayer;
    HighlightType highlightType; 
    ActionPerformer performer;

    Token token; 
    BaseObject target;
    int currentCost;

    GridManager gridManager;
    TokenManager tokenManager; 

    public ActionType ActionType { get { return actionType; } }
    public HighlightLayer HighlightLayer { get { return highlightLayer; } }
    public HighlightType HighlightType { get { return highlightType; } }
    public ActionPerformer Performer { get { return performer; } }

    public int Cost { get { return currentCost; } }

    public event Action OnActionCanceled;
    public event Action OnActionComplete;

    public ResourceType resourceType;
    public ResourceType ResourceType { get { return resourceType; } }
    public int OwnerID { get { return token.OwnerID; } }

    public BaseObject Executor => token;

    public Predicate<Vector2Int> Validation => CanBuff; 

    public DivineShieldAction(Token token, ActionPerformer performer)
    {
        actionType = ActionType.DivineShield;
        highlightLayer = HighlightLayer.Action;
        highlightType = HighlightType.SummonHighlight;
        this.performer = performer;

        this.gridManager = ServiceLocator.Get<GridManager>();
        this.tokenManager =ServiceLocator.Get<TokenManager>();

        this.token = token; 
        this.target = token;
        this.currentCost = 2;

        resourceType = ResourceType.Ability; 
    }

    public void Enter()
    {

    }

    public async UniTask Execute(Vector2Int targetPosition)
    {
        Exit(); 

        // 나중에는 함수로 따로 작성 
        if (!tokenManager.IsTokenAtGridPosition(targetPosition))
        {
            OnActionCanceled?.Invoke();
            return;
        }

        Token target = tokenManager.GetTokenFrom(targetPosition);

        if (target is not IBuffable buffable)
        {
            OnActionCanceled?.Invoke();
            return;
        }

        IBuff buff = new DivineShield(buffable);
        await buff.OnApply();

        OnActionComplete?.Invoke(); 
    }

    public void Exit()
    {
    }

    bool CanBuff(Vector2Int pos)
    {
        if (!tokenManager.IsTokenAtGridPosition(pos))
            return false;

        Token token = tokenManager.GetTokenFrom(pos);

        if (token != null)
        {
            if (!token.IsAllies(target.OwnerID))
                return false;
            if (token.Data.Tag == UnitTag.King)
                return false;
            if (token is not IBuffable)
                return false;

            return true;
        }

        return false;
    }

    public bool IsValid()
    {
        return true; 
    }

}
