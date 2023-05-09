//-----------------------------------------------------------------------
// <copyright file="UpdatedDtoEvent.cs" company="Undersoft">
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
    public class UpdatedHandler<TStore, TEntity, TDto> : INotificationHandler<Updated<TStore, TEntity, TDto>>
        where TEntity : Entity
        where TDto : Dto
        where TStore : IDataStore
    {
        protected readonly IEntityRepository<Event> _eventStore;
        protected readonly IEntityRepository<TEntity> _repository;

        public UpdatedHandler()
        {
        }

        public UpdatedHandler(
            IEntityRepository<IReportStore, TEntity> repository,
            IEntityRepository<IEventStore, Event> eventStore)
        {
            _repository = repository;
            _eventStore = eventStore;
        }

        public virtual Task Handle(Updated<TStore, TEntity, TDto> request, CancellationToken cancellationToken)
        {
            return Task.Run(
                async () =>
                {
                    try
                    {
                        if(_eventStore.Add(request) == null)
                            throw new Exception(
                                $"{($"{GetType().Name} or entity ")}{($"{typeof(TEntity).Name} unable add event")}");

                        if(request.Command.PublishMode == PublishMode.PropagateCommand)
                        {
                            TEntity result;
                            if(request.Predicate == null)
                                result = await _repository.SetBy(request.Command.Data);
                        else if(request.Conditions == null)
                                result = await _repository.SetBy(request.Command.Data, request.Predicate);
                        else
                                result = await _repository.SetBy(
                                    request.Command.Data,
                                    request.Predicate,
                                    request.Conditions);

                            if(result == null)
                                throw new Exception(
                                    $"{($"{GetType().Name} for entity ")}{($"{typeof(TEntity).Name} unable update report")}");

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
