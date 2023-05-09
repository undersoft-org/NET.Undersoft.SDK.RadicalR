using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RadicalR
{
    public class GetDsoHandler<TStore, TEntity> : IRequestHandler<GetDso<TStore, TEntity>, IQueryable<TEntity>> where TEntity : Entity where TStore : IDataStore
    {
        protected readonly IEntityRepository<TEntity> _repository;

        public GetDsoHandler(IEntityRepository<TStore, TEntity> repository)
        {
            _repository = repository;
        }

        public virtual Task<IQueryable<TEntity>> Handle(GetDso<TStore, TEntity> request, CancellationToken cancellationToken)
        {
            return Task.Run(() => _repository[request.Sort, request.Expanders]);
        }
    }
}
