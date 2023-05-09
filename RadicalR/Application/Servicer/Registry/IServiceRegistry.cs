using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using System.Series;

namespace RadicalR
{
    public interface IServiceRegistry : IServiceCollection
    {
        ServiceDescriptor this[string name] { get; set; }
        ServiceDescriptor this[Type serviceType] { get; set; }

        IServiceManager Manager { get; }
        IServiceCollection Services { get; }

        ObjectAccessor<T> AddObject<T>() where T : class;
        ObjectAccessor<T> AddObject<T>(ObjectAccessor<T> accessor) where T : class;
        ObjectAccessor<T> AddObject<T>(T obj) where T : class;
        ObjectAccessor AddObject(Type type, object obj);
        ObjectAccessor AddObject(Type type);
        IServiceProvider BuildServiceProviderFromFactory();
        IServiceProvider BuildServiceProviderFromFactory<TContainerBuilder>([NotNull] Action<TContainerBuilder> builderAction = null);
        bool ContainsKey(Type type);
        bool ContainsKey<TService>();
        ServiceDescriptor Get(Type contextType);
        ServiceDescriptor Get<TService>() where TService : class;
        ulong GetKey(ServiceDescriptor item);
        ulong GetKey(string item);
        ulong GetKey(Type item);
        T GetObject<T>() where T : class;
        object GetObject(Type type);
        IServiceProvider GetProvider();
        T GetRequiredObject<T>() where T : class;
        object GetRequiredService(Type type);
        T GetRequiredService<T>() where T : class;
        Lazy<object> GetRequiredServiceLazy(Type type);
        Lazy<T> GetRequiredServiceLazy<T>() where T : class;
        T GetRequiredSingleton<T>() where T : class;
        Lazy<object> GetServiceLazy(Type type);
        Lazy<T> GetServiceLazy<T>() where T : class;
        T GetSingleton<T>() where T : class;
        bool IsAdded(Type type);
        bool IsAdded<T>() where T : class;
        void MergeServices(bool actualizeExternalServices = true);
        void MergeServices(IServiceCollection services, bool actualizeExternalServices = true);
        bool Remove<TContext>() where TContext : class;
        ICard<ServiceDescriptor> Set(ServiceDescriptor descriptor);
        bool TryAdd(ServiceDescriptor profile);
        ObjectAccessor<T> TryAddObject<T>() where T : class;
        ObjectAccessor TryAddObject(Type type);
        bool TryGet<TService>(out ServiceDescriptor profile) where TService : class;
    }
}