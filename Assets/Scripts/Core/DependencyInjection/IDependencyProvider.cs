namespace MHL.Core.DependencyInjection
{
    public interface IDependencyProvider
    {
        void RegisterDependencies(IDependencyContainer container);
    }
}