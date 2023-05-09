using MediatR;
using System.Series;

namespace RadicalR
{
    public abstract class CommandEventSet<TCommand> : Deck<CommandEvent<TCommand>>, INotification where TCommand : Command
    {
        public PublishMode PublishMode { get; set; }

        protected CommandEventSet(PublishMode publishPattern, CommandEvent<TCommand>[] commands) : base(commands)
        {
            PublishMode = publishPattern;
        }
    }
}