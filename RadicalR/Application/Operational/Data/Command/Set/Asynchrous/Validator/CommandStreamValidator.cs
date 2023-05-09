using FluentValidation;
using System;
using System.Collections;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace RadicalR
{
    public class CommandStreamValidator<TDto> : CommandSetValidator<TDto> where TDto : Dto
    {
        public CommandStreamValidator(IServicer servicer) : base(servicer) { }     
    }
}