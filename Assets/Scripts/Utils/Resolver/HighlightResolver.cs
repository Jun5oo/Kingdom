using System.Collections.Generic;
using UnityEngine;

public struct HighlightContext
{
    public HighlightType type;
    public HighlightLayer layer; 
}

public class HighlightResolver 
{
    Dictionary<ActionType, (HighlightType, HighlightLayer)> highlightDict; 

    public HighlightResolver()
    {
        highlightDict = new Dictionary<ActionType, (HighlightType, HighlightLayer)>();

        highlightDict.Add(ActionType.Summon, (HighlightType.SummonHighlight, HighlightLayer.Action)); 
        highlightDict.Add(ActionType.Move, (HighlightType.MoveHighlight, HighlightLayer.Action));
        highlightDict.Add(ActionType.Attack, (HighlightType.AttackHighlight, HighlightLayer.Action));
    }

    public HighlightContext Resolve(ActionType actionType)
    {
        HighlightContext ctx; 

        if (highlightDict.TryGetValue(actionType, out var value))
        {
            (HighlightType type, HighlightLayer layer) = value; 

            ctx = new HighlightContext();
            ctx.type = type; 
            ctx.layer = layer;

            return ctx; 
        }

        Debug.LogError($"{actionType}의 Highlight 정보를 찾을 수 없습니다.");

        return new HighlightContext(); 
    }
}
