using MediatR;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Text.Json.Serialization;

namespace RadicalR
{
    public class UpsertSetAsync<TStore, TEntity, TDto>  : UpsertSet<TStore, TEntity, TDto>, IStreamRequest<Command<TDto>> where TEntity : Entity where TDto : Dto where TStore : IDataStore
    {
        public UpsertSetAsync(PublishMode publishPattern, TDto input, object key)
        : base(publishPattern, input, key) { }

        public UpsertSetAsync(PublishMode publishPattern, TDto[] inputs, Func<TEntity, Expression<Func<TEntity, bool>>> predicate)
            : base(publishPattern, inputs, predicate) { }
        public UpsertSetAsync(PublishMode publishPattern, TDto[] inputs, Func<TEntity, Expression<Func<TEntity, bool>>> predicate, params Func<TEntity, Expression<Func<TEntity, bool>>>[] conditions)
           : base(publishPattern, inputs, predicate, conditions) { }
    }
}
