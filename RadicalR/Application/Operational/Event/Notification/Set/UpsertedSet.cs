using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Text.Json.Serialization;

namespace RadicalR
{
    public class UpsertedSet<TStore, TEntity, TDto>  : CommandEventSet<Command<TDto>> 
        where TEntity : Entity where TDto : Dto where TStore : IDataStore
    {
        [JsonIgnore] public Func<TEntity, Expression<Func<TEntity, bool>>> Predicate { get; }
        [JsonIgnore] public Func<TEntity, Expression<Func<TEntity, bool>>>[] Conditions { get; }

        public UpsertedSet(UpsertSet<TStore, TEntity, TDto> commands) 
            : base(commands.PublishMode, commands.ForOnly(c => c.Entity != null, c => new Upserted<TStore, TEntity, TDto>
            ((UpsertCommand<TStore, TEntity, TDto>)c)).ToArray())
        {            
            Conditions = commands.Conditions;
            Predicate = commands.Predicate;
        }
    }
}
