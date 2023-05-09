using System;
using System.Linq.Expressions;
using System.Text.Json.Serialization;

namespace RadicalR
{
    public class DeleteDso<TStore, TEntity> : DsoCommand<TEntity> where TEntity : Entity where TStore : IDataStore
    {       
        [JsonIgnore] public Func<TEntity, Expression<Func<TEntity, bool>>> PredicateFunction { get; }
        [JsonIgnore] public Expression<Func<TEntity, bool>> PredicateExpression { get; }

        public DeleteDso(PublishMode publishPattern, TEntity input) 
            : base(input, CommandMode.Delete, publishPattern)
        {
        }

        public DeleteDso(PublishMode publishPattern, params object[] keys)
         : base(CommandMode.Delete, publishPattern, keys)
        {
        }

        public DeleteDso(PublishMode publishPattern, Expression<Func<TEntity, bool>> predicate) : base(CommandMode.Delete, publishPattern)
        {
            PredicateExpression = predicate;
        }

        public DeleteDso(PublishMode publishPattern, TEntity input, Func<TEntity, Expression<Func<TEntity, bool>>> predicate) 
            : base(input, CommandMode.Delete, publishPattern)
        {
            PredicateFunction = predicate;
        }
    }
}
