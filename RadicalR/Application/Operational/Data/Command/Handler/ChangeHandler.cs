using FluentValidation.Results;
using MediatR;
using System;
using System.Logs;
using System.Threading;
using System.Threading.Tasks;

namespace RadicalR
{
    public class ChangeHandler<TStore, TEntity, TDto> : IRequestHandler<ChangeCommand<TStore, TEntity, TDto>, Command<TDto>>
        where TEntity : Entity where TDto : Dto where TStore : IDataStore
    {
        protected readonly IEntityRepository<TEntity> _repository;
        protected readonly IRadicalr _radicalr;

        public ChangeHandler(IRadicalr radicalr, IEntityRepository<TStore, TEntity> repository)
        {
            _radicalr = radicalr;
            _repository = repository;
        }

        public virtual async Task<Command<TDto>> Handle(ChangeCommand<TStore, TEntity, TDto> request, CancellationToken cancellationToken)
        {
            if (!request.Result.IsValid)
                return request;
            try
            {              
                if (request.Keys != null)
                    request.Entity = await _repository.PatchBy(request.Data, request.Keys).ConfigureAwait(false);
                else
                    request.Entity = await _repository.PatchBy(request.Data, request.Predicate).ConfigureAwait(false);

                if (request.Entity == null) throw new Exception($"{ GetType().Name } for entity " +
                                                                $"{ typeof(TEntity).Name } unable patch entry");                
                
                _ = _radicalr.Publish(new Changed<TStore, TEntity, TDto>(request), cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                request.Result.Errors.Add(new ValidationFailure(string.Empty, ex.Message));
                this.Failure<Applog>(ex.Message, request.ErrorMessages, ex);
            }

            return request;
        }
    }
}
