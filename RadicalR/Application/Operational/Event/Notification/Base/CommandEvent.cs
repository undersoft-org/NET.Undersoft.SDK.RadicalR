using MediatR;
using System.Logs;
using System.Text.Json;
using System.Uniques;

namespace RadicalR
{
    public abstract class CommandEvent<TCommand> : Event, INotification where TCommand : Command
    {
        public TCommand Command { get; }

        protected CommandEvent(TCommand command)
        {
            var aggregateTypeFullName = command.Entity.ProxyRetypeFullName();
            var eventTypeFullName = GetType().FullName;

            Command = command;
            Id = (long)Unique.New;
            AggregateId = command.Id;
            AggregateType = aggregateTypeFullName;
            EventType = eventTypeFullName;
            var entity = (Entity)command.Entity;
            OriginKey = entity.OriginKey;
            OriginName = entity.OriginName;
            Modifier = entity.Modifier;
            Modified = entity.Modified;
            Creator = entity.Creator;
            Created = entity.Created;
            PublishStatus = PublishStatus.Ready;
            PublishTime = Log.Clock;

            EventData = JsonSerializer.SerializeToUtf8Bytes((Command)command);
        }
    }
}