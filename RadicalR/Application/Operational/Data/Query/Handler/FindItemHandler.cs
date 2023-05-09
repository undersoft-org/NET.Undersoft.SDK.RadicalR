using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace RadicalR
{
    public class FindItemHandler<TStore, TEntity, TDto> : IRequestHandler<FindItem<TStore, TEntity, TDto>, TDto>  
        where TEntity : Entity where TStore : IDataStore
    {
        protected readonly IEntityRepository<TEntity> _repository;

        public FindItemHandler(IEntityRepository<TStore, TEntity> repository)
        {
            _repository = repository;
        }

        public virtual Task<TDto> Handle(FindItem<TStore, TEntity, TDto> request, CancellationToken cancellationToken)
        {
            if(request.Keys != null)
                return _repository.Find<TDto>(request.Keys, request.Expanders);
            return _repository.Find<TDto>(request.Predicate, false, request.Expanders);
        }
    }
}
