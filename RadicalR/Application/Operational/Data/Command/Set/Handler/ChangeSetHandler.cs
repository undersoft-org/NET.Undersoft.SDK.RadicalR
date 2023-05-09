using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Logs;
using System.Threading;
using System.Threading.Tasks;

namespace RadicalR
{
	public class ChangeSetHandler<TStore, TEntity, TDto> : IRequestHandler<ChangeSet<TStore, TEntity, TDto>, CommandSet<TDto>>
		where TEntity : Entity where TDto : Dto where TStore : IDataStore
	{
		protected readonly IEntityRepository<TEntity> _repository;
		protected readonly IRadicalr _radicalr;

		public ChangeSetHandler(IRadicalr radicalr, IEntityRepository<TStore, TEntity> repository)
		{
            _radicalr = radicalr;
			_repository = repository;
		}

		public virtual async Task<CommandSet<TDto>> Handle(ChangeSet<TStore, TEntity, TDto> request, CancellationToken cancellationToken)
		{
			try
			{
				IEnumerable<TEntity> entities;
				if (request.Predicate == null)
					entities = _repository.PatchBy(request.ForOnly(d => d.IsValid, d => d.Data));
				else
					entities = _repository.PatchBy(request.ForOnly(d => d.IsValid, d => d.Data), request.Predicate);

				await entities.ForEachAsync((e) => { request[e.Id].Entity = e; }).ConfigureAwait(false);

				_ = _radicalr.Publish(new ChangedSet<TStore, TEntity, TDto>(request)).ConfigureAwait(false);
			}
			catch (Exception ex)
			{
				this.Failure<Domainlog>(ex.Message, request.Select(r => r.Output).ToArray(), ex);
			}
			return request;
		}
	}
}
