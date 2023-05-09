﻿using System.Linq.Expressions;
using System.Uniques;

namespace RadicalR
{
    public class FindQuery<TStore, TEntity, TDto> : Query<TStore, TEntity, IQueryable<TDto>>
        where TEntity : Entity where TStore : IDataStore where TDto : class, IUnique
    {
        public FindQuery(params object[] keys) : base(keys) { }
        public FindQuery(object[] keys, params Expression<Func<TEntity, object>>[] expanders) : base(keys, expanders) { }
        public FindQuery(Expression<Func<TEntity, bool>> predicate) : base(predicate) { }
        public FindQuery(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] expanders) : base(predicate, expanders) { }
    }
}
