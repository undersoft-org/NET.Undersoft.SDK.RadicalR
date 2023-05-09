using MediatR;

namespace RadicalR
{
    public class GetAsyncHandler<TStore, TEntity, TDto> : IStreamRequestHandler<GetAsync<TStore, TEntity, TDto>, TDto>
        where TEntity : Entity where TStore : IDataStore where TDto : class
    {
        protected readonly IEntityRepository<TEntity> _repository;

        public GetAsyncHandler(IEntityRepository<TStore, TEntity> repository)
        {
            _repository = repository;
        }

        public virtual IAsyncEnumerable<TDto> Handle(GetAsync<TStore, TEntity, TDto> request,
                                                CancellationToken cancellationToken)
        {
            return _repository.GetAsync<TDto>(request.Offset, request.Limit, request.Sort, request.Expanders);
        }
    }
}
