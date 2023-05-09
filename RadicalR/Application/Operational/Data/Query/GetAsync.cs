using MediatR;
using System.Linq.Expressions;

namespace RadicalR
{
    public class GetAsync<TStore, TEntity, TDto> : GetItems<TStore, TEntity, TDto>, IStreamRequest<TDto>
        where TEntity : Entity where TStore : IDataStore
    {
        public GetAsync(int offset, int limit, params Expression<Func<TEntity, object>>[] expanders) : base(offset, limit, expanders)
        {
        }

        public GetAsync(int offset, int limit, SortExpression<TEntity> sortTerms, params Expression<Func<TEntity, object>>[] expanders) : base(offset, limit, sortTerms, expanders)
        {
        }
    }
}
