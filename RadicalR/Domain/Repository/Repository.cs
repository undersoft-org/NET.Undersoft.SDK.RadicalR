using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Collections;
using System.Linq.Expressions;

namespace RadicalR
{
    public abstract partial class Repository<TEntity> : Repository, IRepository<TEntity>
        where TEntity : class, IIdentifiable
    {
        protected IDataCache cache;
        protected IQueryable<TEntity> query;

        public Repository()
        {
            ElementType = typeof(TEntity).ProxyRetype();
            Expression = Expression.Constant(this);
        }

        public Repository(IRepositoryClient repositorySource) : base(repositorySource)
        {
            ElementType = typeof(TEntity).ProxyRetype();
            Expression = Expression.Constant(this.AsEnumerable());
        }

        public Repository(IRepositoryEndpoint repositorySource) : base(repositorySource)
        {
            ElementType = typeof(TEntity).ProxyRetype();
            Expression = Expression.Constant(this.AsEnumerable());
        }

        public Repository(object context) : base(context)
        {
            ElementType = typeof(TEntity).ProxyRetype();
            Expression = Expression.Constant(this.AsEnumerable());
        }

        public Repository(IRepositoryContext context) : base(context)
        {
            ElementType = typeof(TEntity).ProxyRetype();
            Expression = Expression.Constant(this.AsEnumerable());
        }

        public Repository(IQueryProvider provider, Expression expression)
        {
            ElementType = typeof(TEntity).ProxyRetype();
            Provider = provider;
            Expression = expression;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public abstract IQueryable<TEntity> AsQueryable();

        public IEnumerator<TEntity> GetEnumerator()
        {
            return Provider.Execute<IQueryable<TEntity>>(Expression).GetEnumerator();
        }

        public override void LinkTrigger(object sender, EntityEntryEventArgs e)
        {
            EntityEntry entry = e.Entry;
            object entity = entry.Entity;
            Type type = entity.ProxyRetype();

            if (type == ElementType)
            {
                LinkedObjects.DoEach(async (o) => await o.LoadAsync(entity));
            }
        }

        public TEntity Sign(TEntity entity)
        {
            entity.Sign();
            cache?.MemorizeAsync(entity);
            return entity;
        }

        public TEntity Stamp(TEntity entity)
        {
            entity.Stamp();
            cache?.MemorizeAsync(entity);
            return entity;
        }

        public abstract IQueryable<TEntity> Query { get; }
    }

    public enum RelatedType
    {
        None = 0,
        Reference = 1,
        Collection = 2,
        Any = 3
    }
}
