//-----------------------------------------------------------------------
// <copyright file="UpdatedDsoEvent.cs" company="Undersoft">
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
    public class UpdatedDsoHandler<TStore, TEntity> : INotificationHandler<UpdatedDso<TStore, TEntity>>
        where TEntity : Entity
        where TStore : IDataStore
    {
        protected readonly IEntityRepository<Event> _eventStore;
        protected readonly IEntityRepository<TEntity> _repository;

        public UpdatedDsoHandler(
            IEntityRepository<IReportStore, TEntity> repository,
            IEntityRepository<IEventStore, Event> eventStore)
        {
            _repository = repository;
            _eventStore = eventStore;
        }

        public virtual Task Handle(UpdatedDso<TStore, TEntity> request, CancellationToken cancellationToken)
        {
            return Task.Run(
                async () =>
                {
                    try
                    {
                        if(_eventStore.Add(request) == null)
                            throw new Exception(
                                $"{$"{GetType().Name} "}{$"for entity {typeof(TEntity).Name} unable add event"}");

                        if(request.Command.PublishMode == PublishMode.PropagateCommand)
                        {
                            DsoCommand<TEntity> cmd = request.Command;
                            if(request.Conditions != null)
                                cmd.Entity = await _repository.Set(
                                    request.Command.Data,
                                    request.Predicate,
                                    request.Conditions);
                        else
                                cmd.Entity = await _repository.Set(request.Command.Data, request.Predicate);

                            if(cmd.Entity == null)
                                throw new Exception(
                                    $"{$"{GetType().Name} "}{$"for entity {typeof(TEntity).Name} unable update report"}");

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
