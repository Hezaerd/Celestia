using System;
using System.Reflection;
using UnityEngine;

namespace MHL.Tools.DI
{
    [DefaultExecutionOrder(-100)]
    public abstract class MonoProvider : MonoBehaviour, IDependencyProvider
    {
        private DependencyContainer Container { get; set; }
        
        private const BindingFlags BindingFlags = 
            System.Reflection.BindingFlags.Public | 
            System.Reflection.BindingFlags.NonPublic | 
            System.Reflection.BindingFlags.Instance;
        
        public virtual void Awake()
        {
            Container = new DependencyContainer();
            RegisterDependencies(Container);
            
            // Inject dependencies into scene
            var monoBehaviours = FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            
            foreach(MonoBehaviour monoBehaviour in monoBehaviours)
            {
                Type type = monoBehaviour.GetType();
                
                // Inject fields
                InjectFields(monoBehaviour, type);
                
                // Inject properties
                InjectProperties(monoBehaviour, type);
                
                // Inject methods
                InjectMethods(monoBehaviour, type);
            }
        }
        
        public abstract void RegisterDependencies(IDependencyContainer container);
        
        private void InjectFields(MonoBehaviour monoBehaviour, Type type)
        {
            var fields = type.GetFields(BindingFlags);
            
            foreach (FieldInfo field in fields)
            {
                InjectAttribute injectAttribute = field.GetCustomAttribute<InjectAttribute>();
                
                if (injectAttribute == null)
                    continue;
                
                try
                {
                    var dependency = Container.Resolve(field.FieldType);
                    field.SetValue(monoBehaviour, dependency);
                }
                catch (Exception e)
                {
                    Debug.LogError($"Failed to inject dependency into field {field.Name} of type {field.FieldType} in {type.Name}. {e.Message}");
                }
            }
        }
        
        private void InjectProperties(MonoBehaviour monoBehaviour, Type type)
        {
            var properties = type.GetProperties(BindingFlags);
            
            foreach (PropertyInfo property in properties)
            {
                InjectAttribute injectAttribute = property.GetCustomAttribute<InjectAttribute>();
                
                if (injectAttribute == null)
                    continue;
                
                if (property.CanWrite == false)
                {
                    Debug.LogError($"Failed to inject dependency into property {property.Name} of type {property.PropertyType} in {type.Name}: Property is read-only");
                    continue;
                }
                
                try
                {
                    var dependency = Container.Resolve(property.PropertyType);
                    property.SetValue(monoBehaviour, dependency);
                }
                catch (Exception e)
                {
                    Debug.LogError($"Failed to inject dependency into property {property.Name} of type {property.PropertyType} in {type.Name}. {e.Message}");
                }
            }
        }
        
        private void InjectMethods(MonoBehaviour monoBehaviour, Type type)
        {
            var methods = type.GetMethods(BindingFlags);
            
            foreach (MethodInfo method in methods)
            {
                InjectAttribute injectAttribute = method.GetCustomAttribute<InjectAttribute>();
                
                if (injectAttribute == null)
                    continue;
                
                var parameters = method.GetParameters();
                var arguments = new object[parameters.Length];
                
                for (var i = 0; i < parameters.Length; i++)
                {
                    try
                    {
                        arguments[i] = Container.Resolve(parameters[i].ParameterType);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"Failed to resolve parameter {parameters[i].Name} of type {parameters[i].ParameterType} for method {method.Name} in {type.Name}: {e.Message}");
                    }
                }
                
                try
                {
                    method.Invoke(monoBehaviour, arguments);
                }
                catch (Exception e)
                {
                    Debug.LogError($"Failed to invoke method {method.Name} in {type.Name}: {e.Message}");
                }
            }
        }
    }
}