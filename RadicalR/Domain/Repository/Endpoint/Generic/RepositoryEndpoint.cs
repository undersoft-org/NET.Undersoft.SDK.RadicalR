using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace RadicalR
{
    public class RepositoryEndpoint<TContext> : RepositoryEndpoint, IRepositoryEndpoint<TContext>
        where TContext : DataBaseContext
    {
        protected new DbContextOptionsBuilder<TContext> optionsBuilder;

        public RepositoryEndpoint() : base()
        {
        }

        public RepositoryEndpoint(IServiceConfiguration config) : base()
        {
            contextType = typeof(TContext);
            IConfigurationSection endpoint = config.Endpoint(contextType.FullName);
            string connectionString = config.EndpointConnectionString(contextType.FullName);
            EndpointProvider provider = config.EndpointProvider(contextType.FullName);
            ContextPool = this;
            PoolSize = config.EndpointPoolSize(endpoint);
            optionsBuilder = RepositoryEndpointOptions.BuildOptions<TContext>(provider, connectionString);
            InnerContext = CreateContext(optionsBuilder.Options);
            Context.GetDbEntities();
        }

        public RepositoryEndpoint(DbContextOptions<TContext> options) : base()
        {
            ContextPool = this;
            contextType = options.ContextType;
            InnerContext = CreateContext(options);
            Context.GetDbEntities();
        }

        public RepositoryEndpoint(IRepositoryEndpoint pool) : base(pool)
        {
        }

        public RepositoryEndpoint(EndpointProvider provider, string connectionString) : base()
        {
            ContextPool = this;
            contextType = typeof(TContext);
            optionsBuilder = RepositoryEndpointOptions.BuildOptions<TContext>(provider, connectionString);
            InnerContext = CreateContext(optionsBuilder.Options);
            Context.GetDbEntities();
        }

        public override TContext Context => (TContext)InnerContext;

        public override DbContextOptions<TContext> Options => (DbContextOptions<TContext>)base.Options;

        public override TContext CreateContext() { return typeof(TContext).New<TContext>(Options); }

        public TContext CreateContext(DbContextOptions<TContext> options)
        {
            Options ??= options;
            Type type = typeof(TContext);
            contextType ??= type;
            return type.New<TContext>(options);
        }

        public TContext CreateDbContext() { return CreateContext(); }
        public TContext CreateDbContext(string[] args) { return CreateContext(); }
    }
}
