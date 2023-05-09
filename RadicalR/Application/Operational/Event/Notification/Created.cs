//-----------------------------------------------------------------------
// <copyright file="CreatedDtoEvent.cs" company="Undersoft">
//     Author: Dariusz Hanc
//     Copyright (c) Undersoft. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Linq.Expressions;
using System.Text.Json.Serialization;

namespace RadicalR
{
    public class Created<TStore, TEntity, TDto> : CommandEvent<Command<TDto>> where TEntity : Entity
        where TDto : Dto
        where TStore : IDataStore
    {
        public Created(CreateCommand<TStore, TEntity, TDto> command) : base(command)
        { Predicate = command.Predicate; }

        [JsonIgnore] public Func<TEntity, Expression<Func<TEntity, bool>>> Predicate { get; }
    }
}
