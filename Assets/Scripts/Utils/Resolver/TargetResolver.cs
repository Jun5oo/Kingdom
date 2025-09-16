using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

public class TargetResolver
{
    TokenManager tokenManager;
    GridManager gridManager; 

    public void Init()
    {
        gridManager = ServiceLocator.Get<GridManager>();
        tokenManager = ServiceLocator.Get<TokenManager>();
    }

    public async UniTask<List<Vector2Int>> TryResolve(BaseObject caster, Target target, List<ConditionSO> targetConditions, List<FilterSO> filters, EffectContext context)
    {
        var candidates = new List<Vector2Int>();

        switch (target)
        {
            case Target.Self:
                candidates = ResolveSelfTarget(caster, targetConditions);
                break;
            case Target.Board:
                candidates = ResolveBoardTarget(caster, targetConditions);
                break;
            case Target.Select:
                candidates = await ResolveSelectTarget(caster, targetConditions);
                break;
            case Target.Kill:
                candidates = ResolveKillTarget(caster, targetConditions, context);
                break;
            case Target.Death:
                candidates = ResolveDeathTarget(caster, targetConditions, context);
                break; 
        }

        Debug.Log($"Resolve Complete:{candidates.Count}"); 

        return candidates; 
        // return ApplyFilter(candidates, filters); 
    }

    #region Resolve Method
    List<Vector2Int> ResolveSelfTarget(BaseObject caster, List<ConditionSO> targetConditions)
    {
        var candidates = new List<Vector2Int>();    

        Vector2Int casterPosition = tokenManager.GetGridPositionOfToken(caster as Token);

        if (IsTargetConditionSatisfied(caster, casterPosition, targetConditions))
            candidates.Add(casterPosition);
        
        return candidates; 
    }
    List<Vector2Int> ResolveBoardTarget(BaseObject caster, List<ConditionSO> targetConditions)
    {
        var candidates = new List<Vector2Int>();

        var positions = gridManager.GetAllPositions();

        foreach (var position in positions)
        {
            if (IsTargetConditionSatisfied(caster, position, targetConditions))
                candidates.Add(position);
        }

        return candidates; 
    }
    async UniTask<List<Vector2Int>> ResolveSelectTarget(BaseObject caster, List<ConditionSO> targetConditions)
    {
        var candidates = new List<Vector2Int>();

        GridSelection gridSelection = ServiceLocator.Get<GridSelection>();

        Predicate<Vector2Int> canSelect = position => IsTargetConditionSatisfied(caster, position, targetConditions);
        HighlightContext highlightContext = new HighlightContext { layer = HighlightLayer.Action, type = HighlightType.SummonHighlight };

        var pos = await gridSelection.WaitSelectionAsync(canSelect, highlightContext);

        if (!IsTargetConditionSatisfied(caster, pos, targetConditions))
            return candidates;

        candidates.Add(pos);
        return candidates; 
    }
    List<Vector2Int> ResolveKillTarget(BaseObject caster, List<ConditionSO> targetConditions, EffectContext context)
    {
        var candidates = new List<Vector2Int>(); 

        if (context.TryGet<ObjectContext>(ContextKey.Kill, out var killContext))
        {
            if (!IsTargetConditionSatisfied(caster, killContext.gridPosition, targetConditions))
                return candidates;
        }

        candidates.Add(killContext.gridPosition);
        return candidates; 
    }
    List<Vector2Int> ResolveDeathTarget(BaseObject caster, List<ConditionSO> targetConditions, EffectContext context)
    {
        var candidates = new List<Vector2Int>(); 

        if (context.TryGet<ObjectContext>(ContextKey.Death, out var deathContext))
        {
            if (!IsTargetConditionSatisfied(caster, deathContext.gridPosition, targetConditions))
                return candidates;
        }

        candidates.Add(deathContext.gridPosition);
        return candidates;
    }
    #endregion 

    public List<Vector2Int> GetValidSelectTarget(BaseObject caster, List<ConditionSO> targetConditions, List<FilterSO> filters)
    {
        var candidates = new List<Vector2Int>();
        var positions = gridManager.GetAllPositions();

        foreach (var position in positions)
        {
            if (IsTargetConditionSatisfied(caster, position, targetConditions))
                candidates.Add(position);
        }

        // return ApplyFilter(candidates, filters); 
        return candidates;
    }

    public bool IsTargetConditionSatisfied(BaseObject caster, Vector2Int gridPosition, List<ConditionSO> targetConditions)
    {
        var tokenManager = ServiceLocator.Get<TokenManager>();
        
        if(tokenManager.TryGetTokenFrom(gridPosition, out var token))
            Debug.Log("해당 위치에서 하수인을 찾을 수 없습니다."); 

        foreach (var targetCondition in targetConditions)
        {
            if (!targetCondition.IsTargetConditionSatisfied(caster))
                return false; 
            if (!targetCondition.IsTargetConditionSatisfied(caster, token))
                return false;
            if (!targetCondition.IsTargetConditionSatisfied(caster, gridPosition))
                return false; 
        }

        return true; 
    }

    /*
    private UniTask<List<Vector2Int>> ApplyFilter(List<Vector2Int> candidates, List<FilterSO> filters)
    {
 
    }
    */ 
}
