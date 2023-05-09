using MediatR;
using System;
using System.Linq.Expressions;
using System.Series;
using System.Text.Json.Serialization;

namespace RadicalR
{
    public class FilterDso<TStore, TEntity> : IRequest<IDeck<TEntity>> where TEntity : Entity where TStore : IDataStore
    {
        [JsonIgnore] public Expression<Func<TEntity, bool>> Predicate { get; }
        [JsonIgnore] public SortExpression<TEntity> Sort { get; }
        [JsonIgnore] public Expression<Func<TEntity, object>>[] Expanders { get; }

        public FilterDso(Expression<Func<TEntity, bool>> predicate)
        {
            Predicate = predicate;
        }

        public FilterDso(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] expanders)
        {
            Predicate = predicate;
            Expanders = expanders;
        }

        public FilterDso(Expression<Func<TEntity, bool>> predicate, SortExpression<TEntity> sortTerms,
            params Expression<Func<TEntity, object>>[] expanders)
        {
            Predicate = predicate;
            Sort = sortTerms;
            Expanders = expanders;
        }
    }


}
