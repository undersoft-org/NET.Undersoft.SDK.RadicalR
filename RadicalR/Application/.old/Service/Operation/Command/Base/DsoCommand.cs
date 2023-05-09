using MediatR;
using System;
using System.Series;
using System.Text.Json.Serialization;

namespace RadicalR
{
    public abstract class DsoCommand<TEntity> : Command, IRequest<TEntity>, ICommand where TEntity : Entity
    {
        public override long Id { get => Data.Id; set => Data.Id = value; }

        [JsonIgnore] public override TEntity Data => base.Data as TEntity;

        protected DsoCommand()
        {
        }
        protected DsoCommand(TEntity dataObject, CommandMode commandMode)
        {
            CommandMode = commandMode;
            base.Data = dataObject;
        }
        protected DsoCommand(TEntity dataObject, CommandMode commandMode, PublishMode publishMode)
            : base(dataObject, commandMode, publishMode)
        {
        }
        protected DsoCommand(TEntity dataObject, CommandMode commandMode, PublishMode publishMode, params object[] keys)
            : base(dataObject, commandMode, publishMode, keys)
        {
        }
        protected DsoCommand(CommandMode commandMode, PublishMode publishMode, params object[] keys)
           : base(commandMode, publishMode, keys)
        {
        }            
    }
}