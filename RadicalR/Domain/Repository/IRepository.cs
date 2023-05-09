using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Series;
using System.Threading.Tasks;

namespace RadicalR
{
    public interface IRepository<TEntity> : IRepository, IOrderedQueryable<TEntity>, IEnumerable<TEntity> where TEntity : class, IIdentifiable
    {
        IQueryable<TEntity> this[Expression<Func<TEntity, bool>> predicate] { get; }
        IQueryable<object> this[Expression<Func<TEntity, object>> selector] { get; }
        IQueryable<TEntity> this[params Expression<Func<TEntity, object>>[] expanders] { get; }
        TEntity this[params object[] keys] { get; set; }
        IQueryable<TEntity> this[SortExpression<TEntity> sortTerms] { get; }
        TEntity this[bool reverse, Expression<Func<TEntity, bool>> predicate] { get; }
        object this[bool reverse, Expression<Func<TEntity, object>> selector] { get; }
        TEntity this[bool reverse, params Expression<Func<TEntity, object>>[] expanders] { get; }
        TEntity this[bool reverse, SortExpression<TEntity> sortTerms] { get; }
        IQueryable<TEntity> this[Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] expanders] { get; }
        IQueryable<TEntity> this[Expression<Func<TEntity, bool>> predicate, SortExpression<TEntity> sortTerms] { get; }
        IQueryable<object> this[Expression<Func<TEntity, object>> selector, Expression<Func<TEntity, bool>> predicate] { get; }
        IQueryable<object> this[Expression<Func<TEntity, object>> selector, Expression<Func<TEntity, object>>[] expanders] { get; }
        IGrouping<dynamic, TEntity> this[Func<IQueryable<TEntity>, IGrouping<dynamic, TEntity>> groupByObject, Expression<Func<TEntity, bool>> predicate] { get; }
        IQueryable<TEntity> this[IQueryable<TEntity> query, Expression<Func<TEntity, bool>> predicate] { get; }
        IQueryable<object> this[IQueryable<TEntity> query, Expression<Func<TEntity, int, object>> selector] { get; }
        IQueryable<object> this[IQueryable<TEntity> query, Expression<Func<TEntity, object>> selector] { get; }
        IQueryable<TEntity> this[IQueryable<TEntity> query, params Expression<Func<TEntity, object>>[] expanders] { get; }
        TEntity this[object[] keys, params Expression<Func<TEntity, object>>[] expanders] { get; set; }
        IQueryable<TEntity> this[SortExpression<TEntity> sortTerms, params Expression<Func<TEntity, object>>[] expanders] { get; }
        TEntity this[bool reverse, Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] expanders] { get; }
        TEntity this[bool reverse, Expression<Func<TEntity, bool>> predicate, SortExpression<TEntity> sortTerms] { get; }
        object this[bool reverse, Expression<Func<TEntity, object>> selector, Expression<Func<TEntity, object>>[] expanders] { get; }
        TEntity this[bool reverse, SortExpression<TEntity> sortTerms, params Expression<Func<TEntity, object>>[] expanders] { get; }
        IQueryable<TEntity> this[Expression<Func<TEntity, bool>> predicate, SortExpression<TEntity> sortTerms, params Expression<Func<TEntity, object>>[] expanders] { get; }
        IQueryable<object> this[Expression<Func<TEntity, object>> selector, Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] expanders] { get; }
        object this[Expression<Func<TEntity, object>> selector, object[] keys, params Expression<Func<TEntity, object>>[] expanders] { get; set; }
        IDeck<TEntity> this[int skip, int take, Expression<Func<TEntity, bool>> predicate] { get; }
        IList<object> this[int skip, int take, Expression<Func<TEntity, object>> selector] { get; }
        IQueryable<TEntity> this[int skip, int take, IQueryable<TEntity> query] { get; }
        IDeck<TEntity> this[int skip, int take, params Expression<Func<TEntity, object>>[] expanders] { get; }
        IDeck<TEntity> this[int skip, int take, SortExpression<TEntity> sortTerms] { get; }
        TEntity this[bool reverse, Expression<Func<TEntity, bool>> predicate, SortExpression<TEntity> sortTerms, params Expression<Func<TEntity, object>>[] expanders] { get; }
        object this[bool reverse, Expression<Func<TEntity, object>> selector, Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] expanders] { get; }
        IQueryable<object> this[Expression<Func<TEntity, object>> selector, Expression<Func<TEntity, bool>> predicate, SortExpression<TEntity> sortTerms, params Expression<Func<TEntity, object>>[] expanders] { get; }
        IDeck<TEntity> this[int skip, int take, Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] expanders] { get; }
        IDeck<TEntity> this[int skip, int take, Expression<Func<TEntity, bool>> predicate, SortExpression<TEntity> sortTerms] { get; }
        IList<object> this[int skip, int take, Expression<Func<TEntity, object>> selector, Expression<Func<TEntity, object>>[] expanders] { get; }
        IDeck<TEntity> this[int skip, int take, SortExpression<TEntity> sortTerms, params Expression<Func<TEntity, object>>[] expanders] { get; }
        object this[bool reverse, Expression<Func<TEntity, object>> selector, Expression<Func<TEntity, bool>> predicate, SortExpression<TEntity> sortTerms, params Expression<Func<TEntity, object>>[] expanders] { get; }
        IDeck<TEntity> this[int skip, int take, Expression<Func<TEntity, bool>> predicate, SortExpression<TEntity> sortTerms, params Expression<Func<TEntity, object>>[] expanders] { get; }
        IList<object> this[int skip, int take, Expression<Func<TEntity, object>> selector, Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] expanders] { get; }
        IList<object> this[int skip, int take, Expression<Func<TEntity, object>> selector, Expression<Func<TEntity, bool>> predicate, SortExpression<TEntity> sortTerms, params Expression<Func<TEntity, object>>[] expanders] { get; }

        IQueryable<TEntity> Query { get; }

        IQueryable<TEntity> AsQueryable();

        IEnumerable<TEntity> Add(IEnumerable<TEntity> entity);
        IEnumerable<TEntity> Add(IEnumerable<TEntity> entities, Func<TEntity, Expression<Func<TEntity, bool>>> predicate);
        IAsyncEnumerable<TEntity> AddAsync(IEnumerable<TEntity> entity);
        IAsyncEnumerable<TEntity> AddAsync(IEnumerable<TEntity> entities, Func<TEntity, Expression<Func<TEntity, bool>>> predicate);
        TEntity Add(TEntity entity);
        TEntity Add(TEntity entity, Func<TEntity, Expression<Func<TEntity, bool>>> predicate);

        TEntity Delete(Expression<Func<TEntity, bool>> predicate);
        IEnumerable<TEntity> Delete(IEnumerable<TEntity> entity);
        IEnumerable<TEntity> Delete(IEnumerable<TEntity> entity, Func<TEntity, Expression<Func<TEntity, bool>>> predicate);
        IEnumerable<TEntity> Delete(long[] ids);
        Task<TEntity> Delete(params object[] key);
        TEntity Delete(TEntity entity);
        TEntity Delete(TEntity entity, Func<TEntity, Expression<Func<TEntity, bool>>> predicate);

        IAsyncEnumerable<TEntity> DeleteAsync(IEnumerable<TEntity> entity);
        IAsyncEnumerable<TEntity> DeleteAsync(IEnumerable<TEntity> entity, Func<TEntity, Expression<Func<TEntity, bool>>> predicate);

        Task<bool> Exist(Expression<Func<TEntity, bool>> predicate);
        Task<bool> Exist(Type exceptionType, Expression<Func<TEntity, bool>> predicate, string message);
        Task<bool> Exist(Type exceptionType, object instance, string message);
        Task<bool> Exist<TException>(Expression<Func<TEntity, bool>> predicate, string message) where TException : Exception;
        Task<bool> Exist<TException>(object instance, string message) where TException : Exception;

        Task<IDeck<TEntity>> Filter(int skip, int take, Expression<Func<TEntity, bool>> predicate);
        Task<IDeck<TEntity>> Filter(int skip, int take, Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] expanders);
        Task<IDeck<TEntity>> Filter(int skip, int take, Expression<Func<TEntity, bool>> predicate, SortExpression<TEntity> sortTerms);
        Task<IDeck<TEntity>> Filter(int skip, int take, Expression<Func<TEntity, bool>> predicate, SortExpression<TEntity> sortTerms, params Expression<Func<TEntity, object>>[] expanders);
        Task<IDeck<TEntity>> Filter(int skip, int take, SortExpression<TEntity> sortTerms);
        Task<IDeck<TEntity>> Filter(int skip, int take, SortExpression<TEntity> sortTerms, params Expression<Func<TEntity, object>>[] expanders);
        Task<IDeck<TEntity>> Filter(IQueryable<TEntity> query);
        Task<IList<TResult>> Filter<TResult>(int skip, int take, Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> selector);
        Task<IList<TResult>> Filter<TResult>(int skip, int take, Expression<Func<TEntity, bool>> predicate, SortExpression<TEntity> sortTerms, Expression<Func<TEntity, TResult>> selector, params Expression<Func<TEntity, object>>[] expanders);
        Task<IList<TResult>> Filter<TResult>(int skip, int take, Expression<Func<TEntity, TResult>> selector);
        Task<IList<TResult>> Filter<TResult>(int skip, int take, Expression<Func<TEntity, TResult>> selector, params Expression<Func<TEntity, object>>[] expanders);

        Task<TEntity> Find(Expression<Func<TEntity, bool>> predicate, bool reverse);
        Task<TEntity> Find(Expression<Func<TEntity, bool>> predicate, bool reverse, params Expression<Func<TEntity, object>>[] expanders);
        Task<TEntity> Find(object[] keys, params Expression<Func<TEntity, object>>[] expanders);
        Task<TEntity> Find(params object[] keys);
        Task<TResult> Find<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> selector);
        Task<TResult> Find<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> selector, params Expression<Func<TEntity, object>>[] expanders);
        Task<TResult> Find<TResult>(object[] keys, Expression<Func<TEntity, TResult>> selector, params Expression<Func<TEntity, object>>[] expanders) where TResult : class;

        Task<IDeck<TEntity>> Get(int skip, int take, params Expression<Func<TEntity, object>>[] expanders);
        Task<IDeck<TEntity>> Get(int skip, int take, SortExpression<TEntity> sortTerms, params Expression<Func<TEntity, object>>[] expanders);
        Task<IDeck<TEntity>> Get(params Expression<Func<TEntity, object>>[] expanders);
        Task<IDeck<TEntity>> Get(SortExpression<TEntity> sortTerms, params Expression<Func<TEntity, object>>[] expanders);
        Task<IList<TResult>> Get<TResult>(Expression<Func<TEntity, TResult>> selector);
        Task<IList<TResult>> Get<TResult>(Expression<Func<TEntity, TResult>> selector, params Expression<Func<TEntity, object>>[] expanders);

        new IEnumerator<TEntity> GetEnumerator();

        TEntity NewEntry(params object[] parameters);

        Task<bool> NotExist(Expression<Func<TEntity, bool>> predicate);
        Task<bool> NotExist(Type exceptionType, Expression<Func<TEntity, bool>> predicate, string message);
        Task<bool> NotExist(Type exceptionType, object instance, string message);
        Task<bool> NotExist<TException>(Expression<Func<TEntity, bool>> predicate, string message) where TException : Exception;
        Task<bool> NotExist<TException>(object instance, string message) where TException : Exception;

        IEnumerable<TEntity> Patch<TModel>(IEnumerable<TModel> entity, params Expression<Func<TEntity, object>>[] expanders) where TModel : class, IIdentifiable;
        IEnumerable<TEntity> Patch<TModel>(IEnumerable<TModel> entities, Func<TModel, Expression<Func<TEntity, bool>>> predicate, params Expression<Func<TEntity, object>>[] expanders) where TModel : class, IIdentifiable;
        Task<TEntity> Patch<TModel>(TModel delta) where TModel : class, IIdentifiable;
        Task<TEntity> Patch<TModel>(TModel delta, Func<TModel, Expression<Func<TEntity, bool>>> predicate) where TModel : class, IIdentifiable;
        Task<TEntity> Patch<TModel>(TModel delta, params object[] key) where TModel : class, IIdentifiable;

        IAsyncEnumerable<TEntity> PatchAsync<TModel>(IEnumerable<TModel> entity, params Expression<Func<TEntity, object>>[] expanders) where TModel : class, IIdentifiable;
        IAsyncEnumerable<TEntity> PatchAsync<TModel>(IEnumerable<TModel> entities, Func<TModel, Expression<Func<TEntity, bool>>> predicate, params Expression<Func<TEntity, object>>[] expanders) where TModel : class, IIdentifiable;

        IEnumerable<TEntity> Put(IEnumerable<TEntity> entities, Func<TEntity, Expression<Func<TEntity, bool>>> predicate, params Func<TEntity, Expression<Func<TEntity, bool>>>[] conditions);
        Task<TEntity> Put(TEntity entity, Func<TEntity, Expression<Func<TEntity, bool>>> predicate, params Func<TEntity, Expression<Func<TEntity, bool>>>[] conditions);

        IAsyncEnumerable<TEntity> PutAsync(IEnumerable<TEntity> entities, Func<TEntity, Expression<Func<TEntity, bool>>> predicate, params Func<TEntity, Expression<Func<TEntity, bool>>>[] conditions);

        IEnumerable<TEntity> Set<TModel>(IEnumerable<TModel> entity) where TModel : class, IIdentifiable;
        IEnumerable<TEntity> Set<TModel>(IEnumerable<TModel> entities, Func<TModel, Expression<Func<TEntity, bool>>> predicate, params Func<TModel, Expression<Func<TEntity, bool>>>[] conditions) where TModel : class, IIdentifiable;
        TEntity Update(TEntity entity);
        Task<TEntity> Set<TModel>(TModel entity) where TModel : class, IIdentifiable;
        Task<TEntity> Set<TModel>(TModel entity, Func<TModel, Expression<Func<TEntity, bool>>> predicate, params Func<TModel, Expression<Func<TEntity, bool>>>[] conditions) where TModel : class, IIdentifiable;
        Task<TEntity> Set<TModel>(TModel entity, object key, Func<TEntity, Expression<Func<TEntity, bool>>> condition) where TModel : class, IIdentifiable;
        Task<TEntity> Set<TModel>(TModel entity, object[] key) where TModel : class;

        IAsyncEnumerable<TEntity> SetAsync<TModel>(IEnumerable<TModel> entity) where TModel : class, IIdentifiable;
        IAsyncEnumerable<TEntity> SetAsync<TModel>(IEnumerable<TModel> entities, Func<TModel, Expression<Func<TEntity, bool>>> predicate, params Func<TModel, Expression<Func<TEntity, bool>>>[] conditions) where TModel : class, IIdentifiable;

        TEntity Sign(TEntity entity);

        IQueryable<TEntity> Sort(IQueryable<TEntity> query, SortExpression<TEntity> sortTerms);

        TEntity Stamp(TEntity entity);

    }
}