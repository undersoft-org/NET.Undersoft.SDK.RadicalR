namespace RadicalR
{
    public interface IRepositoryClient
        : IRepositoryContextPool,
            IUnique,
            IDisposable,
            IAsyncDisposable
    {
        DataClientContext Context { get; }

        Uri Route { get; }

        TContext GetContext<TContext>() where TContext : DataClientContext;

        object CreateContext(Type contextType, Uri serviceRoot);
        TContext CreateContext<TContext>(Uri serviceRoot) where TContext : DataClientContext;

        void BuildMetadata();
    }

    public interface IRepositoryClient<TContext>
        : IRepositoryContextPool<TContext>,
            IRepositoryClient where TContext : class
    {
        new TContext Context { get; }

        TContext CreateContext(Uri serviceRoot);
    }
}
