using UnityEngine;

public class PassiveFactory
{
    public IPassive CreatePassive(PassiveType passiveType, BaseObject owner)
    {
        IPassive passive = null; 

        switch (passiveType)
        {
            case PassiveType.GainAbilityCoin:
                passive = new GainAbilityCoin(owner);
                break;
            case PassiveType.SummonGraveyard:
                passive = new SummonGraveyard(owner);
                break; 
        }

        return passive;
    }
}
