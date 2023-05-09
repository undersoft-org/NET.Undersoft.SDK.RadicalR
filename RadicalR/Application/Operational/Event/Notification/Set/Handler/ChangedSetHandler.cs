using MediatR;
using System;
using System.Linq;
using System.Logs;
using System.Series;
using System.Threading;
using System.Threading.Tasks;

namespace RadicalR
{
    public class ChangedSetHandler<TStore, TEntity, TDto> : INotificationHandler<ChangedSet<TStore, TEntity, TDto>>
        where TEntity : Entity where TDto : Dto where TStore : IDataStore
    {
        protected readonly IEntityRepository<TEntity> _repository;
        protected readonly IEntityRepository<Event> _eventStore;

        public ChangedSetHandler() { }
        public ChangedSetHandler(IEntityRepository<IReportStore, TEntity> repository,
                                        IEntityRepository<IEventStore, Event> eventStore)
        {
            _repository = repository;
            _eventStore = eventStore;
        }

        public virtual async Task Handle(ChangedSet<TStore, TEntity, TDto> request, CancellationToken cancellationToken)
        {
            await Task.Run(() =>
            {
                try
                {
                    request.ForOnly(d => !d.Command.IsValid, d => { request.Remove(d); });

                    _eventStore.AddAsync(request).ConfigureAwait(true);

                    if (request.PublishMode == PublishMode.PropagateCommand)
                    {
                        IDeck<TEntity> entities;
                        if (request.Predicate == null)
                            entities = _repository.PatchBy(request.Select(d => d.Command.Data).ToArray()).ToDeck();
                        else
                            entities = _repository.PatchBy(request.Select(d => d.Command.Data).ToArray(), request.Predicate).ToDeck();

                        request.ForEach((r) =>
                        {
                            _ = (entities.ContainsKey(r.AggregateId))
                              ? r.PublishStatus = PublishStatus.Complete
                              : r.PublishStatus = PublishStatus.Uncomplete;
                        });
                    }
                }
                catch (Exception ex)
                {
                    this.Failure<Domainlog>(ex.Message, request.Select(r => r.Command.ErrorMessages).ToArray(), ex);
                    request.ForEach((r) => r.PublishStatus = PublishStatus.Error);
                }
            }, cancellationToken);
        }
    }
}
