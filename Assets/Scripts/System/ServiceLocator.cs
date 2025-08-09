using System;
using System.Collections.Generic;
using UnityEngine;

public static class ServiceLocator
{
    // 등록된 서비스들을 전역적으로 사용할 수 있게 도와주는 클래스 
    // Bootstrapper 클래스에서 이를 등록해야함 
    private static Dictionary<Type, object> services = new Dictionary<Type, object>(); 

    public static void Register<T>(T service) where T: class
    {
        services[typeof(T)] = service; 
    }

    public static T Get<T>() where T : class
    {
        if(services.TryGetValue(typeof(T), out object service))
            return service as T;

        throw new Exception($"Service {typeof(T).Name} not found. Make sure all services are registered before accessing through the service locator.");
    }

    public static void Unregister<T>() where T: class
    {
        services.Remove(typeof(T)); 
    }
}
