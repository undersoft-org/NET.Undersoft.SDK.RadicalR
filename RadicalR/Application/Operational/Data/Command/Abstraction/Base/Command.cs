using MediatR;
using System;
using System.Text.Json.Serialization;

namespace RadicalR
{
    public class Command<TDto> : Command, IRequest<Command<TDto>>, IUnique where TDto : Dto
    {
        [JsonIgnore] public override TDto Data => base.Data as TDto;     

        protected Command()
        {        
        }
        protected Command(CommandMode commandMode, TDto dataObject)
        {
            CommandMode = commandMode;
            base.Data = dataObject;
        }
        protected Command(CommandMode commandMode, PublishMode publishMode, TDto dataObject) 
            : base(dataObject, commandMode, publishMode)
        {
        }
        protected Command(CommandMode commandMode, PublishMode publishMode, TDto dataObject, params object[] keys) 
            : base(dataObject, commandMode, publishMode, keys)
        {
        }
        protected Command(CommandMode commandMode, PublishMode publishMode, params object[] keys)
           : base(commandMode, publishMode, keys)
        {
        }

        public byte[] GetBytes()
        {
            return Data.GetBytes();
        }

        public byte[] GetUniqueBytes()
        {
            return Data.GetUniqueBytes();
        }
        
        public bool Equals(IUnique other)
        {
            return Data.Equals(other);
        }

        public int CompareTo(IUnique other)
        {
            return Data.CompareTo(other);
        }

        public override long Id { get => Data.Id; set => Data.Id = value; }

        public ulong UniqueKey { get => Data.UniqueKey; set => Data.UniqueKey=value; }

        public ulong UniqueType { get => Data.UniqueType; set => Data.UniqueType=value; }
    }
}