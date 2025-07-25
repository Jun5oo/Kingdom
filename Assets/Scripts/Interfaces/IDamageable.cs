using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    public void TakeDamage(int damage, bool isDirect = false);
    public bool IsAllies(int playerID); 
}

