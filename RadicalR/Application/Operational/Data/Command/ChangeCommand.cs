using System;
using System.Linq.Expressions;
using System.Text.Json.Serialization;

namespace RadicalR
{
    public class ChangeCommand<TStore, TEntity, TDto>  : Command<TDto> where TEntity : Entity where TDto : Dto where TStore : IDataStore
    {       
        [JsonIgnore] public Func<TDto, Expression<Func<TEntity, bool>>> Predicate { get; }

        public ChangeCommand(PublishMode publishMode, TDto input, params object[] keys) 
            : base(CommandMode.Change, publishMode, input, keys)
        {
        }
        public ChangeCommand(PublishMode publishMode, TDto input, Func<TDto, Expression<Func<TEntity, bool>>> predicate) 
            : base(CommandMode.Change, publishMode, input)
        {
            Predicate = predicate;
        }
    }
}
