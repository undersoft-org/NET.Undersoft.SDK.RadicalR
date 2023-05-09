using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Uniques;

namespace RadicalR
{
    [LinkedResult]
    [IgnoreApi]
    public abstract class DsoEventController<TKey, TStore, TEntity> : ODataController
        where TEntity : Entity
        where TStore : IDataStore
    {
        protected readonly Func<TKey, Expression<Func<TEntity, bool>>> _keymatcher;
        protected readonly IServicer _radicalr;

        protected DsoEventController() { }

        protected DsoEventController(IRadicalr radicalr) : this(radicalr, k => e => k.Equals(e.Id))
        { }

        protected DsoEventController(
            IRadicalr radicalr,
            Func<TKey, Expression<Func<TEntity, bool>>> keymatcher
        )
        {
            _keymatcher = keymatcher;
            _radicalr = radicalr;
        }


        [HttpDelete]
        public virtual async Task<IActionResult> Delete([FromODataUri] TKey key)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(
                await _radicalr
                    .Send(new DeleteDso<TStore, TEntity>(PublishMode.PropagateCommand, key))
                    .ConfigureAwait(false)
            );
        }

        [EnableQuery]
        [HttpGet]
        public virtual IQueryable Get()
        {
            return _radicalr.Use<TStore, TEntity>().AsQueryable();
        }

        [EnableQuery]
        [HttpGet]
        public virtual UniqueOne Get([FromODataUri] TKey key)
        {
            return _radicalr.Use<TStore, TEntity>()[_keymatcher(key)].AsUniqueOne();
        }

        [HttpPatch]
        public virtual async Task<IActionResult> Patch([FromODataUri] TKey key, TEntity entity)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Updated(
                await _radicalr
                    .Send(
                        new ChangeDso<TStore, TEntity>(
                            PublishMode.PropagateCommand,
                            entity.Sign<TEntity>(key)
                        )
                    )
                    .ConfigureAwait(false)
            );
        }

        [HttpPost]
        public virtual async Task<IActionResult> Post(TEntity entity)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Created(
                await _radicalr
                    .Send(new CreateDso<TStore, TEntity>(PublishMode.PropagateCommand, entity))
                    .ConfigureAwait(false)
            );
        }

        [HttpPut]
        public virtual async Task<IActionResult> Put([FromODataUri] TKey key, TEntity entity)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Updated(
                await _radicalr
                    .Send(
                        new UpdateDso<TStore, TEntity>(
                            PublishMode.PropagateCommand,
                            entity.Sign<TEntity>(key)
                        )
                    )
                    .ConfigureAwait(false)
            );
        }
    }
}
