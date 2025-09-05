using System.Collections.Generic;

public class AbilityFactory
{

    public List<Ability> CreateAbilityAsync(CardData cardData, BaseObject baseObject)
    {
        List<Ability> abilities = new List<Ability>();

        foreach (var abilitySO in cardData.Abilities)
        {
            Ability ability = new Ability(baseObject, abilitySO);
            abilities.Add(ability); 
        }

        return abilities; 
    }

}
