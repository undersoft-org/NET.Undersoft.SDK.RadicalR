using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using System.Linq.Expressions;
using System.Uniques;

namespace RadicalR
{
    [LinkedResult]
    [IgnoreApi]
    public abstract class DsoController<TKey, TStore, TEntity> : ODataController, IDsoController<TKey, TStore, TEntity> where TEntity : Entity where TStore : IDataServiceStore
    {
        protected readonly Func<TKey, Expression<Func<TEntity, bool>>> _keymatcher;
        protected readonly IRadicalr _radicalr;
        protected readonly PublishMode _publishMode;

        protected DsoController() { }
        protected DsoController(IRadicalr radicalr, PublishMode publishMode = PublishMode.PropagateCommand) : this(radicalr, k => e => k.Equals(e.Id), publishMode)
        {
        }
        protected DsoController(IRadicalr radicalr, Func<TKey, Expression<Func<TEntity, bool>>> keymatcher, PublishMode publishMode = PublishMode.PropagateCommand)
        {
            _keymatcher = keymatcher;
            _radicalr = radicalr;
            _publishMode = publishMode;
        }

        [EnableQuery]
        [HttpGet]
        public virtual IQueryable Get()
        {
            return _radicalr.Use<IReportStore, TEntity>().AsQueryable();
        }

        [EnableQuery]
        [HttpGet]
        public virtual UniqueOne Get([FromODataUri] TKey key)
        {
            return _radicalr.Use<IReportStore, TEntity>()[_keymatcher(key)].AsUniqueOne();
        }

        [HttpPost]
        public virtual async Task<IActionResult> Post(TEntity entity)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            return Created(await _radicalr.Send(new CreateDso<IEntryStore, TEntity>
                                                    (_publishMode, entity))
                                                    .ConfigureAwait(false));
        }

        [HttpPatch]
        public virtual async Task<IActionResult> Patch([FromODataUri] TKey key, TEntity entity)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            return Updated(await _radicalr.Send(new ChangeDso<IEntryStore, TEntity>
                                                    (_publishMode, entity, key))
                                                    .ConfigureAwait(false));
        }

        [HttpPut]
        public virtual async Task<IActionResult> Put([FromODataUri] TKey key, TEntity entity)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            return Updated(await _radicalr.Send(new UpdateDso<IEntryStore, TEntity>
                                                     (_publishMode, entity, key))
                                                    .ConfigureAwait(false));
        }

        [HttpDelete]
        public virtual async Task<IActionResult> Delete([FromODataUri] TKey key)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            return Ok(await _radicalr.Send(new DeleteDso<IEntryStore, TEntity>
                                                    (_publishMode, key))
                                                    .ConfigureAwait(false));
        }
    }
}
