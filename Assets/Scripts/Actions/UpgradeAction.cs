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

public class UpgradeAction : IGameAction
{
    ActionType actionType;

    public ActionType ActionType { get { return actionType; } }

    // UpgradeAction을 실행한 주체, 왕 
    Token actionOwner;
    public int OwnerID => actionOwner.OwnerID;

    // 행동 카운트를 소모해서 발동 
    ResourceType resourceType;
    public ResourceType ResourceType { get { return resourceType; } }

    // 행동 코스트 1 
    int cost; 
    public int Cost {  get { return cost; } }

    public event Action OnActionCanceled;
    public event Action OnActionComplete;

    // FSM 상태로 행동 진행 
    UpgradeState currentState; 
    public UpgradeState UpgradeState { get { return currentState; } }

    public BaseObject BaseObject => actionOwner;

    public Predicate<Vector2Int> Validation => throw new NotImplementedException();

    GridManager gridManager;
    TokenManager tokenManager; 
    UpgradeSystem upgradeSystem;

    EventQueue eventQueue;

    UpgradeRecipe currentRecipe; 
    List<Token> picked;

    List<Vector2Int> validPositions; 
    public UpgradeAction(Token token)
    {
        this.actionOwner = token;

        this.actionType = ActionType.Upgrade;

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

    public void Enter()
    {
        /*
        var recipes = upgradeSystem.GetValidRecipes(OwnerID); 
        
        // UI를 보여주고 선택 한 후에 해당 Source에 맞는 유닛들을 Highlight 하려고 하지만, 현재는 Log만 남기고 첫 번째 Recipe만을 선택한다고 가정 
        foreach(var recipe in recipes)
            Debug.Log(recipe.Name);

        currentState = UpgradeState.RecipeSelection;
        
        OnRecipeSelected(recipes[0]); 
        */
    }

    public async UniTask Execute(Vector2Int targetPosition)
    {
        /*
        // Execute가 되었다는 것은, Highlight된 어느 한 Source를 클릭했다는 것 
        if(currentState == UpgradeState.SourceSelection)
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
    
        if(currentState == UpgradeState.PlacementSelection)
        {
            eventQueue.Enqueue(() =>
            {
                foreach(var selected in picked)
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
        */
    }

    public void Exit()
    {
        /*
        gridManager.UnhighlightGridCells(highlightLayer);
        picked.Clear();
        currentRecipe = null; 
        */
    }

    // 하나라도 진화할 수 있는 족보가 있으면 Valid 
    public bool IsValid() => upgradeSystem.GetValidRecipes(OwnerID).Count > 0; 
    
    public void OnRecipeSelected(UpgradeRecipe recipe)
    {
        /*
        currentRecipe = recipe; 
        picked.Clear();
        currentState = UpgradeState.SourceSelection;

        var candidates = GetCandidates(currentRecipe, OwnerID, picked);
        
        gridManager.HighlightGridCells((Vector2Int position) =>
        {
            foreach(var candidate in candidates)
            {
                Vector2Int gridPos = tokenManager.GetGridPositionOfToken(candidate);

                if (gridPos == position)
                    return true; 
            }

            return false; 
        }, highlightType, highlightLayer);
        */ 
    }

    public void OnEnterPlacement()
    {
        /*
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
        */
    }

    // 진화를 하기 위한 재료 후보들을 찾는 함수 (Highlight를 하기 위함) 
    public List<Token> GetCandidates(UpgradeRecipe recipe, int playerID, List<Token> picked)
    {
        var result = new List<Token>();
        var tokens = tokenManager.GetPlayerToken(playerID); 

        if(recipe == null)
        {
            Debug.Log("Null recipe");
            return null; 
        }

        foreach(var token in tokens)
        {
            if (token.Level != recipe.Level)
                continue;

            if (picked.Contains(token))
                continue;

            if (token.Tag != UnitTag.Normal)
                continue;

            if (!recipe.Equal)
            {
                foreach(var pickedToken in picked)
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
