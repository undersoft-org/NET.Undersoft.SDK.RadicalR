using System.Series;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace RadicalR
{
    public class FilterItemsHandler<TStore, TEntity, TDto>
        : IRequestHandler<FilterItems<TStore, TEntity, TDto>, IDeck<TDto>>
        where TEntity : Entity
        where TStore : IDataStore
    {
        protected readonly IEntityRepository<TEntity> _repository;

        public FilterItemsHandler(IEntityRepository<TStore, TEntity> repository)
        {
            _repository = repository;
        }

        public virtual Task<IDeck<TDto>> Handle(
            FilterItems<TStore, TEntity, TDto> request,
            CancellationToken cancellationToken
        )
        {
            if (request.Predicate == null)
                return _repository.Filter<TDto>(
                    request.Offset,
                    request.Limit,
                    request.Sort,
                    request.Expanders
                );
            return _repository.Filter<TDto>(
                request.Offset,
                request.Limit,
                request.Predicate,
                request.Sort,
                request.Expanders
            );
        }
    }
}
