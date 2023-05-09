using Microsoft.Extensions.DependencyInjection;

namespace RadicalR
{
    public partial class ServiceRegistry
    {
        public ObjectAccessor<T> TryAddObject<T>() where T : class
        {
            if (ContainsKey(typeof(ObjectAccessor<T>)))
            {
                return (ObjectAccessor<T>)Get<ObjectAccessor<T>>()?.ImplementationInstance;
            }

            return AddObject<T>();
        }

        public ObjectAccessor TryAddObject(Type type)
        {
            Type accessorType = typeof(ObjectAccessor<>).MakeGenericType(type);
            if (ContainsKey(accessorType))
            {
                return (ObjectAccessor)Get(accessorType)?.ImplementationInstance;
            }

            return AddObject(type);
        }

        public ObjectAccessor<T> AddObject<T>() where T : class
        {
            return AddObject(new ObjectAccessor<T>());
        }

        public ObjectAccessor AddObject(Type type)
        {
            Type oaType = typeof(ObjectAccessor<>).MakeGenericType(type);
            Type iaType = typeof(IObjectAccessor<>).MakeGenericType(type);

            ObjectAccessor accessor = (ObjectAccessor)oaType.New();

            if (ContainsKey(oaType))
            {
                return accessor;
            }

            Put(ServiceDescriptor.Singleton(oaType), accessor);
            Put(ServiceDescriptor.Singleton(iaType), accessor);

            return accessor;
        }

        public ObjectAccessor AddObject(Type type, object obj)
        {
            Type oaType = typeof(ObjectAccessor<>).MakeGenericType(type);
            Type iaType = typeof(IObjectAccessor<>).MakeGenericType(type);

            ObjectAccessor accessor = (ObjectAccessor)oaType.New(obj);

            if (ContainsKey(oaType))
            {
                return accessor;
            }

            Put(ServiceDescriptor.Singleton(oaType), accessor);
            Put(ServiceDescriptor.Singleton(iaType), accessor);

            if (obj != null)
                this.AddSingleton(type, obj);

            return accessor;
        }

        public ObjectAccessor<T> AddObject<T>(T obj) where T : class
        {
            return AddObject(new ObjectAccessor<T>(obj));
        }

        public ObjectAccessor<T> AddObject<T>(ObjectAccessor<T> accessor) where T : class
        {
            if (ContainsKey(typeof(ObjectAccessor<T>)))
            {
                return accessor;
            }

            Put(ServiceDescriptor.Singleton(typeof(ObjectAccessor<T>), accessor));
            Put(ServiceDescriptor.Singleton(typeof(IObjectAccessor<T>), accessor));

            if (accessor.Value != null)
                this.AddSingleton<T>(accessor.Value);

            return accessor;
        }

        public object GetObject(Type type)
        {
            Type accessorType = typeof(IObjectAccessor<>).MakeGenericType(type);
            return ((ObjectAccessor)GetSingleton(accessorType))?.Value;
        }

        public T GetObject<T>()
            where T : class
        {
            return GetSingleton<IObjectAccessor<T>>()?.Value;
        }

        public T GetRequiredObject<T>()
            where T : class
        {
            return GetObject<T>() ?? throw new Exception($"Could not find an object of {typeof(T).AssemblyQualifiedName} in  Be sure that you have used AddObjectAccessor before!");
        }
    }
}