//-----------------------------------------------------------------------
// <copyright file="CreateDsoCommand.cs" company="Undersoft">
//     Author: Dariusz Hanc
//     Copyright (c) Undersoft. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Linq.Expressions;
using System.Text.Json.Serialization;

namespace RadicalR
{
    public class CreateDso<TStore, TEntity> : DsoCommand<TEntity> where TEntity : Entity
        where TStore : IDataStore
    {
        public CreateDso(PublishMode publishPattern, TEntity input) : base(
            input,
            CommandMode.Create,
            publishPattern)
        { input.AutoId(); }

        public CreateDso(
            PublishMode publishPattern,
            TEntity input,
            Func<TEntity, Expression<Func<TEntity, bool>>> predicate) : base(input, CommandMode.Create, publishPattern)
        {
            input.AutoId();
            Predicate = predicate;
        }

        [JsonIgnore] public Func<TEntity, Expression<Func<TEntity, bool>>> Predicate { get; }
    }
}
