using System.Collections.Generic;

public class AbilityFactory
{
    EffectFactory effectFactory;

    public AbilityFactory()
    {
        effectFactory = new EffectFactory(); 
    }

    // 카드 데이터의 EffectData를 GroupID, Trigger를 기반으로 묶어서 Ability로 생성 
    public List<Ability> CreateAbilityAsync(CardData cardData, BaseObject baseObject)
    {
        List<Ability> abilities = new List<Ability>();

        // groupID, List<IEffect> 
        Dictionary<(int groupID, Trigger), List<IEffect>> effectDictionary = new Dictionary<(int groupID, Trigger), List<IEffect>>();

        foreach(var data in cardData.Effects)
        {
            IEffect effect = effectFactory.CreateEffect(data, baseObject);

            // Dictionary에 묶어서 하나의 Ability로 만들기; 

            if (effect == null)
                continue;

            var key = (data.groupID, data.trigger); 

            if (!effectDictionary.TryGetValue(key, out var effects))
            {
                effects = new List<IEffect>();   
                effectDictionary.Add((data.groupID, data.trigger), effects);
            }

            effectDictionary[key].Add(effect); 
        }
        
        foreach(var entry in effectDictionary)
        {
            var (group, trigger) = entry.Key;
            var effects = entry.Value; 

            Ability ability = new Ability(trigger, group, entry.Value, baseObject); 
            abilities.Add(ability);
        }

        return abilities;
    }

}
