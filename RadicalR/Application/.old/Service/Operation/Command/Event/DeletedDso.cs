using System;
using System.Text.Json.Serialization;
using System.Linq.Expressions;

namespace RadicalR
{
    public class DeletedDso<TStore, TEntity> : CommandEvent<DsoCommand<TEntity>> where TEntity : Entity where TStore : IDataStore
    {
        [JsonIgnore] public Func<TEntity, Expression<Func<TEntity, bool>>> PredicateFunction { get; }
        [JsonIgnore] public Expression<Func<TEntity, bool>> PredicateExpression { get; }

        public DeletedDso(DeleteDso<TStore, TEntity> command) : base(command)
        {
            PredicateExpression = command.PredicateExpression;
            PredicateFunction = command.PredicateFunction;
        }
    }
}
