using System;
using System.Linq;
using System.Linq.Expressions;
using System.Text.Json.Serialization;

namespace RadicalR
{
    public class ChangedSet<TStore, TEntity, TDto>  : CommandEventSet<Command<TDto>> 
        where TEntity : Entity where TDto : Dto where TStore : IDataStore
    {  
        [JsonIgnore] public Func<TDto, Expression<Func<TEntity, bool>>> Predicate { get;   }

        public ChangedSet(ChangeSet<TStore, TEntity, TDto> commands) 
            : base(commands.PublishMode, commands.ForOnly(c => c.Entity != null, c => new Changed<TStore, TEntity, TDto>
            ((ChangeCommand<TStore, TEntity, TDto>)c)).ToArray())
        {
            Predicate = commands.Predicate;           
        }
    }
}
