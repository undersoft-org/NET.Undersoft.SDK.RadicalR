using Microsoft.EntityFrameworkCore;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Csdl;
using System.Logs;
using System.Series;
using System.Xml;

namespace RadicalR
{
    public static class OpenClientRegistry
    {
        const int DS_TRYOUTS = 60;

        public static IDeck<IDeck<IEdmEntityType>> Entities = new Catalog<IDeck<IEdmEntityType>>();
        public static IDeck<Type> Mappings = new Catalog<Type>();
        public static IDeck<IDeck<Type>> Contexts = new Catalog<IDeck<Type>>();
        public static IDeck<IEdmModel> EdmModels = new Catalog<IEdmModel>();
        public static IDeck<Type> Stores = new Catalog<Type>();
        public static IDeck<RemoteLink> Relations = new Catalog<RemoteLink>(true);

        public static IEdmModel GetEdmModel(this DataClientContext context)
        {
            Task<IEdmModel> model = GetEdmModelAsync(context);
            model.Wait();
            return model.Result;
        }

        public static async Task<IEdmModel> GetEdmModelAsync(this DataClientContext context)
        {
            // Get the service metadata's Uri
            var metadataUri = context.GetMetadataUri();
            // Create a HTTP request to the metadata's Uri 
            // in order to get a representation for the data model
            HttpClientHandler clientHandler = new HttpClientHandler();
            using (HttpClient client = new HttpClient(clientHandler))
            {
                if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
                {
                    clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) =>
                    {
                        return true;
                    };
                }
                int tryouts = DS_TRYOUTS;
                do
                {
                    try
                    {
                        using (var response = await client.GetAsync(metadataUri))
                        {
                            client.Success<Weblog>("Data Service Client - Connected");

                            if (response.IsSuccessStatusCode)
                            {
                                client.Success<Datalog>("Data Service Client - Metadata Retrieved");
                                // Translate the response into an in-memory stream
                                using (var stream = await response.Content.ReadAsStreamAsync())
                                {   // Convert the stream into an XML representation
                                    using (var reader = XmlReader.Create(stream))
                                    {   // Parse the XML representation of the data model
                                        // into an EDM that can be utilized by OData client libraries
                                        return CsdlReader.Parse(reader);
                                    }
                                }
                            }
                            else
                            {
                                tryouts--;
                                client.Warning<Weblog>("Data Service Client - Http Get Metadata Request Failed", response);
                                Thread.Sleep(5000);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        tryouts--;
                        client.Warning<Datalog>("Data Service Client - Http Connection Failed", client.BaseAddress, ex);
                        Thread.Sleep(5000);
                    }
                }
                while (tryouts > 0);

                client.Warning<Datalog>("Data Service Client - Retry Connection Limit Exceeded " +
                                        "- Unable To Retrieve Metadata From Endpoint", client.BaseAddress);
            }
            return null;
        }

        public static IDeck<IEdmEntityType> GetEdmEntityTypes(this DataClientContext context)
        {
            var contextType = context.GetType();

            if (!Entities.TryGet(contextType, out IDeck<IEdmEntityType> dsEntities))
            {
                dsEntities = new Catalog<IEdmEntityType>();

                var entityTypes = context.GetServiceModel()
                                                .EntityContainer
                                                .EntitySets()
                                                .Select(p => p.EntityType())
                                                .ToArray();

                var iface = GetRemoteStore(contextType);

                foreach (var entityType in entityTypes)
                {
                    dsEntities.Add(entityType.Name, entityType);

                    var localEntityType = EdmAssemblyResolve(entityType);

                    dsEntities.Add(localEntityType.FullName, entityType);

                    if (!Contexts.TryGet(entityType.Name, out IDeck<Type> dsEntityContext))
                        dsEntityContext = new Catalog<Type>();

                    dsEntityContext.Put(iface, contextType);
                    Contexts.Put(entityType.Name, dsEntityContext);
                }
                Entities.Add(contextType, dsEntities);
            }
            return dsEntities;
        }

        public static Type GetRemoteStore(this DataClientContext context)
        {
            return GetRemoteStore(context.GetType());
        }
        public static Type GetRemoteStore(Type contextType)
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

        public static Type GetMappedType(this DataClientContext context, string name)
        {
            string sn = name.Split('.').Last();
            if (Mappings.TryGet(name, out Type t) ||
                Mappings.TryGet(sn, out t))
                return t;
            return Assemblies.FindType(sn);
        }
        public static string GetMappedName(this DataClientContext context, Type type)
        {
            string n = type.FullName;
            if (Entities.TryGet(context.GetType(), out IDeck<IEdmEntityType> deck))
                if (deck.TryGet(type.Name, out IEdmEntityType et))
                    n = et.FullTypeName();
            return n;
        }

        public static Type GetContext<TStore, TEntity>() where TEntity : class, IIdentifiable
        {
            return GetContext(typeof(TStore), typeof(TEntity));
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
        public static Type[] GetContexts<TEntity>() where TEntity : class, IIdentifiable
        {
            if (Contexts.TryGet(typeof(TEntity).Name, out IDeck<Type> dbEntityContext))
                return dbEntityContext.ToArray();
            return null;
        }

        private static Type EdmAssemblyResolve(IEdmEntityType entityType)
        {
            var remoteName = entityType.Name;
            var remoteFullName = entityType.FullTypeName();
            Type localEntityType = null;
            if (remoteName.Contains("Identifier"))
            {
                var entityName = remoteName.Replace("IdentifierDto", null);
                var argumentType = Assemblies.FindType(entityName);
                localEntityType = typeof(IdentifierDto<>).MakeGenericType(argumentType);
            }
            else
            {
                localEntityType = Assemblies.FindType(remoteName);
            }

            if (localEntityType == null)
                return null;

            Mappings.Put(remoteName, localEntityType);
            Mappings.Put(remoteFullName, localEntityType);

            return localEntityType;
        }
    }
}
