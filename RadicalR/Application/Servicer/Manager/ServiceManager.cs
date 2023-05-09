using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace RadicalR
{
    public class ServiceManager : RepositoryManager, IServiceManager, IAsyncDisposable
    {
        private new bool disposedValue;
        private static IServiceRegistry registry;
        private static IServiceConfiguration configuration;

        protected IServiceScope scope;

        public IServiceProvider Provider => GetProvider();
        public IServiceScope Scope => scope ??= CreateScope();
        public IServiceConfiguration Configuration
        {
            get => configuration;
            set => configuration = value;
        }
        public IServiceRegistry Registry => registry;

        public ServiceManager() : base()
        {
            Services = this;
        }

        internal ServiceManager(IServiceCollection services) : this()
        {
            if (registry == null)
            {
                registry = new ServiceRegistry(services, this);
                registry.MergeServices();
                AddObject<IServiceManager>(this);
                BuildServiceProviderFactory(registry);
            }
            else
                registry.MergeServices(services, false);

            if (configuration == null)
            {
                configuration = new ServiceConfiguration(registry);
                AddObject<IServiceConfiguration>(configuration);
            }
        }

        public virtual IServiceProviderFactory<IServiceCollection> BuildServiceProviderFactory(IServiceRegistry registry)
        {
            var options = new ServiceProviderOptions();

            var factory = new DefaultServiceProviderFactory(options);

            AddObject<IServiceProviderFactory<IServiceCollection>>(factory);
            AddObject<IServiceCollection>(registry);

            registry.Services.Replace(ServiceDescriptor.Singleton<IServiceProviderFactory<IServiceCollection>>(factory));
            registry.Services.Replace(ServiceDescriptor.Singleton<IServiceCollection>(registry));
            registry.MergeServices();

            return factory;
        }

        public virtual T GetService<T>()
        {
            var result = Provider.GetService<T>();
            return result;
        }

        public virtual IEnumerable<T> GetServices<T>()
        {
            return Provider.GetServices<T>();
        }

        public virtual T GetRequiredService<T>()
        {
            return Provider.GetRequiredService<T>();
        }

        public virtual object GetService(Type type)
        {
            return Provider.GetService(type);
        }

        public virtual IEnumerable<object> GetServices(Type type)
        {
            return Provider.GetServices(type);
        }

        public Lazy<T> GetRequiredServiceLazy<T>()
        {
            return new Lazy<T>(GetRequiredService<T>, true);
        }

        public Lazy<T> GetServiceLazy<T>()
        {
            return new Lazy<T>(GetService<T>, true);
        }

        public Lazy<IEnumerable<T>> GetServicesLazy<T>()
        {
            return new Lazy<IEnumerable<T>>(GetServices<T>, true);
        }

        public virtual T GetSingleton<T>() where T : class
        {
            return GetObject<T>();
        }

        public virtual object GetSingleton(Type type)
        {
            return registry.GetObject(type);
        }

        public virtual object GetRequiredService(Type type)
        {
            return Provider.GetRequiredService(type);
        }

        public virtual T NewService<T>(params object[] parameters)
        {
            return ActivatorUtilities.CreateInstance<T>(Provider, parameters);
        }

        public virtual T GetOrNewService<T>()
        {
            return ActivatorUtilities.GetServiceOrCreateInstance<T>(Provider);
        }

        public static void SetProvider(IServiceProvider serviceProvider)
        {
            var _provider = serviceProvider;
            _provider.GetRequiredService<ObjectAccessor<IServiceProvider>>().Value = _provider;
        }

        public static IServiceProvider BuildInternalProvider()
        {
            var provider = ServiceManager.GetRegistry().BuildServiceProviderFromFactory<IServiceCollection>();
            SetProvider(provider);
            return provider;
        }

        public static IServiceProvider GetProvider()
        {
            var _provider = registry.GetProvider();
            if (_provider == null)
                return BuildInternalProvider();

            return _provider;
        }

        public static IServiceProviderFactory<IServiceCollection> GetServiceFactory()
        {
            return GetObject<IServiceProviderFactory<IServiceCollection>>();
        }

        public ObjectFactory NewFactory<T>(Type[] constrTypes)
        {
            return ActivatorUtilities.CreateFactory(typeof(T), constrTypes);
        }

        public ObjectFactory NewFactory(Type instanceType, Type[] constrTypes)
        {
            return ActivatorUtilities.CreateFactory(instanceType, constrTypes);
        }

        public static T GetObject<T>() where T : class
        {
            return registry.GetObject<T>();
        }

        public static T AddObject<T>(T obj) where T : class
        {
            return registry.AddObject<T>(obj).Value;
        }
        public static T AddObject<T>() where T : class
        {
            return registry.AddObject<T>(typeof(T).New<T>()).Value;
        }

        public static IServiceScope CreateScope()
        {
            return GetProvider().CreateScope();
        }

        public static IServiceManager GetManager()
        {
            if (registry == null)
                return new ServiceManager(new ServiceCollection());
            return registry.GetObject<IServiceManager>();
        }

        public static IServiceRegistry GetRegistry()
        {
            if (registry == null)
                return new ServiceManager(new ServiceCollection()).Registry;
            return registry;
        }

        public static IServiceConfiguration GetConfiguration()
        {
            return configuration;
        }

        protected override void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (scope != null)
                        scope.Dispose();
                }
                disposedValue = true;
            }
        }

        public override async ValueTask DisposeAsyncCore()
        {
            await new ValueTask(Task.Run(() =>
            {
                if (scope != null)
                    scope.Dispose();

            })).ConfigureAwait(false);
        }
    }
}
