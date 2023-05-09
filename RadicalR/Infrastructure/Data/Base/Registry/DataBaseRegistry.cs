using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Instant;
using System.Reflection;
using System.Series;

namespace RadicalR
{
    public static class DataBaseRegistry
    {
        public static IDeck<IDeck<PropertyInfo>> Properties = new Catalog<IDeck<PropertyInfo>>();
        public static IDeck<MemberRubric[]> Remotes = new Catalog<MemberRubric[]>();
        public static IDeck<Type> Callers = new Catalog<Type>(true);
        public static IMassDeck<IDeck<PropertyInfo[]>> Identities = new MassCatalog<IDeck<PropertyInfo[]>>();
        public static IDeck<IDeck<IEntityType>> Entities = new Catalog<IDeck<IEntityType>>();
        public static IDeck<IDeck<Type>> Contexts = new Catalog<IDeck<Type>>();
        public static IDeck<Type> Stores = new Catalog<Type>();
        public static IDeck<EndpointProvider> Providers = new Catalog<EndpointProvider>();
        public static IDeck<EndpointProvider> Linkers = new Catalog<EndpointProvider>();

        public static IDeck<PropertyInfo> GetDbProperties(this IDataBaseContext context)
        {
            var contextType = context.GetType();

            if (!Properties.TryGet(contextType, out IDeck<PropertyInfo> dbSetProperties))
            {
                dbSetProperties = new Catalog<PropertyInfo>();

                var properties = context.GetType().GetProperties();

                foreach (var property in properties)
                {
                    var setType = property.PropertyType;

                    var isDbSet = setType.IsGenericType && (typeof(IQueryable<>)
                        .IsAssignableFrom(setType.GetGenericTypeDefinition()));

                    if (isDbSet)
                    {
                        var obj = property.GetValue(context, null);
                        var genType = obj.GetType().GenericTypeArguments.FirstOrDefault();
                        dbSetProperties.Put(genType, property);

                    }
                }

                Properties.Put(contextType, dbSetProperties);
            }
            return dbSetProperties;
        }
        public static IDeck<PropertyInfo> GetDbProperties(Type contextType)
        {
            Properties.TryGet(contextType, out IDeck<PropertyInfo> dbSetProperties);
            return dbSetProperties;
        }

        public static IDeck<IEntityType> GetDbEntities(this IDataBaseContext context)
        {
            var contextType = context.GetType();

            if (!Entities.TryGet(contextType, out IDeck<IEntityType> dbEntityTypes))
            {
                dbEntityTypes = new Catalog<IEntityType>();

                var entityTypes = ((DbContext)context).Model.GetEntityTypes();

                var iface = GetDbStore(contextType);

                foreach (var entityType in entityTypes)
                {
                    var clrType = entityType.ClrType;

                    if (clrType != null && clrType.GetInterfaces().Contains(typeof(IEntity)))
                    {
                        dbEntityTypes.Put(clrType.FullName, entityType);

                        if (!Contexts.TryGet(clrType, out IDeck<Type> dbContext))
                            dbContext = new Catalog<Type>();

                        dbContext.Put(iface, contextType);
                        Contexts.Put(clrType, dbContext);
                    }
                }
                Entities.Put(contextType, dbEntityTypes);
                GetDbIndentities(context);
                GetDbLinks(context);
            }

            return dbEntityTypes;
        }
        public static IDeck<IEntityType> GetDbEntities(Type contextType)
        {
            Entities.TryGet(contextType, out IDeck<IEntityType> dbEntityTypes);
            return dbEntityTypes;
        }

        public static IDeck<MemberRubric[]> GetDbLinks(this IDataBaseContext context)
        {
            var contextType = context.GetType();

            var entities = GetDbEntities(context);

            foreach (var entity in entities)
            {
                var remoteProperties = entity.ClrType.GetProperties()
                                            .Where(p => p.CustomAttributes
                                            .Any(a => a.AttributeType == typeof(RemoteAttribute))).ToArray();
                if (remoteProperties.Any())
                {
                    var rubrics = entity.ClrType.ToSleeve().Rubrics;
                    var remoteRubrics = rubrics.ContainsIn(n => n.Name, remoteProperties.Collect(p => p.Name)).ToArray();
                    Remotes.Put(entity.ClrType.FullName, remoteRubrics);
                    remoteRubrics.ForEach((r) =>
                    {
                        Type remoteType = r.RubricType;
                        if (remoteType.IsGenericType)
                        {
                            if (remoteType.IsAssignableTo(typeof(IIdentifiers)))
                                remoteType = remoteType.BaseType;
                            remoteType = remoteType.GetGenericArguments().LastOrDefault();
                        }
                        Callers.Put(remoteType.FullName, entity.ClrType);
                    });
                }
            }

            return Remotes;
        }

        public static Type GetDbStore(this IDataBaseContext context)
        {
            return GetDbStore(context.GetType());
        }

        public static Type GetDbStore(Type contextType)
        {
            if (!Stores.TryGet(contextType, out Type iface))
            {
                var type = contextType.IsGenericType
                    ? contextType
                    : contextType.BaseType;

                iface = type.GenericTypeArguments
                        .Where(i => i
                        .IsAssignableTo(typeof(IDataStore)))
                        .FirstOrDefault();

                if (iface == null)
                    iface = typeof(IDataStore);

                Stores.Put(iface, contextType);
                Stores.Put(contextType, iface);
            }
            return iface;
        }

        public static IDeck<IDeck<PropertyInfo[]>> GetDbIndentities(this IDataBaseContext context)
        {
            if (!Properties.ContainsKey(context.GetType()))
            {
                var entityTypes = ((DbContext)context).Model.GetEntityTypes();
                foreach (var entityType in entityTypes)
                {
                    var dbSetKeys = new Catalog<PropertyInfo[]>();
                    var type = entityType.ClrType;

                    for (int i = 3; i < 12; i += 4)
                    {
                        var _i = i;
                        _i *= 11;

                        var idType = (DbIdentityType)_i;

                        PropertyInfo[] ids = null;

                        switch (idType)
                        {
                            case DbIdentityType.PrimaryKey:
                                ids = entityType.GetKeys().Where(k => k.IsPrimaryKey())
                                    .SelectMany(k => k.Properties.Select(p => p.PropertyInfo)).ToArray();
                                break;
                            case DbIdentityType.Index:
                                ids = entityType.GetIndexes()
                                    .SelectMany(k => k.Properties.Select(p => p.PropertyInfo))
                                    .ToArray();
                                break;
                            case DbIdentityType.ForeignKey:
                                ids = entityType.GetForeignKeys()
                                    .SelectMany(k => k.Properties.Select(p => p.PropertyInfo))
                                    .ToArray();
                                break;
                            default:
                                break;
                        }

                        dbSetKeys.Put((uint)_i, ids);
                    }
                    Identities.Put(type, dbSetKeys);
                }
            }
            return Identities;
        }

        public static PropertyInfo[] GetIdentity(Type entityType, DbIdentityType identityType)
        {
            PropertyInfo[] identity = null;
            GetIdentities(entityType)?.TryGet(identityType, out identity);
            return identity;
        }
        public static PropertyInfo[] GetIdentity(this IUnique entity, DbIdentityType identityType)
        {
            if (Identities.TryGet(entity.GetType(), out IDeck<PropertyInfo[]> dbSetKeys))
                if (dbSetKeys.TryGet(identityType, out PropertyInfo[] keyProperties))
                    return keyProperties;

            return null;
        }

        public static IDeck<PropertyInfo[]> GetIdentities(this IUnique entity)
        {
            if (Identities.TryGet(entity.GetType(), out IDeck<PropertyInfo[]> dbSetKeys))
                return dbSetKeys;

            return null;
        }
        public static IDeck<PropertyInfo[]> GetIdentities(Type entityType)
        {
            Identities.TryGet(entityType, out IDeck<PropertyInfo[]> dbIdentities);
            return dbIdentities;
        }

        public static PropertyInfo GetProperty(this IDataBaseContext context, string entityTypeName)
        {
            return GetProperty(context, Type.GetType(entityTypeName));
        }
        public static PropertyInfo GetProperty(this IDataBaseContext context, Type entityType)
        {
            if (GetDbProperties(context).TryGet(entityType, out PropertyInfo dbSetProperty))
                return dbSetProperty;
            return null;
        }
        public static PropertyInfo GetProperty<TEntity>(this IDataBaseContext context)
        {
            return GetProperty(context, typeof(TEntity));
        }

        public static IEntityType GetEntityType(this IDataBaseContext context, string entityTypeName)
        {
            if (GetDbEntities(context).TryGet(entityTypeName, out IEntityType dbEntityType))
                return dbEntityType;
            return null;
        }
        public static IEntityType GetEntityType(this IDataBaseContext context, Type entityType)
        {
            if (GetDbEntities(context).TryGet(entityType.Name, out IEntityType dbEntityType))
                return dbEntityType;
            return null;
        }
        public static IEntityType GetEntityType<TEntity>(this IDataBaseContext context)
        {
            return GetEntityType(context, typeof(TEntity).Name);
        }

        public static MemberRubric[] GetLinkedMembers(Type entityType)
        {
            if (Remotes.TryGet(entityType.FullName, out MemberRubric[] dbRemote))
                return dbRemote;
            return null;
        }
        public static MemberRubric[] GetLinkedMembers<TOrigin>()
        {
            return GetLinkedMembers(typeof(TOrigin));
        }

        public static MemberRubric GetLinkedMember(Type entityType, Type targetType)
        {
            var remotes = GetLinkedMembers(entityType);
            if (remotes == null)
                return null;
            return remotes.Where(r => (r.RubricType.IsGenericType)
                                ? r.RubricType.GetGenericArguments().Any(ga => ga.Equals(targetType))
                                : r.RubricType.Equals(targetType)).FirstOrDefault();
        }
        public static MemberRubric GetLinkedMember<TOrigin, TTarget>()
        {
            return GetLinkedMember(typeof(TOrigin), typeof(TTarget));
        }

        private static MethodInfo ContextSetEntityMethod { get; set; }

        public static object GetDbSet(this IDataBaseContext context, string entityTypeName)
        {
            var entityType = GetEntityType(context, entityTypeName);
            if (entityType != null)
            {
                var clrType = entityType.ClrType;

                if (clrType != null && clrType.GetInterfaces().Contains(typeof(IEntity)))
                {
                    ContextSetEntityMethod ??= context.GetType().GetMethods().Where(m => m.IsGenericMethod &&
                            m.Name == "Set" && !m.GetParameters().Any()).ToArray().FirstOrDefault().MakeGenericMethod(clrType);

                    object dbset = ContextSetEntityMethod.Invoke(context, null);

                    return dbset;
                }
            }
            return null;
        }
        public static object GetDbSet(this IDataBaseContext context, Type entityType)
        {
            return GetDbSet(context, entityType.Name);
        }

        public static DbSet<TEntity> GetDbSet<TEntity>(this IDataBaseContext context) where TEntity : class, IIdentifiable
        {
            var entityType = GetEntityType<TEntity>(context);
            if (entityType != null)
                return (DbSet<TEntity>)context.EntitySet<TEntity>();
            return null;
        }

        public static Type GetContext(Type storeType, Type entityType)
        {
            if (Contexts.TryGet(entityType.Name, out IDeck<Type> dbEntityContext))
            {
                var iface = storeType
                    .GetInterfaces()
                    .Where(i => i.GetInterfaces()
                        .Contains(typeof(IDataStore))).FirstOrDefault();

                if (iface == null && storeType == typeof(IDataStore))
                    iface = typeof(IDataStore);

                if (dbEntityContext.TryGet(storeType, out Type contextType))
                    return contextType;
            }

            return null;
        }
        public static Type GetContext<TStore, TEntity>() where TEntity : Entity
        {
            if (Contexts.TryGet(typeof(TEntity), out IDeck<Type> dbEntityContext))
            {
                var iface = typeof(TStore)
                            .GetInterfaces()
                            .Where(i => i.GetInterfaces()
                                .Contains(typeof(IDataStore))).FirstOrDefault();

                if (iface == null && typeof(TStore) == typeof(IDataStore))
                    iface = typeof(IDataStore);

                if (dbEntityContext.TryGet(typeof(TStore), out Type contextType))
                    return contextType;
            }

            return null;
        }
        public static Type[] GetContexts<TEntity>() where TEntity : Entity
        {
            if (Contexts.TryGet(typeof(TEntity), out IDeck<Type> dbEntityContext))
                return dbEntityContext.ToArray();
            return null;
        }

        private static string PrimaryKey(this EntityEntry entry)
        {
            var key = entry.Metadata.FindPrimaryKey();

            var values = new List<object>();
            foreach (var property in key.Properties)
            {
                var value = entry.Property(property.Name).CurrentValue;
                if (value != null)
                {
                    values.Add(value);
                }
            }

            return string.Join(",", values);
        }
    }

    public enum DbIdentityType
    {
        NONE = 0,
        PrimaryKey = 33,
        Index = 77,
        ForeignKey = 121
    }
}
