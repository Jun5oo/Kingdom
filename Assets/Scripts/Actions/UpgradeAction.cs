using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

public enum UpgradeState
{
    RecipeSelection,
    SourceSelection,
    PlacementSelection,
    Done
}
/// <summary>
/// 필드 위의 유닛들을 재료로 삼아 상위 등급 유닛으로 합성(업그레이드)하는 액션.
/// 왕이 실행하며, RecipeSelection → SourceSelection → PlacementSelection 순서로 진행된다.
/// UpgradeRecipe를 통해 어떤 재료 조합이 가능한지 판별한다.
/// </summary>
public class UpgradeAction : IAction
{
    ActionType actionType;
    HighlightLayer highlightLayer;
    HighlightType highlightType;

    public ActionType ActionType { get { return actionType; } }
    public HighlightLayer HighlightLayer { get { return highlightLayer; } }
    public HighlightType HighlightType { get { return highlightType; } }

    ActionPerformer performer;
    public ActionPerformer Performer { get { return performer; } }

    Token actionOwner; // 업그레이드 액션을 실행하는 왕 토큰
    public int OwnerID => actionOwner.OwnerID;

    ResourceType resourceType; // 행동 포인트(Action)를 소모
    public ResourceType ResourceType { get { return resourceType; } }

    int cost; // 소모 비용 1
    public int Cost { get { return cost; } }

    public event Action OnActionCanceled;
    public event Action OnActionComplete;

    UpgradeState currentState; // 현재 업그레이드 진행 단계
    public UpgradeState UpgradeState { get { return currentState; } }

    public BaseObject Executor => actionOwner;

    public Predicate<Vector2Int> Validation => throw new NotImplementedException();

    GridManager gridManager;
    TokenManager tokenManager;
    UpgradeSystem upgradeSystem;

    EventQueue eventQueue;

    UpgradeRecipe currentRecipe;
    List<Token> picked;

    List<Vector2Int> validPositions;
    public UpgradeAction(Token token, ActionPerformer performer)
    {
        this.actionOwner = token;
        this.performer = performer;

        this.actionType = ActionType.Upgrade;
        this.highlightLayer = HighlightLayer.Action;
        // 임시 
        this.highlightType = HighlightType.SummonHighlight;

        this.resourceType = ResourceType.Action;
        this.cost = 1;

        gridManager = ServiceLocator.Get<GridManager>();
        tokenManager = ServiceLocator.Get<TokenManager>();
        upgradeSystem = ServiceLocator.Get<UpgradeSystem>();

        eventQueue = ServiceLocator.Get<EventQueue>();

        currentRecipe = null;
        picked = new List<Token>();

        validPositions = new List<Vector2Int>
        {
            new Vector2Int(1, 1),
            new Vector2Int(1, 0),
            new Vector2Int(1, -1),
            new Vector2Int(-1, 1),
            new Vector2Int(-1, 0),
            new Vector2Int(-1, -1),
            new Vector2Int(0, 1),
            new Vector2Int(0, -1)
        };
    }

    /// <summary>
    /// 현재 유효한 레시피 목록을 조회하고 첫 번째 레시피를 자동 선택한다.
    /// (추후 UI를 통한 레시피 선택으로 대체 예정)
    /// </summary>
    public void Enter()
    {
        var recipes = upgradeSystem.GetValidRecipes(OwnerID);

        foreach (var recipe in recipes)
            Debug.Log(recipe.Name);

        currentState = UpgradeState.RecipeSelection;
        OnRecipeSelected(recipes[0]);
    }

    /// <summary>
    /// SourceSelection 상태에서는 재료 유닛을 선택하고,
    /// PlacementSelection 상태에서는 업그레이드 결과 유닛의 배치 위치를 선택한다.
    /// </summary>
    public async UniTask Execute(Vector2Int targetPosition)
    {
        // 하이라이트된 재료 유닛 셀을 클릭했을 때
        if (currentState == UpgradeState.SourceSelection)
        {
            var selected = tokenManager.GetTokenFrom(targetPosition);
            if (selected == null)
                return;

            if (!picked.Contains(selected))
                picked.Add(selected);

            gridManager.UnhighlightGridCells(highlightLayer);


            if (currentRecipe.SourceRequired > picked.Count)
            {
                var candidates = GetCandidates(currentRecipe, OwnerID, picked);

                gridManager.HighlightGridCells((Vector2Int position) =>
                {
                    foreach (var candidate in candidates)
                    {
                        Vector2Int gridPos = tokenManager.GetGridPositionOfToken(candidate);

                        if (gridPos == position)
                            return true;
                    }

                    return false;
                }, highlightType, highlightLayer);

                return;
            }

            else
            {
                OnEnterPlacement();
                return;
            }
        }

        if (currentState == UpgradeState.PlacementSelection)
        {
            eventQueue.Enqueue(() =>
            {
                foreach (var selected in picked)
                    tokenManager.DestroyToken(selected);

                return UniTask.CompletedTask;
            });

            eventQueue.Enqueue(async () =>
            {
                await upgradeSystem.Upgrade(currentRecipe, picked, OwnerID, targetPosition, actionOwner.Data);
            });

            await eventQueue.ExecuteAllAsync();

            currentState = UpgradeState.Done;
            OnActionComplete?.Invoke();

            Exit();
        }


        return;
    }

    public void Exit()
    {
        gridManager.UnhighlightGridCells(highlightLayer);
        picked.Clear();
        currentRecipe = null;
    }

    /// <summary> 현재 플레이어가 사용 가능한 업그레이드 레시피가 하나라도 있으면 유효한 액션이다. </summary>
    public bool IsValid() => upgradeSystem.GetValidRecipes(OwnerID).Count > 0;

    /// <summary> 레시피를 선택하고 해당 레시피의 재료 후보 유닛들을 하이라이트한다. </summary>
    public void OnRecipeSelected(UpgradeRecipe recipe)
    {
        currentRecipe = recipe;
        picked.Clear();
        currentState = UpgradeState.SourceSelection;

        var candidates = GetCandidates(currentRecipe, OwnerID, picked);

        gridManager.HighlightGridCells((Vector2Int position) =>
        {
            foreach (var candidate in candidates)
            {
                Vector2Int gridPos = tokenManager.GetGridPositionOfToken(candidate);

                if (gridPos == position)
                    return true;
            }

            return false;
        }, highlightType, highlightLayer);
    }

    /// <summary> 재료 선택이 완료된 후 업그레이드 결과 유닛의 배치 가능 위치를 하이라이트한다. </summary>
    public void OnEnterPlacement()
    {
        gridManager.UnhighlightGridCells(highlightLayer);

        gridManager.HighlightGridCells((Vector2Int gridPosition) =>
        {
            if (tokenManager.TryGetKingTokenFrom(OwnerID, out Token kingToken))
            {
                Vector2Int gridPos = tokenManager.GetGridPositionOfToken(kingToken);

                foreach (var validPos in validPositions)
                {
                    Vector2Int availablePos = gridPos + validPos;
                    if (!tokenManager.IsTokenAtGridPosition(availablePos) && availablePos == gridPosition)
                        return true;
                }

                return false;
            }

            return false;

        }, HighlightType, HighlightLayer);

        currentState = UpgradeState.PlacementSelection;
    }

    /// <summary>
    /// 레시피 조건에 맞는 재료 후보 유닛 목록을 반환한다.
    /// 이미 선택된(picked) 유닛은 제외하고, 레시피 레벨·종류 조건에 맞는 유닛만 포함한다.
    /// </summary>
    public List<Token> GetCandidates(UpgradeRecipe recipe, int playerID, List<Token> picked)
    {
        var result = new List<Token>();
        var tokens = tokenManager.GetPlayerToken(playerID);

        if (recipe == null)
        {
            Debug.Log("Null recipe");
            return null;
        }

        foreach (var token in tokens)
        {
            if (token.Level != recipe.Level)
                continue;

            if (picked.Contains(token))
                continue;

            if (token.Tag != UnitTag.Normal)
                continue;

            if (!recipe.Equal)
            {
                foreach (var pickedToken in picked)
                {
                    if (pickedToken.Data.ID == token.Data.ID)
                        continue;
                }
            }

            result.Add(token);
        }

        return result;
    }

}
