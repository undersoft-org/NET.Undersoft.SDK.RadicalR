using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

namespace RadicalR
{
    public static class RepositoryEndpointOptions
    {
        public static IServiceRegistry AddEntityServicesForDb(EndpointProvider provider)
        {
            IServiceRegistry registry = ServiceManager.GetRegistry();
            if (!DataBaseRegistry.Providers.ContainsKey((int)provider))
            {
                switch (provider)
                {
                    case EndpointProvider.SqlServer:
                        registry.AddEntityFrameworkSqlServer();
                        break;
                    case EndpointProvider.AzureSql:
                        registry.AddEntityFrameworkSqlServer();
                        break;
                    case EndpointProvider.PostgreSql:
                        registry.AddEntityFrameworkNpgsql();
                        break;
                    case EndpointProvider.SqlLite:
                        registry.AddEntityFrameworkSqlite();
                        break;
                    case EndpointProvider.MariaDb:
                        registry.AddEntityFrameworkMySql();
                        break;
                    case EndpointProvider.MySql:
                        registry.AddEntityFrameworkMySql();
                        break;
                    case EndpointProvider.Oracle:
                        registry.AddEntityFrameworkOracle();
                        break;
                    case EndpointProvider.CosmosDb:
                        registry.AddEntityFrameworkCosmos();
                        break;
                    case EndpointProvider.MemoryDb:
                        registry.AddEntityFrameworkInMemoryDatabase();
                        break;
                    default:
                        break;
                }
                registry.AddEntityFrameworkProxies();
                DataBaseRegistry.Providers.Add((int)provider, provider);
            }
            return registry;
        }

        public static DbContextOptionsBuilder<TContext> BuildOptions<TContext>(
            EndpointProvider provider,
            string connectionString)
            where TContext : DbContext
        {
            return (DbContextOptionsBuilder<TContext>)BuildOptions(
                new DbContextOptionsBuilder<TContext>(),
                provider,
                connectionString)
                .ConfigureWarnings(w => w.Ignore(CoreEventId.DetachedLazyLoadingWarning));
        }

        public static DbContextOptionsBuilder BuildOptions(EndpointProvider provider, string connectionString)
        {
            return BuildOptions(new DbContextOptionsBuilder(), provider, connectionString)
                .ConfigureWarnings(w => w.Ignore(CoreEventId.DetachedLazyLoadingWarning));
        }

        public static DbContextOptionsBuilder BuildOptions(
            DbContextOptionsBuilder builder,
            EndpointProvider provider,
            string connectionString)
        {
            switch (provider)
            {
                case EndpointProvider.SqlServer:
                    return builder
                        .UseSqlServer(connectionString)
                        .UseLazyLoadingProxies();
                case EndpointProvider.AzureSql:
                    return builder
                        .UseSqlServer(connectionString)
                        .UseLazyLoadingProxies();

                case EndpointProvider.PostgreSql:
                    return builder.UseInternalServiceProvider(
                        ServiceManager.GetManager())
                        .UseNpgsql(connectionString)
                        .UseLazyLoadingProxies();

                case EndpointProvider.SqlLite:
                    return builder
                        .UseSqlite(connectionString)
                        .UseLazyLoadingProxies();

                case EndpointProvider.MariaDb:
                    return builder
                        .UseMySql(
                            ServerVersion
                            .AutoDetect(connectionString))
                        .UseLazyLoadingProxies();

                case EndpointProvider.MySql:
                    return builder
                        .UseMySql(
                            ServerVersion
                            .AutoDetect(connectionString))
                        .UseLazyLoadingProxies();

                case EndpointProvider.Oracle:
                    return builder
                        .UseOracle(connectionString)
                        .UseLazyLoadingProxies();

                case EndpointProvider.CosmosDb:
                    return builder
                        .UseCosmos(
                            connectionString.Split('#')[0],
                            connectionString.Split('#')[1],
                            connectionString.Split('#')[2])
                        .UseLazyLoadingProxies();

                case EndpointProvider.MemoryDb:
                    return builder.UseInternalServiceProvider(new ServiceManager())
                        .UseInMemoryDatabase(connectionString)
                        .UseLazyLoadingProxies()
                        .ConfigureWarnings(
                            w => w.Ignore(
                                InMemoryEventId
                                .TransactionIgnoredWarning));
                default:
                    break;
            }

            return builder;
        }
    }

    public enum EndpointProvider
    {
        None,
        SqlServer,
        MemoryDb,
        AzureSql,
        PostgreSql,
        SqlLite,
        MySql,
        MariaDb,
        Oracle,
        CosmosDb,
        MongoDb,
        FileSystem
    }

    public enum ClientProvider
    {
        None,
        OData,
        RabbitMQ,
        gRPC
    }
}
