using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace RadicalR
{
    [DataContract]
    public class IdentifierDto<TDto> : IdentifierDto where TDto : Dto
    {
        [JsonIgnore]
        [IgnoreDataMember]
        public virtual TDto Entity { get; set; }
    }

    public class IdentifierDto : Dto
    {
        public virtual long EntityId { get; set; }

        public IdKind Kind { get; set; }

        public string Name { get; set; }

        public string Value { get; set; }

        public long Key { get; set; }
    }
}