using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Routing.Attributes;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using System.Threading.Tasks;

namespace RadicalR
{
    [LinkedResult]
    [ODataRouteComponent(StoreRoutes.Constant.EntryStore)]
    public abstract class DsoCommandController<TKey, TStore, TEntity> : ODataController where TEntity : Entity where TStore : IDataStore
    {
        protected readonly IServicer _radicalr;

        protected DsoCommandController(IRadicalr radicalr)
        {
            _radicalr = radicalr;
        }

        [IgnoreApi]
        [HttpPost]
        public virtual async Task<IActionResult> Post(TEntity entity)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            return Created(await _radicalr.Send(new CreateDso<TStore, TEntity>
                                                    (PublishMode.PropagateCommand, entity))
                                                    .ConfigureAwait(false));
        }

        [IgnoreApi]
        [HttpPatch]
        public virtual async Task<IActionResult> Patch([FromODataUri] TKey key, TEntity entity)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            return Updated(await _radicalr.Send(new ChangeDso<TStore, TEntity>
                                                    (PublishMode.PropagateCommand,
                                                    entity.Sign<TEntity>(key)))
                                                    .ConfigureAwait(false));
        }

        [IgnoreApi]
        [HttpPut]
        public virtual async Task<IActionResult> Put([FromODataUri] TKey key, TEntity entity)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            return Updated(await _radicalr.Send(new UpdateDso<TStore, TEntity>
                                                    (PublishMode.PropagateCommand,
                                                    entity.Sign<TEntity>(key)))
                                                    .ConfigureAwait(false));
        }

        [IgnoreApi]
        [HttpDelete]
        public virtual async Task<IActionResult> Delete([FromODataUri] TKey key)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            return Ok(await _radicalr.Send(new DeleteDso<TStore, TEntity>
                                                    (PublishMode.PropagateCommand, key))
                                                    .ConfigureAwait(false));
        }
    }
}
