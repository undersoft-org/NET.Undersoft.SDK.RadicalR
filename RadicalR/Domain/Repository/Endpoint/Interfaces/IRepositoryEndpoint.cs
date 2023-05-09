using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace RadicalR
{
    public interface IRepositoryEndpoint<TStore, TEntity> : IRepositoryEndpoint where TEntity : class, IIdentifiable
    {
        IQueryable<TEntity> FromSql(string sql, params object[] parameters);

        DbSet<TEntity> EntitySet();
    }

    public interface IRepositoryEndpoint : IRepositoryContextPool
    {
        IDataBaseContext CreateContext(DbContextOptions options);

        IDataBaseContext CreateContext(Type contextType, DbContextOptions options);

        object EntitySet<TEntity>() where TEntity : class, IIdentifiable;

        object EntitySet(Type entityType);

        IDataBaseContext Context { get; }

        DbContextOptions Options { get; }
    }

    public interface IRepositoryEndpoint<TContext> : IRepositoryContextPool<TContext>, IDesignTimeDbContextFactory<TContext>, IDbContextFactory<TContext>, IRepositoryEndpoint
        where TContext : DbContext
    {
        TContext CreateContext(DbContextOptions<TContext> options);

        new DbContextOptions<TContext> Options { get; }
    }
}
