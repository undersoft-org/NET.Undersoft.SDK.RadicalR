using FluentValidation.Results;
using System.Collections.Generic;

namespace RadicalR
{
    public interface ICommandstream<TDto> : ICommandSet where TDto : Dto
    {
        public new IAsyncEnumerable<Command<TDto>> Commands { get; }
    }
}