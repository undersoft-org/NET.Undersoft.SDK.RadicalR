using System.Series;
using System.Threading;
using System.Threading.Tasks;

namespace RadicalR
{
    public interface IRepositoryContextPool : IDeck<IRepositoryContext>, IRepositoryContextFactory
    {
        int PoolSize { get; set; }

        void CreatePool();

        IRepositoryContext Rent();

        void Return();

        Task ReturnAsync(CancellationToken cancellationToken = default);
    }

    public interface IRepositoryContextPool<TContext> : IRepositoryContextPool, IRepositoryContextFactory<TContext>
        where TContext : class { }

}