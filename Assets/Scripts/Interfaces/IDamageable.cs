using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    public int CP { get; }
    public List<Vector2Int> AttackRange { get; }
    public void TakeDamage(int damage, bool isDirect = false);
    public bool IsAllies(Token token); 
}

