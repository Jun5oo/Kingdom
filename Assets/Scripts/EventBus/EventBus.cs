using System;
using UnityEngine;

public static class EventBus<T> where T: IGameEvent 
{
    public static Action<T> OnEvent;

    public static void Publish(T gameEvent) => OnEvent?.Invoke(gameEvent);
    public static void Subscribe(Action<T> listener) => OnEvent += listener; 
    public static void Unsubscribe(Action<T> listener) => OnEvent -= listener;  
}
