//-----------------------------------------------------------------------
// <copyright file="UpdatedDtoEventSet.cs" company="Undersoft">
//     Author: Dariusz Hanc
//     Copyright (c) Undersoft. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Text.Json.Serialization;

namespace RadicalR
{
    public class UpdatedSet<TStore, TEntity, TDto> : CommandEventSet<Command<TDto>> where TEntity : Entity
        where TDto : Dto
        where TStore : IDataStore
    {
        public UpdatedSet(UpdateSet<TStore, TEntity, TDto> commands) : base(
            commands.PublishMode,
            commands.ForOnly(
                c => c.Entity != null,
                c => new Updated<TStore, TEntity, TDto>((UpdateCommand<TStore, TEntity, TDto>)c))
                .ToArray())
        {
            Predicate = commands.Predicate;
            Conditions = commands.Conditions;
        }

        [JsonIgnore] public Func<TDto, Expression<Func<TEntity, bool>>>[] Conditions { get; }

        [JsonIgnore] public Func<TDto, Expression<Func<TEntity, bool>>> Predicate { get; }
    }
}
