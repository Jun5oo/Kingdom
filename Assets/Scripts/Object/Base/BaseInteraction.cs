using System;
using UnityEngine;

/// <summary> 오브젝트 인터랙션 상태. Moving 중에는 Hover/Select 입력을 무시한다. </summary>
public enum InteractionState
{
    Idle,
    Hover,
    Selected,
    Moving
}

/// <summary>
/// Card/Token 인터랙션(Hover, Select, Move 상태 전환)의 공통 기반 클래스.
/// IHoverable과 ISelectable을 구현하며, 현재 상태(InteractionState)에 따라
/// Hover/Select 가능 여부를 자동으로 제어한다.
/// </summary>
public abstract class BaseInteraction : MonoBehaviour, IHoverable, ISelectable
{
    private BaseObject baseObject;
    [SerializeField] protected InteractionState currentState;

    protected Vector3 originPos;         // 선택 전 원래 위치
    protected Quaternion originRotation; // 선택 전 원래 회전
    protected Vector3 originScale;       // 선택 전 원래 크기

    public virtual BaseObject BaseObject { get { return baseObject; } }

    public event Action OnSelectedComplete; // 선택 행동이 완료되었을 때 발생

    public virtual void Init(BaseObject baseObject)
    {
        this.baseObject = baseObject;
        currentState = InteractionState.Idle;
    }

    public virtual bool IsHoverable() => currentState == InteractionState.Idle;
    public virtual bool IsSelectable() => currentState == InteractionState.Idle || currentState == InteractionState.Hover;

    public abstract void OnHover();
    public abstract void OffHover();
    public abstract void OnSelected();
    public abstract void OnDeselected();

    /// <summary> 이동이 완료되면 상태를 Idle로 복귀시킨다. </summary>
    public virtual void OnMoveComplete()
    {
        if (currentState == InteractionState.Moving)
            currentState = InteractionState.Idle;
    }

    /// <summary> 선택 흐름이 완료되었음을 알리는 이벤트를 발생시킨다. </summary>
    public virtual void OnSelectionComplete()
    {
        OnSelectedComplete?.Invoke();
    }

    /// <summary> PRS 갱신 시작 시 상태를 Moving으로 전환한다. </summary>
    public virtual void OnUpdatePRS()
    {
        currentState = InteractionState.Moving;
    }

    /// <summary> OnSelectedComplete 구독을 전부 해제한다. </summary>
    public virtual void UnSubscribe()
    {
        OnSelectedComplete = null;
    }
}
