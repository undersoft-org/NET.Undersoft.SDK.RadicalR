using Microsoft.OData.Client;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RadicalR
{
    public interface IRemoteRepository<TEntity> : IRepository<TEntity> where TEntity : class, IIdentifiable
    {
        DataClientContext Context { get; }

        new DataServiceQuery<TEntity> Query { get; }

        DataServiceQuerySingle<TEntity> QuerySingle(params object[] keys);

        Task<IEnumerable<TEntity>> FindMany(params object[] keys);

        new Task<TEntity> Find(params object[] keys);
    }
} 