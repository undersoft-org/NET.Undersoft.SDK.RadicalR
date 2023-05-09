﻿using MediatR;
using System;
using System.Linq;
using System.Logs;
using System.Threading;
using System.Threading.Tasks;

namespace RadicalR
{
    public class CreatedSetHandler<TStore, TEntity, TDto> : INotificationHandler<CreatedSet<TStore, TEntity, TDto>>
        where TEntity : Entity where TDto : Dto where TStore : IDataStore
    {
        protected readonly IEntityRepository<TEntity> _repository;
        protected readonly IEntityRepository<Event> _eventStore;

        public CreatedSetHandler() { }
        public CreatedSetHandler(IEntityRepository<IReportStore, TEntity> repository,
                                        IEntityRepository<IEventStore, Event> eventStore)
        {
            _repository = repository;
            _eventStore = eventStore;
        }

        public virtual Task Handle(CreatedSet<TStore, TEntity, TDto> request, CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                try
                {
                    request.ForOnly(d => !d.Command.IsValid, d => { request.Remove(d); });

                    _eventStore.AddAsync(request).ConfigureAwait(true);

                    if (request.PublishMode == PublishMode.PropagateCommand)
                    {
                        var entities = _repository.Add(request.Select(d => d.Command.Entity).Cast<TEntity>(), request.Predicate).ToDeck();

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
