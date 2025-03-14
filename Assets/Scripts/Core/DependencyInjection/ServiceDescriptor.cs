using System;

namespace MHL.Core.DependencyInjection
{
    public class ServiceDescriptor
    {
        public Type ImplementationType { get; }
        public ServiceLifetime Lifetime { get; }
        
        public ServiceDescriptor(Type implementationType, ServiceLifetime lifetime)
        {
            ImplementationType = implementationType;
            Lifetime = lifetime;
        }
    }
}