using MediatR;
using System;
using System.Linq.Expressions;
using System.Text.Json.Serialization;

namespace RadicalR
{
    public class UpdateDso<TStore, TEntity> : DsoCommand<TEntity>, IRequest<TEntity> where TEntity : Entity where TStore : IDataStore
    {
        [JsonIgnore] public Func<TEntity, Expression<Func<TEntity, bool>>> Predicate { get; }
        [JsonIgnore] public Func<TEntity, Expression<Func<TEntity, bool>>>[] Conditions { get; }

        public UpdateDso(PublishMode publishPattern, TEntity input, params object[] keys)
           : base(input, CommandMode.Update, publishPattern, keys)
        {            
        }
        public UpdateDso(PublishMode publishPattern, TEntity input, Func<TEntity, Expression<Func<TEntity, bool>>> predicate)
             : base(input, CommandMode.Update, publishPattern)
        {            
            Predicate = predicate;
        }
        public UpdateDso(PublishMode publishPattern, TEntity input, Func<TEntity, Expression<Func<TEntity, bool>>> predicate, params Func<TEntity, Expression<Func<TEntity, bool>>>[] conditions)
            : base(input, CommandMode.Update, publishPattern)
        {         
            Predicate = predicate;
            Conditions = conditions;
        }
    }
}
