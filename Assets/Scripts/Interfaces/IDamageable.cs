using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    // 데미지를 받을 수 있는 오브젝트 인터페이스 
    public int TakeDamage(int damage, bool isDirect = false);
    public bool IsAllies(int playerID); 
}

