using FluentValidation.Results;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Series;

namespace RadicalR
{
    public class CommandStream<TDto> : CommandSet<TDto>, IStreamRequest<Command<TDto>>, ICommandSet<TDto> where TDto : Dto
    {
        protected CommandStream() : base()
        {
        }
        protected CommandStream(CommandMode commandMode) : base(commandMode)
        {
        }
        protected CommandStream(CommandMode commandMode, PublishMode publishPattern, Command<TDto>[] DtoCommands) : base(commandMode, publishPattern, DtoCommands)
        {
        }
    }
}