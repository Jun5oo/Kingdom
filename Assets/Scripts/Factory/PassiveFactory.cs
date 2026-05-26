using UnityEngine;

/// <summary>
/// PassiveType에 따라 IPassive 인스턴스를 생성하는 팩토리.
/// 알 수 없는 PassiveType이면 null을 반환한다.
/// </summary>
public class PassiveFactory
{
    /// <summary> passiveType에 해당하는 IPassive를 owner에 바인딩하여 반환한다. </summary>
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
