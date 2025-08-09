using Cysharp.Threading.Tasks;
using UnityEngine;

public interface IDestructible
{
    // 파괴가 가능한 오브젝트 인터페이스
    bool IsDead { get; }
    public void Die(); 

}
