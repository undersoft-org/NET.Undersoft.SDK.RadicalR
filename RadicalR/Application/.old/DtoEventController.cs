using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
using System.Text.Json;

namespace RadicalR
{
    [ApiController]
    public abstract class DtoEventController<TKey, TStore, TEntity, TDto> : ControllerBase
        where TEntity : Entity
        where TDto : Dto
        where TStore : IDataStore
    {
        protected Func<TKey, Func<TDto, object>> _keysetter = k => e => e.SetId(k);
        protected Func<TKey, Expression<Func<TEntity, bool>>> _keymatcher = k =>
            e => k.Equals(e.Id);
        protected Func<TDto, Expression<Func<TEntity, bool>>> _predicate;
        protected readonly IRadicalr _radicalr;
        protected readonly PublishMode _publishMode;

        protected DtoEventController(IRadicalr radicalr)
        {
            _radicalr = radicalr;
        }

        protected DtoEventController(
            IRadicalr radicalr,
            Func<TKey, Expression<Func<TEntity, bool>>> keymatcher
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

        [HttpPost]
        public virtual async Task<IActionResult> Post([FromBody] TDto[] dtos)
        {
            bool isValid = false;

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _radicalr
                .Send(new CreateSet<TStore, TEntity, TDto>(_publishMode, dtos))
                .ConfigureAwait(false);

            object[] response = result
                .ForEach(c => (isValid = c.IsValid) ? (c.Id as object) : c.ErrorMessages)
                .ToArray();
            return (!isValid) ? UnprocessableEntity(response) : Ok(response);
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
                        new FilterItems<TStore, TEntity, TDto>(
                            offset,
                            limit,
                            new FilterExpression<TEntity>(query.Filter).Create(),
                            new SortExpression<TEntity>(query.Sort)
                        )
                    )
                    .ConfigureAwait(false)
            );
        }

        [HttpPost("{Key}")]
        public virtual async Task<IActionResult> Post(TKey key, [FromBody] TDto dto)
        {
            bool isValid = false;

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _keysetter(key).Invoke(dto);

            var result = await _radicalr
                .Send(new CreateSet<TStore, TEntity, TDto>(_publishMode, new[] { dto }))
                .ConfigureAwait(false);

            var response = result
                .ForEach(c => (isValid = c.IsValid) ? c.Id as object : c.ErrorMessages)
                .ToArray();
            return (!isValid) ? UnprocessableEntity(response) : Ok(response);
        }

        [HttpPatch]
        public virtual async Task<IActionResult> Patch([FromBody] TDto[] dtos)
        {
            bool isValid = false;

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            CommandSet<TDto> result = await _radicalr
                .Send(new ChangeSet<TStore, TEntity, TDto>(PublishMode.PropagateCommand, dtos))
                .ConfigureAwait(false);

            object[] response = result
                .ForEach(c => (isValid = c.IsValid) ? (c.Id as object) : c.ErrorMessages)
                .ToArray();
            return (!isValid) ? UnprocessableEntity(response) : Ok(response);
        }

        [HttpPatch("{key}")]
        public virtual async Task<IActionResult> Patch(TKey key, [FromBody] TDto dto)
        {
            bool isValid = false;

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            Command<TDto> result = await _radicalr
                .Send(new ChangeCommand<TStore, TEntity, TDto>(PublishMode.PropagateCommand, dto, key))
                .ConfigureAwait(false);

            object response = result.IsValid ? (result.Id as object) : result.ErrorMessages;
            return (!isValid) ? UnprocessableEntity(response) : Ok(response);
        }

        [HttpPut]
        public virtual async Task<IActionResult> Put([FromBody] TDto[] dtos)
        {
            bool isValid = false;

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _radicalr.Send(new UpdateSet<TStore, TEntity, TDto>
                                                                        (_publishMode, dtos, _predicate))
                                                                                    .ConfigureAwait(false);

            var response = result.ForEach(c => (isValid = c.IsValid) ? (c.Id as object) : c.ErrorMessages)
                .ToArray();
            return (!isValid) ? UnprocessableEntity(response) : Ok(response);
        }

        [HttpPut("{key}")]
        public virtual async Task<IActionResult> Put(TKey key, [FromBody] TDto dto)
        {
            bool isValid = false;

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _keysetter(key).Invoke(dto);

            var result = await _radicalr.Send(new UpdateSet<TStore, TEntity, TDto>
                                                        (_publishMode, new[] { dto }, _predicate))
                                                            .ConfigureAwait(false);

            var response = result.ForEach(c => (isValid = c.IsValid)
                                                  ? c.Id as object
                                                  : c.ErrorMessages).ToArray();
            return (!isValid)
                   ? UnprocessableEntity(response)
                   : Ok(response);
        }

        [HttpDelete]
        public virtual async Task<IActionResult> Delete([FromBody] TDto[] dtos)
        {
            bool isValid = false;

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            CommandSet<TDto> result = await _radicalr
                .Send(new DeleteSet<TStore, TEntity, TDto>(PublishMode.PropagateCommand, dtos))
                .ConfigureAwait(false);

            object[] response = result
                .ForEach(c => (isValid = c.IsValid) ? (c.Id as object) : c.ErrorMessages)
                .ToArray();
            return (!isValid) ? UnprocessableEntity(response) : Ok(response);
        }

        [HttpDelete("{key}")]
        public virtual async Task<IActionResult> Delete(TKey key, [FromBody] TDto dto)
        {
            bool isValid = false;

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _keysetter(key).Invoke(dto);

            var result = await _radicalr
                .Send(
                    new DeleteSet<TStore, TEntity, TDto>(
                        PublishMode.PropagateCommand,
                        new[] { dto }
                    )
                )
                .ConfigureAwait(false);

            var response = result
                .ForEach(c => (isValid = c.IsValid) ? c.Id as object : c.ErrorMessages)
                .ToArray();
            return (!isValid) ? UnprocessableEntity(response) : Ok(response);
        }


    }
}
