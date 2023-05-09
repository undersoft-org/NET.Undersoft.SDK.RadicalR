using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace RadicalR
{
    public interface IPage<TEntity> : IPagedSet<TEntity>
    {
        IPage<TEntity> AsPage(int pageIndex, int pageSize, int indexFrom = 0);      

        Task<IPagedSet<TEntity>> Get(params Expression<Func<TEntity, object>>[] expanders);
        Task<IPagedSet<TModel>>  Get<TModel>(params Expression<Func<TEntity, object>>[] expanders);

        Task<IPagedSet<TEntity>> Filter(SortExpression<TEntity> sortTerms);
        Task<IPagedSet<TEntity>> Filter(Expression<Func<TEntity, bool>> predicate);
        Task<IPagedSet<TEntity>> Filter(Expression<Func<TEntity, bool>> predicate, SortExpression<TEntity> sortTerms);
        Task<IPagedSet<TEntity>> Filter(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] expanders);
        Task<IPagedSet<TEntity>> Filter(Expression<Func<TEntity, bool>> predicate, SortExpression<TEntity> sortTerms, params Expression<Func<TEntity, object>>[] expanders);
        Task<IPagedSet<TEntity>> Filter(SortExpression<TEntity> sortTerms, params Expression<Func<TEntity, object>>[] expanders);

        Task<IPagedSet<TModel>> Filter<TModel>(SortExpression<TEntity> sortTerms);
        Task<IPagedSet<TModel>> Filter<TModel>(Expression<Func<TEntity, bool>> predicate);
        Task<IPagedSet<TModel>> Filter<TModel>(Expression<Func<TEntity, bool>> predicate, SortExpression<TEntity> sortTerms);
        Task<IPagedSet<TModel>> Filter<TModel>(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] expanders);
        Task<IPagedSet<TModel>> Filter<TModel>(Expression<Func<TEntity, bool>> predicate, SortExpression<TEntity> sortTerms, params Expression<Func<TEntity, object>>[] expanders);
        Task<IPagedSet<TModel>> Filter<TModel>(SortExpression<TEntity> sortTerms, params Expression<Func<TEntity, object>>[] expanders);

    }

}
