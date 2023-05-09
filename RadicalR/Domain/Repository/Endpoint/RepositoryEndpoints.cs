using Microsoft.EntityFrameworkCore;
using System;
using System.Series;
using System.Uniques;

namespace RadicalR
{
    public class RepositoryEndpoints : Catalog<IRepositoryEndpoint>, IRepositoryEndpoints
    {        
        public RepositoryEndpoints() : base(false, 17) { }        

        public IRepositoryEndpoint this[string contextName]
        {
            get => base[contextName.UniqueKey64()];
            set => base.Set(contextName.UniqueKey64(), value);
        }
        public IRepositoryEndpoint this[DbContext context]
        {
            get => base[context.GetType()];
            set => base.Set(context.GetType(), value);
        }
        public IRepositoryEndpoint this[Type contextType]
        {
            get => base[contextType];
            set => base.Set(contextType, value);
        }

        public IRepositoryEndpoint Get(Type contextType)
        {
            return base[contextType];
        }
        public IRepositoryEndpoint<TContext> Get<TContext>() where TContext : DbContext
        {
            return (IRepositoryEndpoint<TContext>)base[typeof(TContext)];
        }
   
        public bool TryGet(Type contextType, out IRepositoryEndpoint repoSource)
        {
            return base.TryGet(contextType, out repoSource);
        }
        public bool TryGet<TContext>(out IRepositoryEndpoint<TContext> repoSource) where TContext : DbContext
        {            
            if (TryGet(typeof(TContext), out IRepositoryEndpoint _repo))
            {
                repoSource = (IRepositoryEndpoint<TContext>)_repo;
                return false;
            }
            repoSource = null;
            return false;
        }

        public bool TryAdd(Type contextType, IRepositoryEndpoint repoSource)
        {
            return base.Add(contextType, repoSource);
        }
        public override bool TryAdd(IRepositoryEndpoint repoSource)
        {
            return base.Add(repoSource.ContextType, repoSource);
        }
        public bool TryAdd<TContext>(IRepositoryEndpoint<TContext> repoSource) where TContext : DbContext
        {
            return base.Add(typeof(TContext), repoSource);
        }       

        public IRepositoryEndpoint<TContext> Add<TContext>(IRepositoryEndpoint<TContext> repoSource) where TContext : DbContext
        {
            return (IRepositoryEndpoint<TContext>)base.Put(typeof(TContext), repoSource).Value;
        }
        public override void Add(IRepositoryEndpoint repoSource)
        {
             base.Put(repoSource.ContextType, repoSource);
        }

        public bool Remove<TContext>() where TContext : DbContext
        {
            return TryRemove(typeof(TContext));
        }

        public int PoolCount(Type contextType)
        {
            return Get(contextType).Count;
        }
        public int PoolCount<TContext>() where TContext : DbContext
        {
            return Get<TContext>().Count;
        }

        public IRepositoryEndpoint<TContext> Put<TContext>(IRepositoryEndpoint<TContext> repoSource)
            where TContext : DbContext
        {
            return (IRepositoryEndpoint<TContext>)base.Put(typeof(TContext), repoSource).Value;            
        }    

        public IRepositoryEndpoint<TContext> New<TContext>(DbContextOptions<TContext> options) where TContext : DataBaseContext
        {
            return Add(new RepositoryEndpoint<TContext>(options));
        }
        public IRepositoryEndpoint New(Type contextType, DbContextOptions options)
        {
            return Put(new RepositoryEndpoint(contextType, options)).Value;
        }

        public ulong GetKey(IRepositoryEndpoint item)
        {
            return item.UniqueKey;
        }    

        protected override void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {           
                    base.Dispose(true);
                }
                disposedValue = true;
            }
        }
    }
      
}
