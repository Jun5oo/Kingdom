using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

public class TargetResolver
{
    // Target Trigger에 따라서 대상을 다 가져온 후에, TargetCondition에 맞게 Filtering 후 Candidate를 return 
    public async UniTask<List<Vector2Int>> TryResolve(BaseObject caster, Target target, List<ConditionSO> targetConditions, List<FilterSO> filters, EffectContext context)
    {
        var candidates = new List<Vector2Int>();

        GridManager gridManager = ServiceLocator.Get<GridManager>();
        TokenManager tokenManager = ServiceLocator.Get<TokenManager>(); 

        if(target == Target.Self)
        {
            Vector2Int casterPosition = tokenManager.GetGridPositionOfToken(caster as Token); 
            if(IsTargetConditionSatisfied(casterPosition, caster, targetConditions, context))
            {
                candidates.Add(casterPosition);
            }

        }

        if(target == Target.Board)
        {
            var positions = gridManager.GetAllPositions(); 

            foreach(var position in positions)
            {
                if(IsTargetConditionSatisfied(position, caster, targetConditions, context))
                    candidates.Add(position);
            }
        }

        if(target == Target.Select)
        {
            GridSelection gridSelection = ServiceLocator.Get<GridSelection>(); 

            Predicate<Vector2Int> canSelect = position => IsTargetConditionSatisfied(position, caster, targetConditions, context);
            HighlightContext highlightContext = new HighlightContext { layer = HighlightLayer.Action, type = HighlightType.SummonHighlight };

            var pos = await gridSelection.WaitGridSelectionAsync(canSelect, highlightContext);

            if (!IsTargetConditionSatisfied(pos, caster, targetConditions, context))
                return candidates; 

            candidates.Add(pos); 
        }

        return candidates;
    }

    bool IsTargetConditionSatisfied(Vector2Int gridPosition, BaseObject caster, List<ConditionSO> targetConditions, EffectContext context)
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

}
