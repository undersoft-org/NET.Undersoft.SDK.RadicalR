using System;
using System.Text.Json.Serialization;
using System.Linq.Expressions;

namespace RadicalR
{
    public class Deleted<TStore, TEntity, TDto>  : CommandEvent<Command<TDto>> where TEntity : Entity where TDto : Dto where TStore : IDataStore
    {
        [JsonIgnore] public Func<TEntity, Expression<Func<TEntity, bool>>> Predicate { get; }

        public Deleted(DeleteCommand<TStore, TEntity, TDto> command) : base(command)
        {
            Predicate = command.Predicate;
        }
    }
}
