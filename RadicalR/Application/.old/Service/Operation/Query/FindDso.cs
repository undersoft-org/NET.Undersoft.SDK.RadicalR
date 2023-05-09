using MediatR;
using System;
using System.Linq.Expressions;
using System.Text.Json.Serialization;
using System.Uniques;

namespace RadicalR
{
    public class FindDso<TStore, TEntity> : IRequest<UniqueOne<TEntity>> where TEntity : Entity where TStore : IDataStore
    {
        [JsonIgnore] public object[] Keys { get; }
        [JsonIgnore] public Expression<Func<TEntity, bool>> Predicate { get; }
        [JsonIgnore] public Expression<Func<TEntity, object>>[] Expanders { get; }

        public FindDso(params object[] keys)
        {
            Keys = keys;
        }

        public FindDso(object[] keys, params Expression<Func<TEntity, object>>[] expanders)
        {
            Keys = keys;
            Expanders = expanders;
        }

        public FindDso(Expression<Func<TEntity, bool>> predicate)
        {
            Predicate = predicate;
        }

        public FindDso(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] expanders)
        {
            Predicate = predicate;
            Expanders = expanders;
        }
    }

}
