using MediatR;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Text.Json.Serialization;

namespace RadicalR
{
	public class ChangeSetAsync<TStore, TEntity, TDto> : ChangeSet<TStore, TEntity, TDto>, IStreamRequest<Command<TDto>> where TEntity : Entity where TDto : Dto where TStore : IDataStore
	{
		public ChangeSetAsync(PublishMode publishPattern, TDto input, object key)
		 : base(publishPattern, input, key)
		{
		}

		public ChangeSetAsync(PublishMode publishPattern, TDto[] inputs)
             : base(publishPattern, inputs) { }

		public ChangeSetAsync(PublishMode publishPattern, TDto[] inputs, Func<TDto, Expression<Func<TEntity, bool>>> predicate)
           : base(publishPattern, inputs, predicate)
        {
		}
	}
}
