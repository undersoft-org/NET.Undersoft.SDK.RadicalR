//-----------------------------------------------------------------------
// <copyright file="UpdatedDsoEvent.cs" company="Undersoft">
//     Author: Dariusz Hanc
//     Copyright (c) Undersoft. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Linq.Expressions;
using System.Text.Json.Serialization;

namespace RadicalR
{
    public class UpdatedDso<TStore, TEntity> : CommandEvent<DsoCommand<TEntity>> where TEntity : Entity
        where TStore : IDataStore
    {
        public UpdatedDso(UpdateDso<TStore, TEntity> command) : base(command)
        {
            Predicate = command.Predicate;
            Conditions = command.Conditions;
        }

        [JsonIgnore] public Func<TEntity, Expression<Func<TEntity, bool>>>[] Conditions { get; }

        [JsonIgnore] public Func<TEntity, Expression<Func<TEntity, bool>>> Predicate { get; }
    }
}
