using MediatR;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Text.Json.Serialization;

namespace RadicalR
{
    public class DeleteSetAsync<TStore, TEntity, TDto> : DeleteSet<TStore, TEntity, TDto>, IStreamRequest<Command<TDto>> where TEntity : Entity where TDto : Dto where TStore : IDataStore
    {
        [JsonIgnore] public Func<TEntity, Expression<Func<TEntity, bool>>> Predicate { get; }

        public DeleteSetAsync(PublishMode publishPattern, object key)
        : base(publishPattern, key) { }

        public DeleteSetAsync(PublishMode publishPattern, TDto input, object key)
        : base(publishPattern, input, key) { }


        public DeleteSetAsync(PublishMode publishPattern, TDto[] inputs)
        : base(publishPattern, inputs) { }

        public DeleteSetAsync(PublishMode publishPattern, TDto[] inputs, Func<TEntity, Expression<Func<TEntity, bool>>> predicate)
            : base(publishPattern, inputs, predicate) { }
    }
}
