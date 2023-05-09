using SharpCompress.Common;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Text.Json.Serialization;

namespace RadicalR
{
    public class UpdateSet<TStore, TEntity, TDto>  : CommandSet<TDto> where TEntity : Entity where TDto : Dto where TStore : IDataStore
    {
        [JsonIgnore] public Func<TDto, Expression<Func<TEntity, bool>>> Predicate { get; }
        [JsonIgnore] public Func<TDto, Expression<Func<TEntity, bool>>>[] Conditions { get; }

        public UpdateSet(PublishMode publishPattern, TDto input, object key)
       : base(CommandMode.Change, publishPattern, new[] { new UpdateCommand<TStore, TEntity, TDto>(publishPattern, input, key) }) { }

        public UpdateSet(PublishMode publishPattern, TDto[] inputs)
          : base(CommandMode.Update, publishPattern, inputs.Select(input => new UpdateCommand<TStore, TEntity, TDto>(publishPattern, input)).ToArray())
        {            
        }
        
        public UpdateSet(PublishMode publishPattern, TDto[] inputs, Func<TDto, Expression<Func<TEntity, bool>>> predicate) 
            : base(CommandMode.Update, publishPattern, inputs.Select(input => new UpdateCommand<TStore, TEntity, TDto>(publishPattern, input, predicate)).ToArray())
        {
            Predicate = predicate;
        }
       
        public UpdateSet(PublishMode publishPattern, TDto[] inputs, Func<TDto, Expression<Func<TEntity, bool>>> predicate, 
                                    params Func<TDto, Expression<Func<TEntity, bool>>>[] conditions)
            : base(CommandMode.Update, publishPattern, inputs.Select(input => new UpdateCommand<TStore, TEntity, TDto>(publishPattern, input, predicate, conditions)).ToArray())
        {
            Predicate = predicate;
            Conditions = conditions;
        }
    }
}
