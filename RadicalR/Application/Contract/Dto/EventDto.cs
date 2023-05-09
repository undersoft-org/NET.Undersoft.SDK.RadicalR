using System;

namespace RadicalR
{
    public class EventDto : Dto
    {
        public virtual int EventVersion { get; set; }
        public virtual string EventType { get; set; }
        public virtual long AggregateId { get; set; }
        public virtual string AggregateType { get; set; }
        public virtual string EventData { get; set; }
        public virtual DateTime PublishTime { get; set; }
        public virtual PublishStatus PublishStatus { get; set; }
    }
}