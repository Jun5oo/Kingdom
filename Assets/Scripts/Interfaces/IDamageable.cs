using UnityEngine;

public interface IDamageable
{
    public void TakeDamage(int damage, bool isDirect = false);
    public bool IsAlies(); 
}

