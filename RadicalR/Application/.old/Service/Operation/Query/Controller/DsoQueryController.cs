using System;
using System.Linq;
using System.Linq.Expressions;
using System.Uniques;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Attributes;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace RadicalR
{
    [LinkedResult]
    [ODataAttributeRouting]
    [ODataRouteComponent(StoreRoutes.Constant.ReportStore)]
    public abstract class DsoQueryController<TKey, TStore, TEntity> : ODataController where TEntity : Entity where TStore : IDataStore
    {
        protected readonly Func<TKey, Expression<Func<TEntity, bool>>> _keymatcher;
        protected readonly IEntityRepository<TEntity> _repository;

        protected DsoQueryController(IRadicalr radicalr) : this(radicalr, k => e => k.Equals(e.Id))
        {
        }
        protected DsoQueryController(IRadicalr radicalr, Func<TKey, Expression<Func<TEntity, bool>>> keymatcher)
        {
            _repository = radicalr.Use<TStore, TEntity>();
            _keymatcher = keymatcher;
        }

        [EnableQuery]
        [IgnoreApi]
        [HttpGet]
        public virtual IQueryable<TEntity> Get()
        {
            return _repository.AsQueryable();
        }

        [EnableQuery]
        [IgnoreApi]
        [HttpGet]
        public virtual UniqueOne<TEntity> Get([FromODataUri] TKey key)
        {
            return _repository[_keymatcher(key)].AsUniqueOne();
        }
    }
}
