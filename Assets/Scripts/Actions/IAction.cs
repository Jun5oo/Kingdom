using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

/// <summary>
/// 모든 액션(이동, 공격, 소환 등)이 구현해야 하는 공통 인터페이스.
/// ActionSystem은 이 인터페이스를 통해 액션을 실행한다.
/// </summary>
public interface IAction
{
    public int OwnerID { get; }           // 이 액션의 소유자 플레이어 ID
    public BaseObject Executor { get; }   // 액션을 실행하는 주체 (Card 또는 Token)

    public ActionType ActionType { get; }            // 액션 종류 (Move, Attack, Summon 등)
    public HighlightLayer HighlightLayer { get; }    // 그리드 하이라이트 레이어
    public HighlightType HighlightType { get; }      // 그리드 하이라이트 색상/타입
    public ActionPerformer Performer { get; }        // 실행 주체 (Player 또는 System/AI)
    public Predicate<Vector2Int> Validation { get; } // 대상 그리드 셀이 유효한지 판별하는 조건
    public ResourceType ResourceType { get; }        // 소모 리소스 타입 (Action 또는 Ability)
    public int Cost { get; }                         // 액션 실행에 필요한 리소스 소모량

    /// <summary> 액션 진입 시 초기화 (하이라이트 표시 등) </summary>
    public void Enter();
    /// <summary> 액션 종료 시 정리 (하이라이트 제거 등) </summary>
    public void Exit();
    /// <summary> 플레이어가 선택한 그리드 좌표로 실제 액션을 수행 </summary>
    public UniTask Execute(Vector2Int targetPosition);
    /// <summary> 현재 이 액션을 실행할 수 있는 상태인지 여부 </summary>
    public bool IsValid();

    public event Action OnActionCanceled; // 액션이 취소되었을 때 발생
    public event Action OnActionComplete; // 액션이 완료되었을 때 발생
}
