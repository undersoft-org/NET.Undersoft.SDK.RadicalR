using System;
using System.Logs;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation.Results;
using MediatR;

namespace RadicalR
{
    public class UpdateHandler<TStore, TEntity, TDto> : IRequestHandler<UpdateCommand<TStore, TEntity, TDto> , Command<TDto>> 
        where TEntity : Entity where TDto : Dto where TStore : IDataStore
    {
        protected readonly IEntityRepository<TEntity> _repository;
        protected readonly IRadicalr _radicalr;

        public UpdateHandler(IRadicalr radicalr, IEntityRepository<TStore, TEntity> repository)
        {
            _repository = repository;
            _radicalr = radicalr;
        }

        public async Task<Command<TDto>> Handle(UpdateCommand<TStore, TEntity, TDto> request, CancellationToken cancellationToken)
        {
            if (!request.Result.IsValid) return request;

            try
            {                
                if (request.Predicate == null)
                    request.Entity = await _repository.SetBy(request.Data, request.Keys);
                else if (request.Conditions == null)
                    request.Entity = await _repository.SetBy(request.Data, request.Predicate);
                else
                    request.Entity = await _repository.SetBy(request.Data, request.Predicate, request.Conditions);

                if (request.Entity == null) throw new Exception($"{ this.GetType().Name } for entity " +
                                                                $"{ typeof(TEntity).Name } unable update entry");
            
                _ = _radicalr.Publish(new Updated<TStore, TEntity, TDto>(request)).ConfigureAwait(false);
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
