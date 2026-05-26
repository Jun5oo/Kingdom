using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// UniTask 기반의 순차 비동기 이벤트 큐.
/// Enqueue()로 작업을 쌓고 ExecuteAllAsync()로 순서대로 실행한다.
/// 전투 해결(공격 → 반격 → 간접 데미지 → 사망 처리) 순서 보장에 사용된다.
/// ExecuteAllAsync() 실행 중 Cancel()을 호출하면 현재 작업 완료 후 중단된다.
/// </summary>
public class EventQueue
{
    Queue<Func<UniTask>> queue;
    bool isRunning;
    bool isCanceled;

    public EventQueue()
    {
        queue = new Queue<Func<UniTask>>();
        isRunning = false;
        isCanceled = false;
    }

    /// <summary> 비동기 작업을 큐 끝에 추가한다. </summary>
    public void Enqueue(Func<UniTask> task)
    {
        queue.Enqueue(task);
    }

    /// <summary>
    /// 큐에 있는 모든 작업을 순서대로 실행한다.
    /// 이미 실행 중이면 즉시 반환한다. 각 작업의 예외는 로그로 기록하고 계속 진행한다.
    /// </summary>
    public async UniTask ExecuteAllAsync()
    {
        if (isRunning)
            return;

        isRunning = true;
        isCanceled = false;

        while (queue.Count > 0)
        {
            if (isCanceled)
            {
                Debug.Log($"{this} canceled");
                isRunning = false;
                isCanceled = false;
                return;
            }

            var _event = queue.Dequeue();

            try
            {
                await _event();
            }
            catch (Exception ex)
            {
                Debug.LogError($"Event 실행 중 오류 발생{ex}");
            }
        }

        isRunning = false;
        isCanceled = false;
    }

    /// <summary> 다음 루프 체크 시 큐 실행을 중단하도록 플래그를 설정한다. </summary>
    public void Cancel()
    {
        isCanceled = true;
    }

    /// <summary> 큐에 남은 모든 작업을 즉시 제거한다. </summary>
    public void Clear()
    {
        queue.Clear();
    }

    public bool IsRunning { get { return isRunning; } }
    public bool IsCanceled { get { return isCanceled; } }
}
