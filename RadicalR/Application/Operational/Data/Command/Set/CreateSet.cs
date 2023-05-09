using System;
using System.Linq;
using System.Linq.Expressions;
using System.Text.Json.Serialization;

namespace RadicalR
{
    public class CreateSet<TStore, TEntity, TDto>  : CommandSet<TDto> where TEntity : Entity where TDto : Dto where TStore : IDataStore
    {       
        [JsonIgnore] public Func<TEntity, Expression<Func<TEntity, bool>>> Predicate { get; }

        public CreateSet(PublishMode publishPattern, TDto input, object key)
       : base(CommandMode.Create, publishPattern, new[] { new CreateCommand<TStore, TEntity, TDto>(publishPattern, input, key) }) { }

        public CreateSet(PublishMode publishPattern, TDto[] inputs) 
            : base(CommandMode.Create, publishPattern, inputs.Select(input => new CreateCommand<TStore, TEntity, TDto>(publishPattern, input)).ToArray())
        {
        }
        public CreateSet(PublishMode publishPattern, TDto[] inputs, Func<TEntity, Expression<Func<TEntity, bool>>> predicate)
           : base(CommandMode.Create, publishPattern, inputs.Select(input => new CreateCommand<TStore, TEntity, TDto>(publishPattern, input, predicate)).ToArray())
        {
            Predicate = predicate;
        }
    }
}
