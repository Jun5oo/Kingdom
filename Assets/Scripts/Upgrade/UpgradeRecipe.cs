using System.Collections.Generic;
using Unity.VisualScripting;

public class UpgradeRecipe
{
    protected string recipeName;
    protected int tokenLevel;
    protected int tokenRequired;
    protected int abilityCoinRequired;

    protected bool isCardIDSame;

    public string Name { get { return recipeName; } }
    public int Level { get { return tokenLevel; } }
    public int SourceRequired { get { return tokenRequired; } }   
    public int ResourceRequired {  get { return abilityCoinRequired; } }
    public bool Equal {  get { return isCardIDSame; } }
    public UpgradeRecipe(string name, int tokenLevel, int tokenRequired, int abilityCoinRequired, bool isCardIDSame = false)
    {
        this.recipeName = name;
        this.tokenLevel = tokenLevel;
        this.tokenRequired = tokenRequired;
        this.abilityCoinRequired = abilityCoinRequired;

        this.isCardIDSame = isCardIDSame; 
    }

    public bool IsMatch(List<Token> tokens)
    {
        if (tokens == null || tokens.Count == 0)
            return false;

        var candidates = new List<Token>(); 

        foreach(var token in tokens)
        {
            if (token.Tag != UnitTag.Normal)
                continue;

            if (token.Level != tokenLevel)
                continue;

            candidates.Add(token); 
        }

        // 필요한 유닛 수 확인
        if (candidates.Count < tokenRequired) 
            return false;

        int playerID = candidates[0].OwnerID; 
        AbilityResourceSystem abilityResourceSystem = ServiceLocator.Get<AbilityResourceSystem>();

        // 왕 토큰이 필요한 경우, 플레이어가 소유한 왕 토큰이 충분한지 확인 
        if (abilityResourceSystem.GetCurrentResources(playerID) < abilityCoinRequired)
            return false;

        if (isCardIDSame)
        {
            // 동일한 카드를 요구하는 경우 
            var count = new Dictionary<int, int>(); 
            
            foreach(var candidate in candidates)
            {
                if (count.ContainsKey(candidate.ID))
                    count[candidate.ID]++;
                else
                    count[candidate.ID] = 1;

                if(count.TryGetValue(candidate.ID, out int num))
                {
                    if (num >= tokenRequired)
                        return true; 
                }
            }

            return false; 
        }

        else
        {
            var count = new Dictionary<int, int>();

            // 그렇지 않은 경우 
            foreach (var candidate in candidates)
            {
                if (count.ContainsKey(candidate.ID))
                    count[candidate.ID]++;
                else
                    count[candidate.ID] = 1;
            }

            if (count.Count < tokenRequired) 
                return false;
            else 
                return true; 
        }
    }
}
