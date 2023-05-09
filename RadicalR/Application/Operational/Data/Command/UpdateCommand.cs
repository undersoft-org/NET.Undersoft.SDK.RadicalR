using System;
using System.Linq.Expressions;
using System.Text.Json.Serialization;

namespace RadicalR
{
    public class UpdateCommand<TStore, TEntity, TDto>  : Command<TDto> where TEntity : Entity where TDto : Dto where TStore : IDataStore
    {
        [JsonIgnore] public Func<TEntity, Expression<Func<TEntity, bool>>> Predicate { get; }
        [JsonIgnore] public Func<TEntity, Expression<Func<TEntity, bool>>>[] Conditions { get; }

        public UpdateCommand(PublishMode publishPattern, TDto input, params object[] keys)
           : base(CommandMode.Update, publishPattern, input, keys)
        {
        }
        
        public UpdateCommand(PublishMode publishPattern, TDto input, Func<TEntity, 
                                Expression<Func<TEntity, bool>>> predicate) 
            : base(CommandMode.Update, publishPattern, input)
        {
            Predicate = predicate;
        }
        
        public UpdateCommand(PublishMode publishPattern, TDto input, Func<TEntity, Expression<Func<TEntity, bool>>> predicate, 
                                params Func<TEntity, Expression<Func<TEntity, bool>>>[] conditions) 
            : base(CommandMode.Update, publishPattern, input)
        {
            Predicate = predicate;
            Conditions = conditions;
        }
    }
}
