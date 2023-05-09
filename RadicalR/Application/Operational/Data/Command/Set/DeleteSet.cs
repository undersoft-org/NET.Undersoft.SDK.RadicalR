using System;
using System.Linq;
using System.Linq.Expressions;
using System.Text.Json.Serialization;

namespace RadicalR
{
    public class DeleteSet<TStore, TEntity, TDto> : CommandSet<TDto> where TEntity : Entity where TDto : Dto where TStore : IDataStore
    {
        [JsonIgnore] public Func<TEntity, Expression<Func<TEntity, bool>>> Predicate { get; }

        public DeleteSet(PublishMode publishPattern, object key)
      : base(CommandMode.Create, publishPattern, new[] { new DeleteCommand<TStore, TEntity, TDto>(publishPattern, key) }) { }

        public DeleteSet(PublishMode publishPattern, TDto input, object key)
        : base(CommandMode.Create, publishPattern, new[] { new DeleteCommand<TStore, TEntity, TDto>(publishPattern, input, key) }) { }


        public DeleteSet(PublishMode publishPattern, TDto[] inputs)
            : base(CommandMode.Delete, publishPattern, inputs.Select(input => new DeleteCommand<TStore, TEntity, TDto>(publishPattern, input)).ToArray())
        {
        }
        public DeleteSet(PublishMode publishPattern, TDto[] inputs, Func<TEntity, Expression<Func<TEntity, bool>>> predicate)
           : base(CommandMode.Delete, publishPattern, inputs.Select(input => new DeleteCommand<TStore, TEntity, TDto>(publishPattern, input, predicate)).ToArray())
        {
            Predicate = predicate;
        }
    }
}
