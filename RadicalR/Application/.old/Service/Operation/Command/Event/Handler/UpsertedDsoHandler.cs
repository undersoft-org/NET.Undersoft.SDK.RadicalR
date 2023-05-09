using System;
using System.Logs;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation.Results;
using MediatR;

namespace RadicalR
{
    public class UpsertedDsoHandler<TStore, TEntity> : INotificationHandler<UpsertedDso<TStore, TEntity>> where TEntity : Entity where TStore : IDataStore
    {
        protected readonly IEntityRepository<TEntity> _repository;
        protected readonly IEntityRepository<Event> _eventStore;

        public UpsertedDsoHandler(IEntityRepository<IReportStore, TEntity> repository,
                                       IEntityRepository<IEventStore, Event> eventStore)
        {
            _repository = repository;
            _eventStore = eventStore;
        }

        public virtual Task Handle(UpsertedDso<TStore, TEntity> request, CancellationToken cancellationToken)
        {
            return Task.Run(async () =>
            {
                try
                {
                    if (_eventStore.Add(request) == null)
                        throw new Exception($"{ GetType().Name } " +
                               $"for entity { typeof(TEntity).Name } unable add event");

                    if (request.Command.PublishMode == PublishMode.PropagateCommand)
                    {
                       var cmd = request.Command;
                        if (request.Conditions != null)
                            cmd.Entity = await _repository.Put((TEntity)cmd.Data, request.Predicate, request.Conditions);
                        else
                            cmd.Entity = await _repository.Put((TEntity)cmd.Data, request.Predicate);

                        if (cmd.Entity == null)
                            throw new Exception($"{ this.GetType().Name } " +
                                 $"for entity { typeof(TEntity).Name } unable renew report");

                        request.PublishStatus = PublishStatus.Complete;
                    }
                }
                catch (Exception ex)
                {
                    request.Command.Result.Errors.Add(new ValidationFailure(string.Empty, ex.Message));
                    this.Failure<Domainlog>(ex.Message, request.Command.ErrorMessages, ex);
                    request.PublishStatus = PublishStatus.Error;
                }
            }, cancellationToken);
        }
    }
}
