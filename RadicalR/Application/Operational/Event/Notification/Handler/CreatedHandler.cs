//-----------------------------------------------------------------------
// <copyright file="CreatedDtoEvent.cs" company="Undersoft">
//     Author: Dariusz Hanc
//     Copyright (c) Undersoft. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using FluentValidation.Results;
using MediatR;
using System;
using System.Logs;
using System.Threading;
using System.Threading.Tasks;

namespace RadicalR
{
    public class CreatedHandler<TStore, TEntity, TDto> : INotificationHandler<Created<TStore, TEntity, TDto>>
        where TEntity : Entity
        where TDto : Dto
        where TStore : IDataStore
    {
        protected readonly IEntityRepository<Event> _eventStore;
        protected readonly IEntityRepository<TEntity> _repository;

        public CreatedHandler()
        {
        }

        public CreatedHandler(
            IEntityRepository<IReportStore, TEntity> repository,
            IEntityRepository<IEventStore, Event> eventStore)
        {
            _repository = repository;
            _eventStore = eventStore;
        }

        public virtual Task Handle(Created<TStore, TEntity, TDto>  request, CancellationToken cancellationToken)
        {
            return Task.Run(
                () =>
                {
                    try
                    {
                        if(_eventStore.Add(request) == null)
                            throw new Exception(
                                $"{($"{GetType().Name} ")}{($"for entity {typeof(TEntity).Name} unable add event")}");

                        if(request.Command.PublishMode == PublishMode.PropagateCommand)
                        {
                            if(_repository.Add((TEntity)request.Command.Entity, request.Predicate) == null)
                                throw new Exception(
                                    $"{($"{GetType().Name} ")}{($"for entity {typeof(TEntity).Name} unable create report")}");

                            request.PublishStatus = PublishStatus.Complete;
                        }
                    } catch(Exception ex)
                    {
                        request.Command.Result.Errors.Add(new ValidationFailure(string.Empty, ex.Message));
                        this.Failure<Domainlog>(ex.Message, request.Command.ErrorMessages, ex);
                        request.PublishStatus = PublishStatus.Error;
                    }
                },
                cancellationToken);
        }
    }
}
