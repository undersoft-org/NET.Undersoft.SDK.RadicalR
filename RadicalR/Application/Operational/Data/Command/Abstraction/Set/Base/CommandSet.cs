using FluentValidation.Results;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Series;

namespace RadicalR
{
    public class CommandSet<TDto> : Album<Command<TDto>>, IRequest<CommandSet<TDto>>, ICommandSet<TDto> where TDto : Dto
    {
        public CommandMode CommandMode { get; set; }

        public PublishMode PublishMode { get; set; }

        protected CommandSet() : base(true)
        {
        }
        protected CommandSet(CommandMode commandMode) : base()
        {
            CommandMode = commandMode;
        }
        protected CommandSet(CommandMode commandMode, PublishMode publishPattern, Command<TDto>[] DtoCommands) : base(DtoCommands)
        {
            CommandMode = commandMode;
            PublishMode = publishPattern;
        }

        public IEnumerable<Command<TDto>> Commands { get => AsValues(); }

        public ValidationResult Result { get; set; } = new ValidationResult();

        public object Input => Commands.Select(c => c.Data);

        public object Output => Commands.ForEach(c => (c.Result.IsValid) ? c.Id as object : c.Result);

        IEnumerable<ICommand> ICommandSet.Commands { get => this.Cast<ICommand>(); }
    }
}