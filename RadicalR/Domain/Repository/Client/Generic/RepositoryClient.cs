using System;
using System.Linq;
using System.Logs;
using System.Security.Policy;
using System.Series;
using System.Threading;
using System.Threading.Tasks;
using System.Uniques;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.OData.Client;
using RTools_NTS.Util;

namespace RadicalR
{
 
    public class RepositoryClient<TContext> : RepositoryClient, IRepositoryClient<TContext> where TContext : DataClientContext
    {
        public RepositoryClient() { }
        public RepositoryClient(IServiceConfiguration config) : this()
        {
            var contextType = typeof(TContext);
            var endpoint = config.Client(contextType.FullName);
            var connectionString = config.ClientConnectionString(contextType.FullName);
            var provider = config.ClientProvider(contextType.FullName);
            PoolSize = config.ClientPoolSize(endpoint);
            InnerContext = CreateContext(contextType, new Uri(connectionString));
        }
        public RepositoryClient(ClientProvider provider, string connectionString)
        {
            ContextPool = this;
            contextType = typeof(TContext);
            InnerContext = CreateContext<TContext>(new Uri(connectionString));            
        }
        public RepositoryClient(Uri serviceRoot)
        {
            ContextPool = this;
            contextType = typeof(TContext);
            InnerContext = CreateContext<TContext>(uri);
        }
        public RepositoryClient(IRepositoryClient client) : base(client)
        {  
        }

        public override TContext Context => (TContext)InnerContext;          

        public override TContext CreateContext()
        {  
            return typeof(TContext).New<TContext>(uri);            
        }

        public TContext CreateContext(Uri serviceRoot)
        {
            uri = serviceRoot;            
            return typeof(TContext).New<TContext>(uri);            
        }        
    }

}
