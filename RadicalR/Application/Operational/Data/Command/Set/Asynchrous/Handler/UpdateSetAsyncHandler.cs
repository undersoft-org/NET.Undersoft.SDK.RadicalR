using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Logs;
using System.Threading;
using System.Threading.Tasks;

namespace RadicalR
{
    public class UpdateSetAsyncHandler<TStore, TEntity, TDto> : IStreamRequestHandler<UpdateSetAsync<TStore, TEntity, TDto>, Command<TDto>>
        where TEntity : Entity where TDto : Dto where TStore : IDataStore
    {
        protected readonly IEntityRepository<TEntity> _repository;
        protected readonly IRadicalr _radicalr;

        public UpdateSetAsyncHandler(IRadicalr radicalr, IEntityRepository<TStore, TEntity> repository)
        {
            _repository = repository;
            _radicalr = radicalr;
        }

        public IAsyncEnumerable<Command<TDto>> Handle(UpdateSetAsync<TStore, TEntity, TDto> request, CancellationToken cancellationToken)
        {
            try
            {
                IAsyncEnumerable<TEntity> entities;
                if (request.Predicate == null)
                    entities = _repository.SetByAsync(request.ForOnly(d => d.IsValid, d => d.Data));
                else if (request.Conditions == null)
                    entities = _repository.SetByAsync(request.ForOnly(d => d.IsValid, d => d.Data), request.Predicate);
                else
                    entities = _repository.SetByAsync(request.ForOnly(d => d.IsValid, d => d.Data), request.Predicate, request.Conditions);

                var response = entities.ForEachAsync((e) => { var r = request[e.Id]; r.Entity = e; return r; });

                _ = _radicalr.Publish(new UpdatedSet<TStore, TEntity, TDto>(request)).ConfigureAwait(false);

                return response;
            }
            catch (Exception ex)
            {
                this.Failure<Domainlog>(ex.Message, request.Select(r => r.ErrorMessages).ToArray(), ex);
            }
            return null;
        }
    }
}
