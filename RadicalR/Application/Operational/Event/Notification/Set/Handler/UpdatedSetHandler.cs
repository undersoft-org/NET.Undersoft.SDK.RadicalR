//-----------------------------------------------------------------------
// <copyright file="UpdatedDtoEventSet.cs" company="Undersoft">
//     Author: Dariusz Hanc
//     Copyright (c) Undersoft. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using MediatR;
using System;
using System.Linq;
using System.Logs;
using System.Series;
using System.Threading;
using System.Threading.Tasks;

namespace RadicalR
{
    public class UpdatedSetHandler<TStore, TEntity, TDto> : INotificationHandler<UpdatedSet<TStore, TEntity, TDto>>
        where TEntity : Entity
        where TDto : Dto
        where TStore : IDataStore
    {
        protected readonly IEntityRepository<Event> _eventStore;
        protected readonly IEntityRepository<TEntity> _repository;

        public UpdatedSetHandler()
        {
        }

        public UpdatedSetHandler(
            IEntityRepository<IReportStore, TEntity> repository,
            IEntityRepository<IEventStore, Event> eventStore)
        {
            _repository = repository;
            _eventStore = eventStore;
        }

        public virtual Task Handle(
            UpdatedSet<TStore, TEntity, TDto> request,
            CancellationToken cancellationToken)
        {
            return Task.Run(
                () =>
                {
                    try
                    {
                        request.ForOnly(
                            d => !d.Command.IsValid,
                            d =>
                            {
                                request.Remove(d);
                            });

                        _eventStore.AddAsync(request).ConfigureAwait(true);

                        if(request.PublishMode == PublishMode.PropagateCommand)
                        {
                            IDeck<TEntity> entities;
                            if(request.Predicate == null)
                                entities = _repository.SetBy(request.Select(d => d.Command.Data)).ToDeck();
                        else if(request.Conditions == null)
                                entities = _repository.SetBy(request.Select(d => d.Command.Data), request.Predicate)
                                    .ToDeck();
                        else
                                entities = _repository.SetBy(
                                    request.Select(d => d.Command.Data),
                                    request.Predicate,
                                    request.Conditions)
                                    .ToDeck();

                            request.ForEach(
                                (r) =>
                                {
                                    _ = entities.ContainsKey(r.AggregateId)
                                        ? (r.PublishStatus = PublishStatus.Complete)
                                        : (r.PublishStatus = PublishStatus.Uncomplete);
                                });
                        }
                    } catch(Exception ex)
                    {
                        this.Failure<Domainlog>(ex.Message, request.Select(r => r.Command.ErrorMessages).ToArray(), ex);
                        request.ForEach((r) => r.PublishStatus = PublishStatus.Error);
                    }
                },
                cancellationToken);
        }
    }
}
