//-----------------------------------------------------------------------
// <copyright file="ChangedDtoEvent.cs" company="Undersoft">
//     Author: Dariusz Hanc
//     Copyright (c) Undersoft. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Linq.Expressions;
using System.Text.Json.Serialization;

namespace RadicalR
{
    public class Changed<TStore, TEntity, TDto> : CommandEvent<Command<TDto>> where TEntity : Entity
        where TDto : Dto
        where TStore : IDataStore
    {
        public Changed(Command<TDto> command) : base(command)
        {
        }

        public Changed(ChangeCommand<TStore, TEntity, TDto> command) : base(command)
        { Predicate = command.Predicate; }

        [JsonIgnore] public Func<TDto, Expression<Func<TEntity, bool>>> Predicate { get; }
    }
}
