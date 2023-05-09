using System;
using System.Text.Json.Serialization;
using System.Linq.Expressions;

namespace RadicalR
{
    public class UpsertedDso<TStore, TEntity> : CommandEvent<DsoCommand<TEntity>> where TEntity : Entity where TStore : IDataStore
    {
        [JsonIgnore] public Func<TEntity, Expression<Func<TEntity, bool>>> Predicate { get; }
        [JsonIgnore] public Func<TEntity, Expression<Func<TEntity, bool>>>[] Conditions { get; }

        public UpsertedDso(UpsertDso<TStore, TEntity> command) : base(command)
        {
            Predicate = command.Predicate;
            Conditions = command.Conditions;
        }
    }
}
