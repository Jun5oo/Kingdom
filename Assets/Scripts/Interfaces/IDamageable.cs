using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 데미지를 받을 수 있는 오브젝트가 구현하는 인터페이스. isDirect=true이면 버프 보정을 건너뛴다.
/// </summary>
public interface IDamageable
{
    // 데미지를 받을 수 있는 오브젝트 인터페이스
    public int TakeDamage(int damage, bool isDirect = false);
    public bool IsAllies(int playerID); 
}

