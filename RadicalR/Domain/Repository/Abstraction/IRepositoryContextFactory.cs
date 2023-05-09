using Microsoft.EntityFrameworkCore;
using System;

namespace RadicalR
{
    public interface IRepositoryContextFactory : IRepositoryContext, IAsyncDisposable, IDisposable
    {
        object CreateContext();

        TContext CreateContext<TContext>() where TContext : class;
    }

    public interface IRepositoryContextFactory<TContext> : IRepositoryContextFactory, IRepositoryContext<TContext>, IAsyncDisposable,
        IDisposable where TContext : class
    {
        new TContext CreateContext();
    }

}