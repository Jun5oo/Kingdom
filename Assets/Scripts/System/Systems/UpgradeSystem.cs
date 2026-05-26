using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 유닛 합성(업그레이드) 로직을 담당하는 시스템.
/// UpgradeRecipe에 정의된 조합 조건을 검증하고 상위 레벨 유닛을 소환한다.
/// </summary>
public class UpgradeSystem
{
    List<UpgradeRecipe> recipes; // 사용 가능한 전체 업그레이드 레시피 목록

    CardDatabase database;
    SummonSystem summonSystem;
    TokenManager tokenManager;

    /// <summary> 업그레이드 레시피를 정의하고 의존 시스템을 초기화한다. </summary>
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

    /// <summary>
    /// 현재 플레이어의 필드 유닛과 코인으로 충족 가능한 레시피 목록을 반환한다.
    /// </summary>
    public List<UpgradeRecipe> GetValidRecipes(int playerID)
    {
        var coin = ServiceLocator.Get<AbilityResourceSystem>().GetCurrentResources(playerID);
        var tokens = tokenManager.GetPlayerToken(playerID);

        List<UpgradeRecipe> upgradable = new List<UpgradeRecipe>();

        foreach (var recipe in recipes)
        {
            if (recipe.IsMatch(tokens))
                upgradable.Add(recipe);
        }

        return upgradable;
    }

    /// <summary>
    /// 재료 유닛들을 제거하고 다음 레벨 유닛을 지정 위치에 소환한다.
    /// 레시피에 코인 비용이 있으면 차감한다.
    /// </summary>
    public async UniTask Upgrade(UpgradeRecipe recipe, List<Token> sources, int playerID, Vector2Int targetPosition, CardData caller)
    {
        if (!recipe.IsMatch(sources))
        {
            Debug.Log("진화를 할 수 없는 조합 식입니다.");
            return;
        }

        int baseLevel = sources[0].Level;
        int nextLevel = baseLevel + 1;

        if (nextLevel > 3)
        {
            Debug.Log("더 이상 업그레이드 할 수 없습니다.");
            return;
        }

        CardData cardData = GetRandomValidOutput(sources[0].Race);
        List<CardData> dataSources = new List<CardData>();

        foreach (var source in sources)
            dataSources.Add(source.Data);

        if (recipe.ResourceRequired > 0)
        {
            IResourceSystem abilitySystem = ServiceLocator.Get<AbilityResourceSystem>();
            abilitySystem.Consume(playerID, recipe.ResourceRequired);
        }

        await summonSystem.Summon(playerID, cardData, targetPosition, caller, dataSources, nextLevel);
    }

    /// <summary>
    /// 해당 종족의 Normal 태그 카드 중 무작위로 업그레이드 결과 CardData를 반환한다.
    /// </summary>
    public CardData GetRandomValidOutput(Race race)
    {
        List<CardData> raceList = database.GetRaceCardList(race)
            .OfType<CardData>()
            .Where(u => u.Tag == UnitTag.Normal)
            .ToList();

        if (raceList.Count == 0)
            Debug.Log($"해당 {race}종족의 카드 데이터를 찾을 수 없습니다.");

        int idx = Random.Range(0, raceList.Count);
        return raceList[idx];
    }
}
