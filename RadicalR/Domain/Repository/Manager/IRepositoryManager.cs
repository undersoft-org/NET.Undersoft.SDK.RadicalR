using System;
using System.Collections.Generic;
using System.Series;
using System.Threading.Tasks;

namespace RadicalR
{
    public interface IRepositoryManager
    {
        IDataMapper Mapper { get; }

        Task AddClientPools();
        Task AddEndpointPools();
        
        IRemoteRepository<TEntity> load<TEntity>() where TEntity : Entity;
        IRemoteRepository<TEntity> Load<TEntity>() where TEntity : Entity;
        IRemoteRepository<TEntity> Load<TEntity>(Type contextType) where TEntity : Entity;
        IRemoteRepository<TEntity> load<TStore, TEntity>() where TStore : IDataStore where TEntity : Entity;
        IRemoteRepository<TEntity> Load<TStore, TEntity>() where TStore : IDataStore where TEntity : Entity;
        
        IRepositoryClient GetClient<TStore, TEntity>() where TEntity : Entity;
        IEnumerable<IRepositoryClient> GetClients();
        IRepositoryEndpoint GetEndpoint<TStore, TEntity>() where TEntity : Entity;
        IEnumerable<IRepositoryEndpoint> GetEndpoints();
        IEntityRepository<TEntity> use<TEntity>() where TEntity : Entity;
        IEntityRepository<TEntity> Use<TEntity>() where TEntity : Entity;
        IEntityRepository<TEntity> Use<TEntity>(Type contextType) where TEntity : Entity;
        IEntityRepository<TEntity> use<TStore, TEntity>()
            where TStore : IDataStore
            where TEntity : Entity;
        IEntityRepository<TEntity> Use<TStore, TEntity>()
            where TStore : IDataStore
            where TEntity : Entity;
    }
}