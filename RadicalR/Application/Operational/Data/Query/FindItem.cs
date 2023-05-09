using System;
using System.Linq.Expressions;
using System.Text.Json.Serialization;

namespace RadicalR
{
    public class FindItem<TStore, TEntity, TDto> : Query<TStore, TEntity, TDto>
        where TEntity : Entity where TStore : IDataStore
    {
        public FindItem(params object[] keys) : base(keys) { }
        public FindItem(object[] keys, params Expression<Func<TEntity, object>>[] expanders) : base(keys, expanders) { }
        public FindItem(Expression<Func<TEntity, bool>> predicate) : base(predicate) { }
        public FindItem(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] expanders) : base(predicate, expanders) { }
    }
}
