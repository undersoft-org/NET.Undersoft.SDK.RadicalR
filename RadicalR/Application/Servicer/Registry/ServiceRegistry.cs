using Microsoft.Extensions.DependencyInjection;
using System.Series;
using System.Uniques;

namespace RadicalR
{
    public partial class ServiceRegistry : Catalog<ServiceDescriptor>, IServiceRegistry
    {
        public ServiceRegistry() : base(true)
        {
            AddObject<IServiceProvider>();
            AddObject<IServiceRegistry>(this);
        }

        public ServiceRegistry(IServiceCollection services, IServiceManager manager) : this()
        {
            Services = services;
            Manager = manager;
        }

        public IServiceCollection Services { get; }
        public IServiceManager Manager { get; }

        public ServiceDescriptor this[string name]
        {
            get => base[GetKey(name)];
            set => base.Set(GetKey(name), value);
        }
        public ServiceDescriptor this[Type serviceType]
        {
            get => base[GetKey(serviceType)];
            set => base.Set(GetKey(serviceType), value);
        }

        public ServiceDescriptor Get(Type contextType)
        {
            return base.Get(GetKey(contextType));
        }

        public ServiceDescriptor Get<TService>() where TService : class
        {
            return base.Get(GetKey<TService>());
        }

        public bool TryGet<TService>(out ServiceDescriptor profile) where TService : class
        {
            if (base.TryGet(GetKey<TService>(), out profile))
                return true;
            return false;
        }

        public override bool TryAdd(ServiceDescriptor profile)
        {
            ulong key = GetKey(profile.ServiceType);
            if (ContainsKey(key))
                return false;
            base.Add(key, profile);
            return true;
        }

        public bool Remove<TContext>() where TContext : class
        {
            return TryRemove(typeof(TContext));
        }

        protected override bool InnerAdd(ServiceDescriptor value)
        {
            var temp = base.InnerAdd(GetKey(value.ServiceType), value);
            return temp;
        }

        protected override ICard<ServiceDescriptor> InnerPut(ServiceDescriptor value)
        {
            var temp = base.InnerPut(GetKey(value.ServiceType), value);
            return temp;
        }

        public override ICard<ServiceDescriptor> Set(ServiceDescriptor descriptor)
        {
            return base.Set(GetKey(descriptor.ServiceType), descriptor);
        }

        public override void Add(ServiceDescriptor item)
        {
            base.Add(item);
        }

        public ulong GetKey(ServiceDescriptor item)
        {
            return item.ServiceType.FullName.UniqueKey64();
        }

        public ulong GetKey(Type item)
        {
            return item.FullName.UniqueKey64();
        }

        public ulong GetKey(string item)
        {
            return item.UniqueKey64();
        }

        public ulong GetKey<T>()
        {
            return typeof(T).FullName.UniqueKey64();
        }

        protected override void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    foreach (var service in this)
                        service.Dispose();
                    base.Dispose(true);
                }
                disposedValue = true;
            }
        }

        public override int IndexOf(ServiceDescriptor item)
        {
            return base.IndexOf(GetKey(item), item);
        }

        public override void Insert(int index, ServiceDescriptor item)
        {
            base.Insert(index, GetCard(GetKey(item), item));
        }

        public override bool Contains(ServiceDescriptor item)
        {
            return base.Contains(GetKey(item), item);
        }

        public override void CopyTo(ServiceDescriptor[] array, int arrayIndex)
        {
            base.CopyTo(array, arrayIndex);
        }

        public override bool Remove(ServiceDescriptor item)
        {
            return base.TryRemove(GetKey(item.ServiceType), item);
        }

        public bool ContainsKey<TService>()
        {
            return base.ContainsKey(GetKey<TService>());
        }

        public bool ContainsKey(Type type)
        {
            return base.ContainsKey(GetKey(type));
        }

        public void MergeServices(bool actualizeExternalServices = true)
        {
            if (Services.Count == Count)
                return;

            var sdeck = new Album<ServiceDescriptor>(true);

            Services.ForEach(s =>
            {
                sdeck.Add(GetKey(s.ServiceType), s);
                if (!Contains(s))
                {
                    Add(s);
                }
            });
            if (actualizeExternalServices)
                this.ForEach(c =>
                {
                    if (!sdeck.Contains(GetKey(c.ServiceType), c))
                    {
                        Services.Add(c);
                    }
                });
        }
        public void MergeServices(IServiceCollection services, bool actualizeExternalServices = true)
        {
            if (services.Count == Count)
                return;

            var sdeck = new Album<ServiceDescriptor>(true);

            services.ForEach(s =>
            {
                if (actualizeExternalServices)
                    sdeck.Add(GetKey(s.ServiceType), s);
                if (!Contains(s))
                {
                    Add(s);
                }
            });
            if (actualizeExternalServices)
                this.ForEach(c =>
                {
                    if (!sdeck.Contains(GetKey(c.ServiceType), c))
                    {
                        services.Add(c);
                    }
                });
        }
    }
}
