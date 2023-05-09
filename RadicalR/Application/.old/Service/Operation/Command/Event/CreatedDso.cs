using System;
using System.Linq.Expressions;
using System.Text.Json.Serialization;

namespace RadicalR
{
    public class CreatedDso<TStore, TEntity> : CommandEvent<DsoCommand<TEntity>> where TEntity : Entity where TStore : IDataStore
    {
        [JsonIgnore] public Func<TEntity, Expression<Func<TEntity, bool>>> Predicate { get; }

        public CreatedDso(CreateDso<TStore, TEntity> command) : base(command)
        {
            Predicate = command.Predicate;
        }
    }
}
