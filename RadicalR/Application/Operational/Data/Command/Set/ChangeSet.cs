using System;
using System.Linq;
using System.Linq.Expressions;
using System.Text.Json.Serialization;

namespace RadicalR
{
	public class ChangeSet<TStore, TEntity, TDto> : CommandSet<TDto> where TEntity : Entity where TDto : Dto where TStore : IDataStore
	{
		[JsonIgnore] public Func<TDto, Expression<Func<TEntity, bool>>> Predicate { get; }

		public ChangeSet(PublishMode publishPattern, TDto input, object key)
		 : base(CommandMode.Change, publishPattern, new[] { new ChangeCommand<TStore, TEntity, TDto>(publishPattern, input, key) })
		{
		}

		public ChangeSet(PublishMode publishPattern, TDto[] inputs)
			: base(CommandMode.Change, publishPattern, inputs.Select(c => new ChangeCommand<TStore, TEntity, TDto>(publishPattern, c, c.Id)).ToArray()) { }

		public ChangeSet(PublishMode publishPattern, TDto[] inputs, Func<TDto, Expression<Func<TEntity, bool>>> predicate)
		   : base(CommandMode.Change, publishPattern, inputs.Select(c => new ChangeCommand<TStore, TEntity, TDto>(publishPattern, c, predicate)).ToArray())
		{
			Predicate = predicate;
		}
	}
}
