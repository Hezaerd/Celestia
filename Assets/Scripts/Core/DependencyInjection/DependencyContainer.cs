using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MHL.Core.DependencyInjection
{
    public class DependencyContainer : IDependencyContainer
    {
        private readonly Dictionary<Type, ServiceDescriptor> _services = new Dictionary<Type, ServiceDescriptor>();
        private readonly Dictionary<Type, object> _singletons = new Dictionary<Type, object>();
        
        /// <summary>
        /// Registers a MonoBehaviour type as a singleton. If an instance exists in the scene,
        /// it will be used. Otherwise, a new GameObject with this component will be created.
        /// </summary>
        /// <typeparam name="T">The MonoBehaviour type to register</typeparam>
        /// <returns>The instance that was created or found</returns>
        public T RegisterSingleton<T>() where T : MonoBehaviour
        {
            return (T)Register(typeof(T), typeof(T), ServiceLifetime.Singleton);
        }
        
        /// <summary>
        /// Registers an existing MonoBehaviour instance as a singleton.
        /// This specific instance will be provided whenever this type is resolved.
        /// </summary>
        /// <typeparam name="T">The MonoBehaviour type to register</typeparam>
        /// <param name="instance">The specific instance to use for this registration</param>
        /// <returns>The registered instance</returns>
        public T RegisterSingleton<T>(T instance) where T : MonoBehaviour
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance), "Cannot register a null instance as a singleton");
            
            Type type = typeof(T);
            _services[type] = new ServiceDescriptor(type, ServiceLifetime.Singleton);
            _singletons[type] = instance;
            
            return instance;
        }
        
        /// <summary>
        /// Registers a non-MonoBehaviour implementation for an interface as a singleton.
        /// The same instance will be provided for all resolutions of this type.
        /// </summary>
        /// <typeparam name="TInterface">The interface type to register</typeparam>
        /// <typeparam name="TImplementation">The implementation type that will be instantiated</typeparam>
        /// <returns>The created instance</returns>
        public TInterface RegisterSingleton<TInterface, TImplementation>() where TImplementation : class, TInterface
        {
            return (TInterface)Register(typeof(TInterface), typeof(TImplementation), ServiceLifetime.Singleton);
        }
        
        /// <summary>
        /// Registers a MonoBehaviour type as transient. Each time this type is resolved,
        /// a new GameObject with this component will be created.
        /// </summary>
        /// <typeparam name="T">The MonoBehaviour type to register</typeparam>
        /// <returns>The first created instance</returns>
        public T RegisterTransient<T>() where T : MonoBehaviour
        {
            return (T)Register(typeof(T), typeof(T), ServiceLifetime.Transient);
        }
        
        /// <summary>
        /// Registers a MonoBehaviour instance type as transient.
        /// </summary>
        /// <typeparam name="T">The MonoBehaviour type to register</typeparam>
        /// <param name="instance">A reference instance (not stored)</param>
        /// <returns>The instance</returns>
        public T RegisterTransient<T>(T instance) where T : MonoBehaviour
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance), "Cannot register a null instance as a transient");
            
            Type type = typeof(T);
            _services[type] = new ServiceDescriptor(type, ServiceLifetime.Transient);
            
            // For transient registrations, we don't store the instance
            // but we return it for convenience
            return instance;
        }
        
        /// <summary>
        /// Registers a non-MonoBehaviour implementation for an interface as transient.
        /// A new instance will be created each time this type is resolved.
        /// </summary>
        /// <typeparam name="TInterface">The interface type to register</typeparam>
        /// <typeparam name="TImplementation">The implementation type that will be instantiated</typeparam>
        /// <returns>The first created instance</returns>
        public TInterface RegisterTransient<TInterface, TImplementation>() where TImplementation : class, TInterface
        {
            return (TInterface)Register(typeof(TInterface), typeof(TImplementation), ServiceLifetime.Transient);
        }
        
        /// <summary>
        /// Internal method to register a type with the container.
        /// </summary>
        /// <param name="serviceType">The type to register (often an interface)</param>
        /// <param name="implementationType">The implementation type that will be instantiated</param>
        /// <param name="lifetime">The lifetime of the registration (Singleton or Transient)</param>
        /// <returns>The instance that was created</returns>
        private object Register(Type serviceType, Type implementationType, ServiceLifetime lifetime)
        {
            if (_services.ContainsKey(serviceType))
                throw new InvalidOperationException($"Service of type {serviceType} already registered");
            
            _services[serviceType] = new ServiceDescriptor(implementationType, lifetime);
            
            // Create an instance if this is a immediately
            var instance = CreateInstance(implementationType);
            
            // Store the instance if this is a singleton
            if (lifetime == ServiceLifetime.Singleton)
                _singletons[serviceType] = instance;
            
            return instance;
        }
        
        /// <summary>
        /// Resolves a dependency from the container.
        /// </summary>
        /// <typeparam name="T">The type to resolve</typeparam>
        /// <returns>An instance of the requested type</returns>
        /// <exception cref="ArgumentException">Thrown when the requested type is not registered</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the descriptor lifetime isn't supported</exception>
        public T Resolve<T>()
        {
            return (T)Resolve(typeof(T));
        }
        
        /// <summary>
        /// Resolves a dependency from the container using Type information.
        /// </summary>
        /// <param name="type">The type to resolve</param>
        /// <returns>An instance of the requested type</returns>
        /// <exception cref="ArgumentException">Thrown when the requested type is not registered</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the descriptor lifetime isn't supported</exception>
        public object Resolve(Type type)
        {
            if (!_services.TryGetValue(type, out ServiceDescriptor descriptor))
                throw new ArgumentException($"Type {type} is not registered with the container");
            
            switch (descriptor.Lifetime)
            {
                case ServiceLifetime.Singleton:
                    if (!_singletons.ContainsKey(type))
                        _singletons[type] = CreateInstance(descriptor.ImplementationType);
                    
                    return _singletons[type];
                
                case ServiceLifetime.Transient:
                    return CreateInstance(descriptor.ImplementationType);
                
                default:
                    throw new ArgumentOutOfRangeException($"Unsupported lifetime {descriptor.Lifetime}");
            }
        }
        
        /// <summary>
        /// Creates an instance of the specified type, handling both MonoBehaviours and regular classes.
        /// </summary>
        /// <param name="type">The type to instantiate</param>
        /// <returns>A new instance of the specified type</returns>
        /// <exception cref="InvalidOperationException">Thrown when a regular class has no usable constructor</exception>
        private object CreateInstance(Type type)
        {
            return typeof(MonoBehaviour).IsAssignableFrom(type) ? CreateMonoBehaviourInstance(type) : CreateClassInstance(type);
        }
        
        private static object CreateMonoBehaviourInstance(Type type)
        {
            // Check if an instance of this type already exists in the scene
            MonoBehaviour exisingInstance = Object.FindFirstObjectByType(type) as MonoBehaviour;
            
            if (exisingInstance != null)
                return exisingInstance;
            
            // Create a new GameObject with this component attached
            GameObject gameObject = new GameObject(type.Name);
            return gameObject.AddComponent(type);
        }
        
        private object CreateClassInstance(Type type)
        {
            ConstructorInfo constructor = type.GetConstructors().FirstOrDefault();
            
            if (constructor == null)
                throw new InvalidOperationException($"Type {type} has no usable constructor");
            
            var parameters = constructor.GetParameters();
            var parameterValues = new object[parameters.Length];
            
            for (var i = 0; i < parameters.Length; i++)
                parameterValues[i] = Resolve(parameters[i].ParameterType);
            
            return Activator.CreateInstance(type, parameterValues);
        }
    }
}