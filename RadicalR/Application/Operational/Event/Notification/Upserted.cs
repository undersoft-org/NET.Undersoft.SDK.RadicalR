using System;
using System.Text.Json.Serialization;
using System.Linq.Expressions;

namespace RadicalR
{
    public class Upserted<TStore, TEntity, TDto>  : CommandEvent<Command<TDto>> where TEntity : Entity where TDto : Dto where TStore : IDataStore
    {
        [JsonIgnore] public Func<TEntity, Expression<Func<TEntity, bool>>> Predicate { get; }
        [JsonIgnore] public Func<TEntity, Expression<Func<TEntity, bool>>>[] Conditions { get; }

        public Upserted(UpsertCommand<TStore, TEntity, TDto> command) : base(command)
        {
            Conditions = command.Conditions;
            Predicate = command.Predicate;
        }
    }
}
