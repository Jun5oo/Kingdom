using System.Collections.Generic;
using UnityEngine;

public abstract class AgentController : MonoBehaviour
{
    private IPhaseState currentState;

    //[field: SerializeField] public DeckController Deck { get; private set; }

    private Dictionary<Phase, IPhaseState> phaseStateDict = new Dictionary<Phase, IPhaseState>();

    [field: SerializeField] public Phase CurrentPhase { get; private set; }

    protected virtual void OnPhaseChanged(Phase newPhase) { }

    /// <summary>
    /// 페이즈 세팅
    /// </summary>
    public abstract void InitializePhaseStates();

    /// <summary>
    /// 페이즈 딕셔너리에 추가
    /// </summary>
    /// <param name="newPhase">추가할 페이즈</param>
    /// <param name="newState">추가할 페이즈 상태</param>
    protected void AddPhase(Phase newPhase, IPhaseState newState)
    {
        if (!phaseStateDict.ContainsKey(newPhase))
        {
            phaseStateDict.Add(newPhase, newState);
        }
    }

    /// <summary>
    /// 페이즈 상태 변경
    /// </summary>
    /// <param name="phase">바꿀 페이즈</param>
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
