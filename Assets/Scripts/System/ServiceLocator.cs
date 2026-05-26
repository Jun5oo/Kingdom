using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 게임 전역에서 시스템/매니저에 접근할 수 있는 서비스 컨테이너.
/// Bootstrapper가 Awake에서 모든 서비스를 등록(Register)하며,
/// 이후 어느 클래스에서든 ServiceLocator.Get&lt;T&gt;()로 참조를 가져올 수 있다.
/// </summary>
public static class ServiceLocator
{
    private static Dictionary<Type, object> services = new Dictionary<Type, object>();

    /// <summary> 타입을 키로 서비스 인스턴스를 등록한다. 같은 타입을 재등록하면 덮어씌워진다. </summary>
    public static void Register<T>(T service) where T : class
    {
        services[typeof(T)] = service;
    }

    /// <summary> 등록된 서비스를 타입으로 조회한다. 등록되지 않은 타입이면 예외를 던진다. </summary>
    public static T Get<T>() where T : class
    {
        if (services.TryGetValue(typeof(T), out object service))
            return service as T;

        throw new Exception($"Service {typeof(T).Name} not found. Make sure all services are registered before accessing through the service locator.");
    }

    /// <summary> 씬 전환 등 정리가 필요할 때 서비스를 해제한다. </summary>
    public static void Unregister<T>() where T : class
    {
        services.Remove(typeof(T));
    }
}
