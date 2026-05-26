using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이어/AI의 페이즈 기반 상태 머신 기반 클래스.
/// Phase와 IPhaseState를 딕셔너리로 관리하며 ChangeState()로 전환한다.
/// BotController와 PlayerController가 상속한다.
/// </summary>
public abstract class AgentController : MonoBehaviour
{
    private IPhaseState currentState;

    private Dictionary<Phase, IPhaseState> phaseStateDict = new Dictionary<Phase, IPhaseState>();

    [field: SerializeField] public Phase CurrentPhase { get; private set; }

    protected virtual void OnPhaseChanged(Phase newPhase) { }

    /// <summary> 서브클래스에서 페이즈별 IPhaseState를 AddPhase()로 등록한다. </summary>
    public abstract void InitializePhaseStates();

    /// <summary> 페이즈와 상태를 딕셔너리에 등록한다. 중복 등록은 무시된다. </summary>
    protected void AddPhase(Phase newPhase, IPhaseState newState)
    {
        if (!phaseStateDict.ContainsKey(newPhase))
        {
            phaseStateDict.Add(newPhase, newState);
        }
    }

    /// <summary> 지정 페이즈로 전환하고 Enter()를 호출한다. 등록되지 않은 페이즈는 무시된다. </summary>
    public void ChangeState(Phase phase)
    {
        if (phaseStateDict.TryGetValue(phase, out IPhaseState newState))
        {
            currentState = newState;
            currentState.Enter(this);
            CurrentPhase = phase;

            OnPhaseChanged(phase);
        }
    }

    public void ExecuteState()
    {
        currentState?.Execute();
    }

    public void ExitState()
    {
        currentState?.Exit();
    }
}
