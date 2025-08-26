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
            case PassiveType.DestroySelf:
                passive = new DestroySelf(owner);
                break; 
            default:
                break; 
        }

        return passive;
    }
}
