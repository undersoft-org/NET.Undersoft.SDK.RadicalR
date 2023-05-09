using System;
using System.Linq.Expressions;
using System.Text.Json.Serialization;

namespace RadicalR
{
    public class DeleteCommand<TStore, TEntity, TDto>  : Command<TDto> where TEntity : Entity where TDto : Dto where TStore : IDataStore
    {
        [JsonIgnore] public Func<TEntity,  Expression<Func<TEntity, bool>>> Predicate { get; }

        public DeleteCommand(PublishMode publishPattern, TDto input) 
            : base(CommandMode.Delete, publishPattern, input)
        {
        }
        public DeleteCommand(PublishMode publishPattern, TDto input, Func<TEntity, Expression<Func<TEntity, bool>>> predicate)
            : base(CommandMode.Delete, publishPattern, input)
        {
            Predicate = predicate;
        }
        public DeleteCommand(PublishMode publishPattern, Func<TEntity, Expression<Func<TEntity, bool>>> predicate) : base(CommandMode.Delete, publishPattern)
        {
            Predicate = predicate;
        }
        public DeleteCommand(PublishMode publishPattern, params object[] keys) 
            : base(CommandMode.Delete, publishPattern, keys)
        {
        }
    }
}
