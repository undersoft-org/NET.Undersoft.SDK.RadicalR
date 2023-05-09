using Microsoft.OData.Client;

namespace RadicalR
{

    public class RemoteOnSets<TStore, TEntity> : RemoteSet<TEntity>, IRemoteOnSets<TStore, TEntity> where TEntity : class, IIdentifiable
    {
        public RemoteOnSets(IRemoteRepository<TStore, TEntity> repository) : base(repository) { }
    }

    public class RemoteOnSets<TEntity> : RemoteSet<TEntity> where TEntity : class, IIdentifiable
    {
        public RemoteOnSets() : base()
        {
        }
        public RemoteOnSets(DataServiceQuery<TEntity> query) : base(query)
        {
        }
        public RemoteOnSets(DataClientContext context, IQueryable<TEntity> query) : base(context, query)
        {
        }
        public RemoteOnSets(DataClientContext context) : base(context)
        {
        }
    }
}