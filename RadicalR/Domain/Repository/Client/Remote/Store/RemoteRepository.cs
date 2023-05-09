using System.Threading;
using System.Threading.Tasks;

namespace RadicalR
{
    public class RemoteRepository<TStore, TEntity> : RemoteRepository<TEntity>, IRemoteRepository<TStore, TEntity>
        where TEntity : class, IIdentifiable
        where TStore : IDataStore
    {
        public RemoteRepository(IRepositoryContextPool<OpenClientContext<TStore>> pool, IEntityCache<TStore, TEntity> cache) : base(
            pool.ContextPool)
        {
            mapper = cache.Mapper;
            this.cache = cache;
        }

        public override Task<int> Save(bool asTransaction, CancellationToken token = default)
        { return ContextLease.Save(asTransaction, token); }
    }
}
