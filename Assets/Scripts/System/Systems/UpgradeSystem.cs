using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UpgradeSystem
{
    List<UpgradeRecipe> recipes;

    CardDatabase database;
    SummonSystem summonSystem; 
    TokenManager tokenManager; 

    public void Init()
    {
        recipes = new List<UpgradeRecipe>();

        recipes.Add(new UpgradeRecipe("동일한 1성 유닛 2개", 1, 2, 0, true));
        recipes.Add(new UpgradeRecipe("동일한 2성 유닛 2개", 2, 2, 0, true));
        recipes.Add(new UpgradeRecipe("서로 다른 2성 유닛 2개 및 코인 1개", 2, 2, 1, false));
        recipes.Add(new UpgradeRecipe("하나의 2성 유닛 및 코인 3개", 2, 1, 3, false)); 

        summonSystem = ServiceLocator.Get<SummonSystem>();
        tokenManager = ServiceLocator.Get<TokenManager>();
        database = ServiceLocator.Get<CardDatabase>();
    }

    public List<UpgradeRecipe> GetValidRecipes(int playerID) 
    {
        // 현재 코인의 개수 
        var coin = ServiceLocator.Get<AbilityResourceSystem>().GetCurrentResources(playerID);
        // 현재 플레이어의 보드 위 유닛 
        var tokens = tokenManager.GetPlayerToken(playerID);

        List<UpgradeRecipe> upgradable = new List<UpgradeRecipe>();

        foreach(var recipe in recipes)
        {
            if (recipe.IsMatch(tokens))
                upgradable.Add(recipe); 
        }

        return upgradable; 
    }

    public async UniTask Upgrade(UpgradeRecipe recipe, List<Token> sources, int playerID, Vector2Int targetPosition, CardData caller)
    {
        // 조합 식 재차 확인 
        if (!recipe.IsMatch(sources))
        {
            Debug.Log("진화를 할 수 없는 조합 식입니다.");
            return;
        }

        int baseLevel = sources[0].Level;
        int nextLevel = baseLevel + 1; 

        if(nextLevel > 3)
        {
            Debug.Log("더 이상 업그레이드 할 수 없습니다.");
            return; 
        }

        CardData cardData = GetRandomValidOutput(sources[0].Race);
        List<CardData> dataSources = new List<CardData>();  

        foreach(var source in sources)
            dataSources.Add(source.Data); 

        if(recipe.ResourceRequired > 0)
        {
            IResourceSystem abilitySystem = ServiceLocator.Get<AbilityResourceSystem>();
            abilitySystem.Consume(playerID, recipe.ResourceRequired); 
        }

        await summonSystem.Summon(playerID, cardData, targetPosition, caller, dataSources, nextLevel); 

    }

    public CardData GetRandomValidOutput(Race race)
    {
        List<CardData> raceList = database.GetRaceCardList(race)
            .OfType<CardData>().
            Where(u => u.Tag == UnitTag.Normal).
            ToList();

        if(raceList.Count == 0)
        {
            Debug.Log($"해당 {race}종족의 카드 데이터를 찾을 수 없습니다."); 
        }

        int idx = Random.Range(0, raceList.Count);
        return raceList[idx]; 
    }
}
