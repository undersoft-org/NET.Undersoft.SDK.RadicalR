using System;
using System.Linq.Expressions;
using System.Text.Json.Serialization;
using MediatR;

namespace RadicalR
{
    public class UpsertDso<TStore, TEntity> : DsoCommand<TEntity>, IRequest<TEntity> where TEntity : Entity where TStore : IDataStore
    {      
        [JsonIgnore] public Func<TEntity, Expression<Func<TEntity, bool>>> Predicate { get; }
        [JsonIgnore] public Func<TEntity, Expression<Func<TEntity, bool>>>[] Conditions { get; }

        public UpsertDso(PublishMode publishPattern, TEntity input, Func<TEntity, Expression<Func<TEntity, bool>>> predicate) 
            : base(input, CommandMode.Upsert, publishPattern)
        {
            Predicate = predicate;
        }
        public UpsertDso(PublishMode publishPattern, TEntity input, Func<TEntity, Expression<Func<TEntity, bool>>> predicate, params Func<TEntity, Expression<Func<TEntity, bool>>>[] conditions)
           : base(input, CommandMode.Upsert, publishPattern)
        {
            Predicate = predicate;
            Conditions = conditions;
        }
    }
}
