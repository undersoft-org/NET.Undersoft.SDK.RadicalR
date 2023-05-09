using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Logs;
using System.Threading;
using System.Threading.Tasks;

namespace RadicalR
{
    public class DeleteSetAsyncHandler<TStore, TEntity, TDto> : IStreamRequestHandler<DeleteSetAsync<TStore, TEntity, TDto> , Command<TDto>>
        where TEntity : Entity where TDto : Dto where TStore : IDataStore
    {
        protected readonly IEntityRepository<TEntity> _repository;       
        protected readonly IRadicalr _radicalr;

        public DeleteSetAsyncHandler(IRadicalr radicalr, IEntityRepository<TStore, TEntity> repository)
        {
            _radicalr = radicalr;
            _repository = repository;
        }

        public virtual IAsyncEnumerable<Command<TDto>> Handle(DeleteSetAsync<TStore, TEntity, TDto> request, CancellationToken cancellationToken)
        {
            try
            {
                IAsyncEnumerable<TEntity> entities;             
                if (request.Predicate == null)
                    entities = _repository.DeleteByAsync(request.ForOnly(d => d.IsValid, d => d.Data));
                else
                    entities = _repository.DeleteByAsync(request.ForOnly(d => d.IsValid, d => d.Data), request.Predicate);

                var response = entities.ForEachAsync((e) => { var r = request[e.Id]; r.Entity = e; return r; });

                _ = _radicalr.Publish(new DeletedSet<TStore, TEntity, TDto>(request)).ConfigureAwait(false);

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
