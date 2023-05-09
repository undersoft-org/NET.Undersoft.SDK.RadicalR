using FluentValidation.Results;
using System.Collections.Generic;

namespace RadicalR
{
    public interface ICommandSet<TDto> : ICommandSet where TDto : Dto
    {
        public new IEnumerable<Command<TDto>> Commands { get; }
    }

    public interface ICommandSet : IDataIO
    {
        public IEnumerable<ICommand> Commands { get; }

        public ValidationResult Result { get; set; }
    }
}