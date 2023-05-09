using FluentValidation.Results;
using MediatR;
using System;
using System.Logs;
using System.Threading;
using System.Threading.Tasks;

namespace RadicalR
{
    public class CreateHandler<TStore, TEntity, TDto>  : IRequestHandler<CreateCommand<TStore, TEntity, TDto> , Command<TDto>> 
        where TEntity : Entity where TDto : Dto where TStore : IDataStore
    {
        protected readonly IEntityRepository<TEntity> _repository;       
        protected readonly IRadicalr _radicalr;

        public CreateHandler(IRadicalr radicalr, IEntityRepository<TStore, TEntity> repository)
        {
            _repository = repository;
            _radicalr = radicalr;
        }

        public async Task<Command<TDto>> Handle(CreateCommand<TStore, TEntity, TDto> request, CancellationToken cancellationToken)
        {
            if (!request.Result.IsValid) 
                return request;
            try
            {
                request.Entity= await _repository.AddBy(request.Data, request.Predicate).ConfigureAwait(false);

                if (request.Entity == null) throw new Exception($"{ GetType().Name } " +
                                                                $"for entity { typeof(TEntity).Name } " +
                                                                $"unable create entry");              

                _ = _radicalr.Publish(new Created<TStore, TEntity, TDto>(request)).ConfigureAwait(false); ;
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
