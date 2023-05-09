
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Instant;
using System.Linq;
using System.Linq.Expressions;

namespace RadicalR
{
    public abstract class CommandStreamValidatorBase<TCommand> : CommandSetValidatorBase<TCommand> where TCommand : ICommandSet
    {
        public CommandStreamValidatorBase(IServicer servicer) : base(servicer)
        {
        }
    }
}