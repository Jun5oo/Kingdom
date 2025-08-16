using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine; 

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

    public void Enqueue(Func<UniTask> task)
    {
        queue.Enqueue(task); 
    }

    public async UniTask ExecuteAllAsync()
    {
        if (isRunning)
            return;

        isRunning = true;
        isCanceled = false; 

        while(queue.Count > 0)
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

    public void Cancel()
    {
        isCanceled = true; 
    }
    public void Clear()
    {
        queue.Clear(); 
    }

    public bool IsRunning { get { return isRunning; } }
    public bool IsCanceled { get { return isCanceled; } }
}
