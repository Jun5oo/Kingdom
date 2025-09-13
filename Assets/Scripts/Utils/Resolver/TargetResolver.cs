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

        // 자신을 선택 
        if(target == Target.Self)
        {
            Vector2Int casterPosition = tokenManager.GetGridPositionOfToken(caster as Token); 
            
            if(IsTargetConditionSatisfied(caster, casterPosition, targetConditions))
            {
                candidates.Add(casterPosition);
            }

        }

        // 보드 위 모든 카드들 대상 (Condition, filter로 candidates 선별) 
        if(target == Target.Board)
        {
            var positions = gridManager.GetAllPositions(); 

            foreach(var position in positions)
            {
                if(IsTargetConditionSatisfied(caster, position, targetConditions))
                    candidates.Add(position);
            }
        }

        // 직접 선택, 여러 개 선택가능  
        if(target == Target.Select)
        {
            GridSelection gridSelection = ServiceLocator.Get<GridSelection>(); 

            Predicate<Vector2Int> canSelect = position => IsTargetConditionSatisfied(caster, position, targetConditions);
            HighlightContext highlightContext = new HighlightContext { layer = HighlightLayer.Action, type = HighlightType.SummonHighlight };

            var pos = await gridSelection.WaitGridSelectionAsync(canSelect, highlightContext);

            if (!IsTargetConditionSatisfied(caster, pos, targetConditions))
                return candidates; 

            candidates.Add(pos); 
        }

        // 처치한 유닛의 위치 
        if(target == Target.Kill)
        {
            // 추후에 
            if(context.TryGet<ObjectContext>(ContextKey.Kill, out var killContext))
            {
                if (!IsTargetConditionSatisfied(caster, killContext.gridPosition, targetConditions))
                    return candidates; 
            }

            candidates.Add(killContext.gridPosition);   
        }

        // 처치당한 유닛의 위치 
        if(target == Target.Death)
        {
            if (context.TryGet<ObjectContext>(ContextKey.Death, out var deathContext))
            {
                if (!IsTargetConditionSatisfied(caster, deathContext.gridPosition, targetConditions))
                    return candidates;
            }

            candidates.Add(deathContext.gridPosition);
        }

        // 마지막으로 파괴된 유닛 선택 
        if(target == Target.LastDestroyed)
        {
            // 추후 
        }

        return candidates;
    }

    bool IsTargetConditionSatisfied(BaseObject caster, Vector2Int gridPosition, List<ConditionSO> targetConditions)
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
