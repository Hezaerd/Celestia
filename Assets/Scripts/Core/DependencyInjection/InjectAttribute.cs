using System;

namespace MHL.Core.DependencyInjection
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method)]
    public class InjectAttribute : Attribute
    {
    }
}