using System.Series;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace RadicalR
{
    public class FilterAsyncHandler<TStore, TEntity, TDto>
        : IStreamRequestHandler<FilterAsync<TStore, TEntity, TDto>, TDto>
        where TEntity : Entity
        where TStore : IDataStore
    {
        protected readonly IEntityRepository<TEntity> _repository;

        public FilterAsyncHandler(IEntityRepository<TStore, TEntity> repository)
        {
            _repository = repository;
        }

        public virtual IAsyncEnumerable<TDto> Handle(
            FilterAsync<TStore, TEntity, TDto> request,
            CancellationToken cancellationToken
        )
        {
            if (request.Predicate == null)
                return _repository.FilterAsync<TDto>(
                    request.Offset,
                    request.Limit,
                    request.Sort,
                    request.Expanders
                );
            return _repository.FilterAsync<TDto>(
                request.Offset,
                request.Limit,
                request.Predicate,
                request.Sort,
                request.Expanders
            );
        }
    }
}
