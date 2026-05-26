using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 파괴 가능한 오브젝트가 구현하는 인터페이스. IsDead로 사망 상태를 확인하고 Die()로 제거 처리를 실행한다.
/// </summary>
public interface IDestructible
{
    // 파괴가 가능한 오브젝트 인터페이스
    bool IsDead { get; }
    public void Die(); 

}
