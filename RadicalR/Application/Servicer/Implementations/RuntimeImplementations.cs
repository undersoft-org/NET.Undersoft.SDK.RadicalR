using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.Edm;
using System;
using System.Collections.Generic;
using System.Series;

namespace RadicalR
{
    public partial class AppSetup
    {
        public static void AddRuntimeImplementations()
        {
            IServiceManager sm = ServiceManager.GetManager();
            IServiceRegistry service = sm.Registry;
            HashSet<Type> duplicateCheck = new HashSet<Type>();
            Type[] stores = new Type[] { typeof(IEntryStore), typeof(IReportStore) };

            /**************************************** DataService Entity Type Routines ***************************************/
            foreach (IDeck<IEdmEntityType> contextEntityTypes in OpenClientRegistry.Entities)
            {
                foreach (IEdmEntityType _entityType in contextEntityTypes)
                {
                    Type entityType = OpenClientRegistry.Mappings[_entityType.Name];

                    if (duplicateCheck.Add(entityType))
                    {
                        Type callerType = DataBaseRegistry.Callers[entityType.FullName];

                        /*****************************************************************************************/
                        foreach (Type store in stores)
                        {
                            if ((entityType != null) && (OpenClientRegistry.GetContext(store, entityType) != null))
                            {
                                /*****************************************************************************************/
                                service.AddScoped(
                                    typeof(IRemoteRepository<,>).MakeGenericType(store, entityType),
                                    typeof(RemoteRepository<,>).MakeGenericType(store, entityType));

                                service.AddScoped(
                                    typeof(IEntityCache<,>).MakeGenericType(store, entityType),
                                    typeof(EntityCache<,>).MakeGenericType(store, entityType));
                                /*****************************************************************************************/
                                service.AddScoped(
                                    typeof(IRemote<,>).MakeGenericType(store, entityType),
                                    typeof(RemoteSet<,>).MakeGenericType(store, entityType));
                                /*****************************************************************************************/
                                if (callerType != null)
                                {
                                    /*********************************************************************************************/
                                    service.AddScoped(
                                        typeof(IRemoteRepositoryLink<,,>).MakeGenericType(store, callerType, entityType),
                                        typeof(RemoteRepositoryLink<,,>).MakeGenericType(store, callerType, entityType));

                                    service.AddScoped(
                                        typeof(ILinkedObject<,>).MakeGenericType(store, callerType),
                                        typeof(RemoteRepositoryLink<,,>).MakeGenericType(store, callerType, entityType));
                                    /*********************************************************************************************/
                                }
                            }
                        }
                    }
                }
            }
            //app.RebuildProviders();
        }
    }
}