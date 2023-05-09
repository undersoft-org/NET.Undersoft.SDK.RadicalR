using AutoMapper;
using FluentValidation;
using IdentityModel;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Net.Http.Headers;
using MongoDB.Driver;
using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using ProtoBuf.Grpc.Configuration;
using ProtoBuf.Grpc.Server;
using System.Diagnostics.Metrics;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RadicalR
{
    public partial class ServiceSetup : IServiceSetup
    {
        protected string[] apiVersions = new string[1] { "1" };
        protected Assembly[] Assemblies;
        protected IMvcBuilder mvc;

        protected IServiceConfiguration configuration => manager.Configuration;
        protected IServiceManager manager { get; }
        protected IServiceRegistry registry { get; set; }
        protected IServiceCollection services => registry.Services;

        public ServiceSetup(IServiceCollection services, IMvcBuilder mvcBuilder = null)
        {
            manager = new ServiceManager(services);
            registry = manager.Registry;
            registry.MergeServices();
            if (mvcBuilder != null)
                mvc = mvcBuilder;
            else
                mvc = services.AddControllers();
            registry.MergeServices(mvc.Services);
        }

        public ServiceSetup(IServiceCollection services, IConfiguration configuration)
            : this(services)
        {
            manager.Configuration = new ServiceConfiguration(configuration, services);
        }     
      
        public IServiceRegistry Services => registry;

        public IServiceSetup AddCaching()
        {
            registry.AddObject(GlobalCache.Catalog);

            Type[] stores = new Type[]
            {
                typeof(IEntryStore),
                typeof(IReportStore),
                typeof(IEventStore)
            };
            foreach (Type item in stores)
            {
                AddStoreCache(item);
            }

            return this;
        }

        public void AddJsonSerializerDefaults()
        {
#if NET6_0            
            var newopts = new JsonSerializerOptions();
            newopts.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            newopts.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            newopts.NumberHandling = JsonNumberHandling.AllowReadingFromString;
            newopts.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));

            var fld = (                
                    typeof(JsonSerializerOptions)
                        .GetField(
                            "s_defaultOptions",
                            System.Reflection.BindingFlags.Static
                                | System.Reflection.BindingFlags.NonPublic
                        ));

            var opt = (JsonSerializerOptions)fld.GetValue(newopts);
            if (opt == null)
                fld.SetValue(newopts, newopts);
            else
                manager.Mapper.Map(newopts, opt);
#endif
#if NET7_0
            var flds = typeof(JsonSerializerOptions).GetRuntimeFields();
            flds.Single(f => f.Name == "_defaultIgnoreCondition")
                .SetValue(JsonSerializerOptions.Default, JsonIgnoreCondition.WhenWritingNull);
            flds.Single(f => f.Name == "_referenceHandler")
                .SetValue(JsonSerializerOptions.Default, ReferenceHandler.IgnoreCycles);          
#endif
        }

        public IServiceSetup AddOpenTelemetry()
        {
            var config = configuration;

            Action<ResourceBuilder> configureResource = r =>
                r.AddService(
                    serviceName: config.GetValue<string>("ServiceName"),
                    serviceVersion: Environment.Version.ToString(),
                    serviceInstanceId: Environment.MachineName
                );

            var tracingExporter = config.GetValue<string>("UseTracingExporter").ToLowerInvariant();
            var histogramAggregation = config
                .GetValue<string>("HistogramAggregation")
                .ToLowerInvariant();
            var metricsExporter = config.GetValue<string>("UseMetricsExporter").ToLowerInvariant();

            services
                .AddOpenTelemetry()
                .ConfigureResource(configureResource)
                .WithTracing(builder =>
                {
                    switch (tracingExporter)
                    {
                        case "jaeger":
                            builder.AddJaegerExporter();

                            builder.ConfigureServices(services =>
                            {
                                // Use IConfiguration binding for Jaeger exporter options.
                                services.Configure<JaegerExporterOptions>(
                                    config.GetSection("Jaeger")
                                );

                                // Customize the HttpClient that will be used when JaegerExporter is configured for HTTP transport.
                                services.AddHttpClient(
                                    "JaegerExporter",
                                    configureClient: (client) =>
                                        client.DefaultRequestHeaders.Add(
                                            "X-Title",
                                            config.Title
                                                + " ,OS="
                                                + Environment.OSVersion
                                                + ",Name="
                                                + Environment.MachineName
                                                + ",Domain="
                                                + Environment.UserDomainName
                                        )
                                );
                            });
                            break;

                        case "zipkin":
                            builder.AddZipkinExporter();

                            builder.ConfigureServices(services =>
                            {
                                // Use IConfiguration binding for Zipkin exporter options.
                                services.Configure<ZipkinExporterOptions>(
                                    config.GetSection("Zipkin")
                                );
                            });
                            break;

                        case "otlp":
                            builder.AddOtlpExporter(otlpOptions =>
                            {
                                // Use IConfiguration directly for Otlp exporter endpoint option.
                                otlpOptions.Endpoint = new Uri(
                                    config.GetValue<string>("Otlp:Endpoint")
                                );
                            });
                            break;

                        default:
                            builder.AddConsoleExporter();
                            break;
                    }
                })
                .WithMetrics(builder =>
                {
                    // Metrics

                    // Ensure the MeterProvider subscribes to any custom Meters.
                    builder
                        .AddRuntimeInstrumentation()
                        .AddHttpClientInstrumentation();
                        //.AddAspNetCoreInstrumentation();

                    switch (histogramAggregation)
                    {
                        case "exponential":
                            builder.AddView(instrument =>
                            {
                                return
                                    instrument.GetType().GetGenericTypeDefinition()
                                    == typeof(Histogram<>)
                                    ? new ExplicitBucketHistogramConfiguration()
                                    : null;
                            });
                            break;
                        default:
                            // Explicit bounds histogram is the default.
                            // No additional configuration necessary.
                            break;
                    }

                    switch (metricsExporter)
                    {
                        case "prometheus":
                            builder.AddPrometheusExporter();
                            break;
                        case "otlp":
                            builder.AddOtlpExporter(otlpOptions =>
                            {
                                // Use IConfiguration directly for Otlp exporter endpoint option.
                                otlpOptions.Endpoint = new Uri(
                                    config.GetValue<string>("Otlp:Endpoint")
                                );
                            });
                            break;
                        default:
                            builder.AddConsoleExporter();
                            break;
                    }
                });

            return this;
        }

        public IServiceSetup AddHealthChecks()
        {
            services.AddHealthChecks();
            return this;
        }       

        public IServiceSetup AddImplementations(Assembly[] assemblies = null)
        {
            AddAppImplementations(assemblies);

            AddDomainImplementations();

            return this;
        }

        public IServiceSetup AddMapper<TProfile>() where TProfile : Profile
        {
            AddMapper(new DataMapper(typeof(TProfile).New<TProfile>()));

            return this;
        }

        public IServiceSetup AddMapper(params Profile[] profiles)
        {
            AddMapper(new DataMapper(profiles));

            return this;
        }

        public IServiceSetup AddMapper(IDataMapper mapper)
        {
            registry.AddObject(mapper);
            registry.AddObject<IDataMapper>(mapper);
            registry.AddScoped<IMapper, DataMapper>();

            return this;
        }

        public IServiceSetup AddRepositoryClients(Assembly[] assemblies = null)
        {
            IServiceConfiguration config = configuration;
            assemblies ??= AppDomain.CurrentDomain.GetAssemblies();
            TypeInfo[] definedTypes = assemblies.SelectMany(a => a.DefinedTypes).ToArray();
            IEnumerable<IConfigurationSection> clients = config.Clients();
            RepositoryClients repoClients = new RepositoryClients();

            services.AddSingleton(registry.AddObject<IRepositoryClients>(repoClients).Value);

            foreach (IConfigurationSection client in clients)
            {
                ClientProvider provider = config.ClientProvider(client);
                string connectionString = config.ClientConnectionString(client).Trim();
                int poolsize = config.ClientPoolSize(client);
                Type preContextType = definedTypes
                    .Where(t => t.FullName.Contains(client.Key))
                    .Select(t => t.UnderlyingSystemType)
                    .FirstOrDefault();
                if (
                    (provider == ClientProvider.None)
                    || (connectionString == null)
                    || (preContextType == null)
                )
                {
                    continue;
                }

                Type[] contextTypes = new Type[]
                {
                    preContextType.MakeGenericType(typeof(ICqrsStore)),
                    preContextType.MakeGenericType(typeof(IEventStore))
                };

                foreach (Type contextType in contextTypes)
                {
                    string routePrefix = AddDataClientPrefix(contextType).Trim();
                    if (!connectionString.EndsWith('/'))
                    {
                        connectionString += "/";
                    }

                    if (routePrefix.StartsWith('/'))
                    {
                        routePrefix = routePrefix.Substring(1);
                    }

                    routePrefix = routePrefix + "/" + provider.ToString().ToLower();

                    string _connectionString = $"{connectionString}{routePrefix}";

                    Type iRepoType = typeof(IRepositoryClient<>).MakeGenericType(contextType);
                    Type repoType = typeof(RepositoryClient<>).MakeGenericType(contextType);

                    IRepositoryClient repoClient = (IRepositoryClient)
                        repoType.New(provider, _connectionString);

                    Type storeDbType = typeof(OpenClientContext<>).MakeGenericType(
                        OpenClientRegistry.GetRemoteStore(contextType)
                    );
                    Type storeRepoType = typeof(RepositoryClient<>).MakeGenericType(storeDbType);

                    IRepositoryClient storeClient = (IRepositoryClient)
                        storeRepoType.New(repoClient);

                    Type istoreRepoType = typeof(IRepositoryClient<>).MakeGenericType(storeDbType);
                    Type ipoolRepoType = typeof(IRepositoryContextPool<>).MakeGenericType(
                        storeDbType
                    );
                    Type ifactoryRepoType = typeof(IRepositoryContextFactory<>).MakeGenericType(
                        storeDbType
                    );
                    Type idataRepoType = typeof(IRepositoryContext<>).MakeGenericType(storeDbType);

                    repoClient.PoolSize = poolsize;

                    IRepositoryClient globalClient = RepositoryManager.AddClient(repoClient);

                    registry.AddObject(iRepoType, globalClient);
                    registry.AddObject(repoType, globalClient);

                    registry.AddObject(istoreRepoType, storeClient);
                    registry.AddObject(ipoolRepoType, storeClient);
                    registry.AddObject(ifactoryRepoType, storeClient);
                    registry.AddObject(idataRepoType, storeClient);
                    registry.AddObject(storeRepoType, storeClient);

                    RepositoryManager.AddClientPool(globalClient.ContextType, poolsize);
                }
            }

            return this;
        }

        public IServiceSetup AddRepositoryEndpoints(Assembly[] assemblies = null)
        {
            IServiceConfiguration config = configuration;
            assemblies ??= Assemblies ??= AppDomain.CurrentDomain.GetAssemblies();
            TypeInfo[] definedTypes = assemblies.SelectMany(a => a.DefinedTypes).ToArray();
            IEnumerable<IConfigurationSection> endpoints = config.Endpoints();

            RepositoryEndpoints repoEndpoints = new RepositoryEndpoints();
            registry.AddSingleton(registry.AddObject<IRepositoryEndpoints>(repoEndpoints).Value);

            var providerNotExists = new HashSet<string>();

            foreach (IConfigurationSection endpoint in endpoints)
            {
                string connectionString = config.EndpointConnectionString(endpoint);
                EndpointProvider provider = config.EndpointProvider(endpoint);
                int poolsize = config.EndpointPoolSize(endpoint);
                Type contextType = definedTypes
                    .Where(t => t.FullName == endpoint.Key)
                    .Select(t => t.UnderlyingSystemType)
                    .FirstOrDefault();

                if (
                    (provider == EndpointProvider.None)
                    || (connectionString == null)
                    || (contextType == null)
                )
                {
                    continue;
                }

                if (providerNotExists.Add(provider.ToString()))
                    RepositoryEndpointOptions.AddEntityServicesForDb(provider);

                Type iRepoType = typeof(IRepositoryEndpoint<>).MakeGenericType(contextType);
                Type repoType = typeof(RepositoryEndpoint<>).MakeGenericType(contextType);
                Type repoOptionsType = typeof(DbContextOptions<>).MakeGenericType(contextType);

                IRepositoryEndpoint repoEndpoint = (IRepositoryEndpoint)
                    repoType.New(provider, connectionString);

                Type storeDbType = typeof(DataBaseContext<>).MakeGenericType(
                    DataBaseRegistry.GetDbStore(contextType)
                );
                Type storeOptionsType = typeof(DbContextOptions<>).MakeGenericType(storeDbType);
                Type storeRepoType = typeof(RepositoryEndpoint<>).MakeGenericType(storeDbType);

                IRepositoryEndpoint storeEndpoint = (IRepositoryEndpoint)
                    storeRepoType.New(repoEndpoint);

                Type istoreRepoType = typeof(IRepositoryEndpoint<>).MakeGenericType(storeDbType);
                Type ipoolRepoType = typeof(IRepositoryContextPool<>).MakeGenericType(storeDbType);
                Type ifactoryRepoType = typeof(IRepositoryContextFactory<>).MakeGenericType(
                    storeDbType
                );
                Type idataRepoType = typeof(IRepositoryContext<>).MakeGenericType(storeDbType);

                repoEndpoint.PoolSize = poolsize;

                IRepositoryEndpoint globalEndpoint = RepositoryManager.AddEndpoint(repoEndpoint);

                AddDataBaseConfiguration(globalEndpoint.Context);

                registry.AddObject(iRepoType, globalEndpoint);
                registry.AddObject(repoType, globalEndpoint);
                registry.AddObject(repoOptionsType, globalEndpoint.Options);

                registry.AddObject(istoreRepoType, storeEndpoint);
                registry.AddObject(ipoolRepoType, storeEndpoint);
                registry.AddObject(ifactoryRepoType, storeEndpoint);
                registry.AddObject(idataRepoType, storeEndpoint);
                registry.AddObject(storeRepoType, storeEndpoint);
                registry.AddObject(storeOptionsType, storeEndpoint.Options);

                RepositoryManager.AddEndpointPool(globalEndpoint.ContextType, poolsize);
            }

            return this;
        }

        public virtual IServiceSetup ConfigureServices(Assembly[] assemblies = null)
        {
            Assemblies ??= assemblies ??= AppDomain.CurrentDomain.GetAssemblies();

            AddMapper(new DataMapper());

            AddCaching();

            AddJsonSerializerDefaults();

            AddRepositoryEndpoints(Assemblies);

            AddRepositoryClients(Assemblies);

            AddImplementations(Assemblies);

            registry.MergeServices();

            return this;
        }

        public IServiceSetup MergeServices()
        {
            registry.MergeServices();

            return this;
        }

        private IServiceSetup AddStoreCache(Type tstore)
        {
            Type idatacache = typeof(IStoreCache<>).MakeGenericType(tstore);
            Type datacache = typeof(StoreCache<>).MakeGenericType(tstore);

            object cache = datacache.New(registry.GetObject<IDataCache>());
            registry.AddObject(idatacache, cache);
            registry.AddObject(datacache, cache);

            return this;
        }

        private void AddDataBaseConfiguration(IDataBaseContext context)
        {
            DataBaseContext _context = context as DataBaseContext;
            _context.ChangeTracker.AutoDetectChangesEnabled = true;
            _context.ChangeTracker.LazyLoadingEnabled = true;
            _context.Database.AutoTransactionsEnabled = false;
        }

        private string AddDataClientPrefix(Type contextType, string routePrefix = null)
        {
            Type iface = OpenClientRegistry.GetRemoteStore(contextType);
            return GetStoreRoutes(iface, routePrefix);
        }

        private string AddDataServiceEndpointPrefix(Type contextType, string routePrefix = null)
        {
            Type iface = DataBaseRegistry.GetDbStore(contextType);
            return GetStoreRoutes(iface, routePrefix);
        }

        private string GetStoreRoutes(Type iface, string routePrefix = null)
        {
            if (iface == typeof(IEntryStore))
            {
                return (routePrefix != null)
                    ? (StoreRoutes.EntryStore = routePrefix)
                    : StoreRoutes.EntryStore;
            }
            else if (iface == typeof(IEventStore))
            {
                return (routePrefix != null)
                    ? (StoreRoutes.EventStore = routePrefix)
                    : StoreRoutes.EventStore;
            }
            else if (iface == typeof(IReportStore))
            {
                return (routePrefix != null)
                    ? (StoreRoutes.ReportStore = routePrefix)
                    : StoreRoutes.ReportStore;
            }
            else if (iface == typeof(ICqrsStore))
            {
                return (routePrefix != null)
                    ? (StoreRoutes.DataStore = routePrefix)
                    : StoreRoutes.DataStore;
            }
            else
            {
                return (routePrefix != null)
                    ? (StoreRoutes.DataStore = routePrefix)
                    : StoreRoutes.DataStore;
            }
        }
    }
}
