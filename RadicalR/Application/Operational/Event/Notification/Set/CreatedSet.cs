using System;
using System.Linq;
using System.Linq.Expressions;
using System.Series;
using System.Text.Json.Serialization;

namespace RadicalR
{
    public class CreatedSet<TStore, TEntity, TDto>  : CommandEventSet<Command<TDto>> 
        where TEntity : Entity where TDto : Dto where TStore : IDataStore
    {  
        [JsonIgnore] public Func<TEntity, Expression<Func<TEntity, bool>>> Predicate { get; }

        public CreatedSet(CreateSet<TStore, TEntity, TDto> commands) 
            : base(commands.PublishMode, commands.ForOnly(c => c.Entity != null, c => new Created<TStore, TEntity, TDto>
            ((CreateCommand<TStore, TEntity, TDto>)c)).ToArray())
        {
            Predicate = commands.Predicate;           
        }
    }
}
