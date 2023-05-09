using System;
using System.Linq.Expressions;

namespace RadicalR
{
    public interface IQuery<TEntity> : IDataIO where TEntity : Entity
    {
        Expression<Func<TEntity, object>>[] Expanders { get; }
        Expression<Func<TEntity, bool>> Predicate { get; }
        SortExpression<TEntity> Sort { get; }
    }
}