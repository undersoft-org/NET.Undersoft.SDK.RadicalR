using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
using System.Text.Json;

namespace RadicalR
{
    [LinkedResult]
    [ApiController]
    public abstract class DtoQueryController<TKey, TStore, TEntity, TDto> : ControllerBase
        where TEntity : Entity
        where TDto : Dto
        where TStore : IDataStore
    {
        protected readonly Func<TKey, Expression<Func<TEntity, bool>>> _keymatcher;
        protected readonly IRadicalr _radicalr;

        protected DtoQueryController(IRadicalr radicalr)
        {
            _radicalr = radicalr;
        }

        protected DtoQueryController(
            Func<TKey, Expression<Func<TEntity, bool>>> keymatcher,
            IRadicalr radicalr
        )
        {
            _radicalr = radicalr;
            _keymatcher = keymatcher;
        }

        [HttpGet]
        public virtual async Task<IActionResult> Get()
        {
            return Ok(
                await _radicalr.Send(new GetItems<TStore, TEntity, TDto>(0, 0)).ConfigureAwait(true)
            );
        }

        [HttpGet("count")]
        public virtual async Task<IActionResult> Count()
        {
            return Ok(await Task.Run(() => _radicalr.use<TStore, TEntity>().Query.Count()));
        }

        [HttpGet("{key}")]
        public virtual async Task<IActionResult> Get(TKey key)
        {
            Task<TDto> query =
                (_keymatcher == null)
                    ? _radicalr.Send(new FindItem<TStore, TEntity, TDto>(key))
                    : _radicalr.Send(new FindItem<TStore, TEntity, TDto>(_keymatcher(key)));

            return Ok(await query.ConfigureAwait(false));
        }

        [HttpGet("{offset}/{limit}")]
        public virtual async Task<IActionResult> Get(int offset, int limit)
        {
            return Ok(
                await _radicalr
                    .Send(new GetItems<TStore, TEntity, TDto>(offset, limit))
                    .ConfigureAwait(true)
            );
        }

        [HttpPost("query/{offset}/{limit}")]
        public virtual async Task<IActionResult> Post(int offset, int limit, QueryItems query)
        {
            query.Filter.ForEach(
                (fi) =>
                    fi.Value = JsonSerializer.Deserialize(
                        ((JsonElement)fi.Value).GetRawText(),
                        Type.GetType($"System.{fi.Type}", null, null, false, true)
                    )
            );

            return Ok(
                await _radicalr
                    .Send(
                        new FilterItems<TStore, TEntity, TDto>(offset, limit,
                            new FilterExpression<TEntity>(query.Filter).Create(),
                            new SortExpression<TEntity>(query.Sort)
                        )
                    )
                    .ConfigureAwait(false)
            );
        }
    }
}
