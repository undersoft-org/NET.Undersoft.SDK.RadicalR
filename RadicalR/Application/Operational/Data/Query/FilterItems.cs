using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Series;
using System.Text.Json.Serialization;

namespace RadicalR
{
    public class FilterItems<TStore, TEntity, TDto> : Query<TStore, TEntity, IDeck<TDto>>
        where TEntity : Entity where TStore : IDataStore
    {
        public FilterItems(int offset, int limit, Expression<Func<TEntity, bool>> predicate) : base(predicate) 
        {
            Offset = offset;
            Limit = limit;
        }
        public FilterItems(int offset, int limit, Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] expanders) : base(predicate, expanders)
        {
            Offset = offset;
            Limit = limit;
        }
        public FilterItems(int offset, int limit, Expression<Func<TEntity, bool>> predicate, SortExpression<TEntity> sortTerms,
                                params Expression<Func<TEntity, object>>[] expanders) : base(predicate, sortTerms, expanders)
        {
            Offset = offset;
            Limit = limit;
        }
    }
}
    