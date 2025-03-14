using UnityEngine;

namespace MHL.Tools.DI
{
    public interface IDependencyContainer
    {
        // Singleton
        T RegisterSingleton<T>() where T : MonoBehaviour;
        T RegisterSingleton<T>(T instance) where T : MonoBehaviour;
        TInterface RegisterSingleton<TInterface, TImplementation>() where TImplementation : class, TInterface;
        
        // Transient
        T RegisterTransient<T>() where T : MonoBehaviour;
        T RegisterTransient<T>(T instance) where T : MonoBehaviour;
        TInterface RegisterTransient<TInterface, TImplementation>() where TImplementation : class, TInterface;
        
        // Resolve
        T Resolve<T>();
    }
}