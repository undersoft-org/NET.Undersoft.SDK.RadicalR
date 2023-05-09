using Microsoft.AspNetCore.OData.Deltas;
using System;
using System.Linq.Expressions;
using System.Text.Json.Serialization;

namespace RadicalR
{
    public class ChangedDso<TStore, TEntity>  : CommandEvent<DsoCommand<TEntity>> where TEntity : Entity where TStore : IDataStore
    {        
        [JsonIgnore] public Delta<TEntity> Delta { get; }
        [JsonIgnore] public Func<TEntity, Expression<Func<TEntity, bool>>> Predicate { get; }

        public ChangedDso(ChangeDso<TStore, TEntity> command) : base(command)
        {
            Delta = command.Delta;
            Predicate = command.Predicate;
        }
    }
}
