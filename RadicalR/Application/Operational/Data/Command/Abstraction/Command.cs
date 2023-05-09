
using FluentValidation.Results;
using System;
using System.Text.Json.Serialization;

namespace RadicalR
{
    public abstract class Command : ICommand
    {
        private Entity entity;

        public virtual long Id { get; set; }

        public object[] Keys { get; set; }

        [JsonIgnore] public virtual Entity Entity 
        { 
            get => entity; 
            set 
            {
                entity = value;
                if (Id == 0 && entity.Id != 0)
                    Id = entity.Id;
            }  
        }

        [JsonIgnore] public virtual object Data { get; set; }

        [JsonIgnore] public ValidationResult Result { get; set; }

        public string ErrorMessages => Result.ToString();

        public CommandMode CommandMode { get; set; }

        public PublishMode PublishMode { get; set; }

        public virtual object Input => Data;

        public virtual object Output => (IsValid) ? Id : ErrorMessages;

        public bool IsValid => Result.IsValid;

        protected Command()
        {
            Result = new ValidationResult();
        }

        protected Command(CommandMode commandMode, PublishMode publishMode) : this()
        {
            CommandMode = commandMode;
            PublishMode = publishMode;
        }
        protected Command(object entryData, CommandMode commandMode, PublishMode publishMode) : this(commandMode, publishMode)
        {
            Data = entryData;
        }

        protected Command(object entryData, CommandMode commandMode, PublishMode publishMode, params object[] keys) : this(commandMode, publishMode, keys)
        {
            Data = entryData;            
        }

        protected Command(CommandMode commandMode, PublishMode publishMode, params object[] keys) : this(commandMode, publishMode)
        {        
            Keys = keys;
        }
    }
}