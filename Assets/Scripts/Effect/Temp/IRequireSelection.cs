using System;
using UnityEngine;

public interface IRequireSelection 
{
    Predicate<Vector2Int> GetValidation(BaseObject owner, EffectContext context);
}
