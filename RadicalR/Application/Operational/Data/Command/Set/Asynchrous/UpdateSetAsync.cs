using MediatR;
using SharpCompress.Common;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Text.Json.Serialization;

namespace RadicalR
{
    public class UpdateSetAsync<TStore, TEntity, TDto> : UpdateSet<TStore, TEntity, TDto>, IStreamRequest<Command<TDto>> where TEntity : Entity where TDto : Dto where TStore : IDataStore
    {
        public UpdateSetAsync(PublishMode publishPattern, TDto input, object key)
       : base(publishPattern, input, key) { }

        public UpdateSetAsync(PublishMode publishPattern, TDto[] inputs)
          : base(publishPattern, inputs)
        {            
        }
        
        public UpdateSetAsync(PublishMode publishPattern, TDto[] inputs, Func<TDto, Expression<Func<TEntity, bool>>> predicate) 
            : base(publishPattern, inputs, predicate) { }
       
        public UpdateSetAsync(PublishMode publishPattern, TDto[] inputs, Func<TDto, Expression<Func<TEntity, bool>>> predicate, 
                                    params Func<TDto, Expression<Func<TEntity, bool>>>[] conditions)
            : base(publishPattern, inputs, predicate, conditions) { }        
    }
}
