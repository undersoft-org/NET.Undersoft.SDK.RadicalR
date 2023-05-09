using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Logs;
using System.Threading;
using System.Threading.Tasks;

namespace RadicalR
{
    public class UpdateSetHandler<TStore, TEntity, TDto> : IRequestHandler<UpdateSet<TStore, TEntity, TDto> , CommandSet<TDto>> 
        where TEntity : Entity where TDto : Dto where TStore : IDataStore
    {
        protected readonly IEntityRepository<TEntity> _repository;
        protected readonly IRadicalr _radicalr;

        public UpdateSetHandler(IRadicalr radicalr, IEntityRepository<TStore, TEntity> repository)
        {
            _repository = repository;
            _radicalr = radicalr;
        }

        public async Task<CommandSet<TDto>> Handle(UpdateSet<TStore, TEntity, TDto> request, CancellationToken cancellationToken)
        {
            try
            {
                IEnumerable<TEntity> entities = null;
                if (request.Predicate == null)
                    entities = _repository.SetBy(request.ForOnly(d => d.IsValid, d => d.Data));
                else if (request.Conditions == null)
                    entities = _repository.SetBy(request.ForOnly(d => d.IsValid, d => d.Data), request.Predicate);
                else
                    entities = _repository.SetBy(request.ForOnly(d => d.IsValid, d => d.Data), request.Predicate, request.Conditions);

                await entities.ForEachAsync((e) => { request[e.Id].Entity = e; }).ConfigureAwait(false);

                _ = _radicalr.Publish(new UpdatedSet<TStore, TEntity, TDto>(request)).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                this.Failure<Domainlog>(ex.Message, request.Select(r => r.ErrorMessages).ToArray(), ex);
            }
            return request;
        }
    }
}
