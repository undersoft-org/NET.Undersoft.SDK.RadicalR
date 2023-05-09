using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Logs;
using System.Threading;
using System.Threading.Tasks;

namespace RadicalR
{
    public class UpsertSetAsyncHandler<TStore, TEntity, TDto> : IStreamRequestHandler<UpsertSetAsync<TStore, TEntity, TDto> , Command<TDto>>
        where TEntity : Entity where TDto : Dto where TStore : IDataStore
    {
        protected readonly IEntityRepository<TEntity> _repository;       
        protected readonly IRadicalr _radicalr;

        public UpsertSetAsyncHandler(IRadicalr radicalr, IEntityRepository<TStore, TEntity> repository)
        {
            _radicalr = radicalr;
            _repository = repository;
        }

        public virtual IAsyncEnumerable<Command<TDto>> Handle(UpsertSetAsync<TStore, TEntity, TDto> request, CancellationToken cancellationToken)
        {
            try
            {
                IAsyncEnumerable<TEntity> entities;
                if (request.Conditions == null)
                    entities = _repository.PutByAsync(request.ForOnly(d => d.IsValid, d => d.Data), request.Predicate);
                else
                    entities = _repository.PutByAsync(request.ForOnly(d => d.IsValid, d => d.Data), request.Predicate, request.Conditions);

                var response = entities.ForEachAsync((e) => { var r = request[e.Id]; r.Entity = e; return r; });

                _ = _radicalr.Publish(new UpsertedSet<TStore, TEntity, TDto>(request)).ConfigureAwait(false);

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
