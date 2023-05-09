using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Series;
using System.Text.Json.Serialization;

namespace RadicalR
{
    public class FilterAsync<TStore, TEntity, TDto> : FilterItems<TStore, TEntity, IDeck<TDto>>, IStreamRequest<TDto>
        where TEntity : Entity where TStore : IDataStore
    {
        public FilterAsync(int offset, int limit, Expression<Func<TEntity, bool>> predicate) : base(offset, limit, predicate) { }
        public FilterAsync(int offset, int limit, Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] expanders) : base(offset, limit, predicate, expanders) { }
        public FilterAsync(int offset, int limit, Expression<Func<TEntity, bool>> predicate, SortExpression<TEntity> sortTerms,
                                params Expression<Func<TEntity, object>>[] expanders) : base(offset, limit, predicate, sortTerms, expanders) { }
    }
}
    