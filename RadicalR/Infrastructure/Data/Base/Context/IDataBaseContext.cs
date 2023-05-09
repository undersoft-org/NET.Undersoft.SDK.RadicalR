using Microsoft.EntityFrameworkCore.Infrastructure;

namespace RadicalR
{
    public interface IDataBaseContext<TStore> : IDataBaseContext where TStore : IDataStore { }

    public interface IDataBaseContext : IResettableService, IDisposable, IAsyncDisposable
    {
        object EntitySet<TEntity>() where TEntity : class, IIdentifiable;

        object EntitySet(Type entityType);

        Task<int> Save(bool asTransaction, CancellationToken token = default(CancellationToken));
    }
}
