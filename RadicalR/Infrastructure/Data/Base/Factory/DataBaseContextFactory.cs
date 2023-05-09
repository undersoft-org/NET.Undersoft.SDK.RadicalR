using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace RadicalR
{
    public class DataBaseContextFactory<TContext> : IDesignTimeDbContextFactory<TContext>, IDbContextFactory<TContext> where TContext : DbContext
    {
        public TContext CreateDbContext(string[] args)
        {
            var config = new ServiceConfiguration();
            var configEndpoint = config.Endpoint(typeof(TContext).FullName);
            var provider = config.EndpointProvider(configEndpoint);
            RepositoryEndpointOptions.AddEntityServicesForDb(provider);
            var options = RepositoryEndpointOptions.BuildOptions<TContext>(provider,
                                                                           config.EndpointConnectionString(configEndpoint)).Options;
            return typeof(TContext).New<TContext>(options);
        }

        public TContext CreateDbContext()
        {
            if (RepositoryManager.TryGetEndpoint<TContext>(out var endpoint))
                return endpoint.CreateContext<TContext>();
            return null;
        }
    }
}