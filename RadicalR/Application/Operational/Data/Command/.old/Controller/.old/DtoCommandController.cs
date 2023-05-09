using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace RadicalR
{
    [LinkedResult]
    [ApiController]
    public abstract class DtoCommandController<TKey, TStore, TEntity, TDto> : ControllerBase where TEntity : Entity
        where TDto : Dto
        where TStore : IDataStore
    {
        protected Func<TKey, Func<TDto, object>> _keymatcher = k => e => e.SetId(k);
        protected Func<TDto, Expression<Func<TEntity, bool>>> _predicate;
        protected IServicer _radicalr;
        protected PublishMode _publishMode;

        protected DtoCommandController(IRadicalr radicalr, PublishMode publishMode = PublishMode.PropagateCommand) { _radicalr = radicalr; _publishMode = publishMode; }

        protected DtoCommandController(IRadicalr radicalr, Func<TKey, Func<TDto, object>> keymatcher, PublishMode publishMode = PublishMode.PropagateCommand)
            : this(radicalr, publishMode) { _keymatcher = keymatcher; }

        protected DtoCommandController(
            IRadicalr radicalr,
            Func<TDto, Expression<Func<TEntity, bool>>> predicate,
            Func<TKey, Func<TDto, object>> keymatcher,
            PublishMode publishMode = PublishMode.PropagateCommand) : this(radicalr, keymatcher, publishMode) { _predicate = predicate; }

        [HttpDelete]
        public virtual async Task<IActionResult> Delete([FromBody] TDto[] dtos)
        {
            bool isValid = false;

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _radicalr.Send(new DeleteSet<TStore, TEntity, TDto>
                                                                (_publishMode, dtos))
                                                                 .ConfigureAwait(false);

            var response = result.ForEach(c => (isValid = c.IsValid)
                                                       ? c.Id as object
                                                       : c.ErrorMessages).ToArray();
            return (!isValid)
                   ? UnprocessableEntity(response)
                   : Ok(response);
        }

        [HttpDelete("{key}")]
        public virtual async Task<IActionResult> Delete([FromRoute] TKey key, [FromBody] TDto dto)
        {
            bool isValid = false;

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _keymatcher(key).Invoke(dto);

            var result = await _radicalr.Send(new DeleteSet<TStore, TEntity, TDto>
                                                                 (_publishMode, new[] { dto }))
                                                                        .ConfigureAwait(false);

            var response = result.ForEach(c => (isValid = c.IsValid)
                                                   ? c.Id as object
                                                   : c.ErrorMessages).ToArray();
            return (!isValid)
                   ? UnprocessableEntity(response)
                   : Ok(response);
        }

        [HttpPatch]
        public virtual async Task<IActionResult> Patch([FromBody] TDto[] dtos)
        {
            bool isValid = false;

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _radicalr.Send(new ChangeSet<TStore, TEntity, TDto>
                                                                    (_publishMode, dtos, _predicate))
                                                                        .ConfigureAwait(false);
            var response = result.ForEach(c => (isValid = c.IsValid)
                                                  ? c.Id as object
                                                  : c.ErrorMessages).ToArray();
            return (!isValid)
                   ? UnprocessableEntity(response)
                   : Ok(response);
        }

        [HttpPatch("{key}")]
        public virtual async Task<IActionResult> Patch([FromRoute] TKey key, [FromBody] TDto dto)
        {
            bool isValid = false;

            if (!ModelState.IsValid) return BadRequest(ModelState);

            _keymatcher(key).Invoke(dto);

            var result = await _radicalr.Send(new ChangeSet<TStore, TEntity, TDto>
                                                  (_publishMode, new[] { dto }, _predicate))
                                                     .ConfigureAwait(false);

            var response = result.ForEach(c => (isValid = c.IsValid)
                                                  ? c.Id as object
                                                  : c.ErrorMessages).ToArray();
            return (!isValid)
                   ? UnprocessableEntity(response)
                   : Ok(response);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Post([FromBody] TDto[] dtos)
        {
            bool isValid = false;

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            CommandSet<TDto> result = await _radicalr.Send(new CreateSet<TStore, TEntity, TDto>
                                                        (_publishMode, dtos)).ConfigureAwait(false);

            object[] response = result.ForEach(c => (isValid = c.IsValid) ? (c.Id as object) : c.ErrorMessages)
                .ToArray();
            return (!isValid) ? UnprocessableEntity(response) : Ok(response);
        }

        [HttpPost("{key}")]
        public virtual async Task<IActionResult> Post([FromRoute] TKey key, [FromBody] TDto dto)
        {
            bool isValid = false;

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _keymatcher(key).Invoke(dto);

            var result = await _radicalr.Send(new CreateSet<TStore, TEntity, TDto>
                                                    (_publishMode, new[] { dto }))
                                                        .ConfigureAwait(false);

            var response = result.ForEach(c => (isValid = c.IsValid)
                                                  ? c.Id as object
                                                  : c.ErrorMessages).ToArray();
            return (!isValid)
                   ? UnprocessableEntity(response)
                   : Ok(response);
        }

        [HttpPut]
        public virtual async Task<IActionResult> Put([FromBody] TDto[] dtos)
        {
            bool isValid = false;

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            CommandSet<TDto> result = await _radicalr.Send(new UpdateSet<TStore, TEntity, TDto>
                                                                        (_publishMode, dtos, _predicate))
                                                                                    .ConfigureAwait(false);

            object[] response = result.ForEach(c => (isValid = c.IsValid) ? (c.Id as object) : c.ErrorMessages)
                .ToArray();
            return (!isValid) ? UnprocessableEntity(response) : Ok(response);
        }

        [HttpPut("{key}")]
        public virtual async Task<IActionResult> Put([FromRoute] TKey key, [FromBody] TDto dto)
        {
            bool isValid = false;

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _keymatcher(key).Invoke(dto);

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
    }
}
