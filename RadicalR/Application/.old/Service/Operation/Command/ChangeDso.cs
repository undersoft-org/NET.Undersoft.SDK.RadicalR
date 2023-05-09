using Microsoft.AspNetCore.OData.Deltas;
using System;
using System.Linq.Expressions;
using System.Text.Json.Serialization;

namespace RadicalR
{
    public class ChangeDso<TStore, TEntity> : DsoCommand<TEntity> where TEntity : Entity where TStore : IDataStore
    {
        [JsonIgnore] public Delta<TEntity> Delta { get; }
        [JsonIgnore] public Func<TEntity, Expression<Func<TEntity, bool>>> Predicate { get; }

        public ChangeDso(PublishMode publishPattern, TEntity input, params object[] keys)
            : base(input, CommandMode.Change, publishPattern, keys)
        {            
        }
        public ChangeDso(PublishMode publishPattern, TEntity input, Func<TEntity, Expression<Func<TEntity, bool>>> predicate) 
            : base(input, CommandMode.Change, publishPattern)
        {
            Predicate = predicate;
        }
        public ChangeDso(PublishMode publishPattern, Delta<TEntity> input, Func<TEntity, Expression<Func<TEntity, bool>>> predicate) 
            : base(input.GetInstance(), CommandMode.Change, publishPattern)
        {
            Delta = input;
            Predicate = predicate;
        }
    }
}
