using FluentValidation.Results;
using MediatR;
using System;
using System.Logs;
using System.Threading;
using System.Threading.Tasks;

namespace RadicalR
{
    public class UpdateDsoHandler<TStore, TEntity> : IRequestHandler<UpdateDso<TStore, TEntity>, TEntity> where TEntity : Entity where TStore : IDataStore
    {
        protected readonly IEntityRepository<TEntity> _repository;
        protected readonly IRadicalr _radicalr;

        public UpdateDsoHandler(IRadicalr radicalr, IEntityRepository<TStore, TEntity> repository)
        {
            _radicalr = radicalr;
            _repository = repository;
        }

        public async Task<TEntity> Handle(UpdateDso<TStore, TEntity> request, CancellationToken cancellationToken)
        {
            return await Task.Run(async () =>
            {
                if (!request.Result.IsValid)
                    return request.Data;

                try
                {
                    if (request.Keys != null)
                        request.Entity = await _repository.Set(request.Data, request.Keys);
                    else if (request.Conditions == null)
                        request.Entity = await _repository.Set(request.Data, request.Predicate);
                    else
                        request.Entity = await _repository.Set(request.Data, request.Predicate, request.Conditions);

                    if (request.Entity == null) throw new Exception($"{ this.GetType().Name } for entity " +
                                                $"{ typeof(TEntity).Name } failed");
                                        
                    _ = _radicalr.Publish(new UpdatedDso<TStore, TEntity>(request)).ConfigureAwait(false); ;

                    return request.Entity as TEntity;
                }
                catch (Exception ex)
                {

                    request.Result.Errors.Add(new ValidationFailure(string.Empty, ex.Message));
                    this.Failure<Applog>(ex.Message, request.ErrorMessages, ex);
                }

                return null;
            }, cancellationToken);
        }
    }
}
