using System.Collections.Generic;

/// <summary>
/// BaseObject의 종류와 CardData를 보고 실행 가능한 ActionType 목록을 반환하는 유틸리티.
/// Card이면 Summon, Token이면 이동·공격 범위 유무를 검사하여 Move/Attack을 포함한다.
/// </summary>
public class ActionResolver
{
    /// <summary>
    /// baseObject에서 수행 가능한 ActionType 목록을 반환한다.
    /// RangeType.None인 Move/Attack은 목록에 포함되지 않는다.
    /// </summary>
    public List<ActionType> GetValidActions(BaseObject baseObject)
    {
        var actions = new List<ActionType>();

        switch (baseObject)
        {
            case Card card:
                actions.Add(ActionType.Summon);
                break;

            case Token token:
                if(token.Data.MoveType[token.Level-1] != RangeType.None)
                    actions.Add(ActionType.Move);
                if (token.Data.AttackType[token.Level-1] != RangeType.None)
                    actions.Add(ActionType.Attack);
                /*
                if (token.Data.Tag == UnitTag.King)
                    actions.Add(ActionType.Upgrade); 
                */ 
                break; 
        }


        return actions; 
    }

}
