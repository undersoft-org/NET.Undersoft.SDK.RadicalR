using MediatR;
using System.Series;

namespace RadicalR
{
    public class GetItemsHandler<TStore, TEntity, TDto> : IRequestHandler<GetItems<TStore, TEntity, TDto>, IDeck<TDto>>
        where TEntity : Entity where TStore : IDataStore
    {
        protected readonly IEntityRepository<TEntity> _repository;

        public GetItemsHandler(IEntityRepository<TStore, TEntity> repository)
        {
            _repository = repository;
        }

        public virtual Task<IDeck<TDto>> Handle(GetItems<TStore, TEntity, TDto> request,
                                                CancellationToken cancellationToken)
        {
            return _repository.Get<TDto>(request.Offset, request.Limit, request.Sort, request.Expanders);
        }
    }
}
