using System.Linq.Expressions;

namespace RadicalR
{
    public class GetQuery<TStore, TEntity, TDto> : Query<TStore, TEntity, IQueryable<TDto>>
        where TEntity : Entity where TStore : IDataStore
    {
        public GetQuery(params Expression<Func<TEntity, object>>[] expanders) : base(expanders)
        {
        }

        public GetQuery(SortExpression<TEntity> sortTerms, params Expression<Func<TEntity, object>>[] expanders) : base(sortTerms, expanders)
        {
        }
    }
}
