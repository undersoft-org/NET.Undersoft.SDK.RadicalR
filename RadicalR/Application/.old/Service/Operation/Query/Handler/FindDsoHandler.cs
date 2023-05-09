using MediatR;
using System.Threading;
using System.Threading.Tasks;
using System.Uniques;

namespace RadicalR
{
    public class FindDsoHandler<TStore, TEntity> : IRequestHandler<FindDso<TStore, TEntity>, UniqueOne<TEntity>> where TEntity : Entity where TStore : IDataStore
    {
        protected readonly IEntityRepository<TEntity> _repository;

        public FindDsoHandler(IEntityRepository<TStore, TEntity> repository)
        {
            _repository = repository;
        }

        public virtual Task<UniqueOne<TEntity>> Handle(FindDso<TStore, TEntity> request, CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                if (request.Predicate == null)
                    return _repository[request.Keys, request.Expanders].AsUniqueOne();
                return _repository[request.Predicate].AsUniqueOne();
            });
        }
    }

}
