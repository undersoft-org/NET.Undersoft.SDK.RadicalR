using System;
using System.Logs;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation.Results;
using MediatR;

namespace RadicalR
{
    public class DeletedDsoHandler<TStore, TEntity> : INotificationHandler<DeletedDso<TStore, TEntity>> where TEntity : Entity where TStore : IDataStore
    { 
        protected readonly IEntityRepository<TEntity> _repository;
        protected readonly IEntityRepository<Event> _eventStore;

        public DeletedDsoHandler(IEntityRepository<IReportStore, TEntity> repository,
                                       IEntityRepository<IEventStore, Event> eventStore)
        {
            _repository = repository;
            _eventStore = eventStore;
        }

        public virtual Task Handle(DeletedDso<TStore, TEntity> request, CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                try
                {
                    if (_eventStore.Add(request) == null)
                        throw new Exception($"{ GetType().Name } " +
                               $"for entity { typeof(TEntity).Name } unable add event");

                    var cmd = request.Command;
                    if (cmd.PublishMode == PublishMode.PropagateCommand)
                    {                     
                        if (cmd.Data == null)
                            cmd.Entity = _repository.Delete(request.PredicateExpression);
                        else
                            cmd.Entity = _repository.Delete((TEntity)cmd.Data, request.PredicateFunction);

                        if (cmd.Entity == null)
                            throw new Exception($"{ this.GetType().Name } " +
                                 $"for entity { typeof(TEntity).Name } unable delete report");

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
