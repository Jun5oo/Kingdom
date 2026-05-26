using UnityEngine;

/// <summary>
/// ActionType에 따라 적절한 IAction 인스턴스를 생성하는 팩토리.
/// Resurrection/DivineShield/Upgrade는 King 태그 토큰에서만 생성된다.
/// </summary>
public class ActionFactory
{
    /// <summary>
    /// actionType과 baseObject 조합으로 IAction을 생성한다.
    /// 잘못된 조합(예: Card로 AttackAction 생성 시도)이면 null을 반환한다.
    /// </summary>
    public IAction CreateAction(ActionType actionType, BaseObject baseObject, ActionPerformer performer = ActionPerformer.Player)
    {
        IAction action = null;

        switch (actionType)
        {
            case ActionType.Summon:
                if (baseObject is Card card)
                    action = new SummonAction(card, performer);
                break;
            case ActionType.Move:
                if (baseObject is Token moveableToken)
                    action = new MoveAction(moveableToken, performer);
                break;
            case ActionType.Attack:
                if (baseObject is Token attackableToken)
                    action = new AttackAction(attackableToken, performer);
                break;
            case ActionType.Resurrection:
                if (baseObject is Token undeadKing)
                {
                    if (undeadKing.Tag == UnitTag.King)
                        action = new ResurrectionAction(undeadKing, performer);
                }
                break;
            case ActionType.DivineShield:
                if (baseObject is Token celestialKing)
                {
                    if (celestialKing.Tag == UnitTag.King)
                        action = new DivineShieldAction(celestialKing, performer);
                }
                break;
            case ActionType.Upgrade:
                if (baseObject is Token king)
                {
                    if (king.Tag == UnitTag.King)
                        action = new UpgradeAction(king, performer);
                }
                break;
        }

        return action;
    }
}
