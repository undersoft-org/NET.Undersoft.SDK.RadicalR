using System;

namespace RadicalR
{
    public class Event : Entity, IEvent
    {
        public virtual uint EventVersion { get; set; }
        public virtual string EventType { get; set; }
        public virtual long AggregateId { get; set; }
        public virtual string AggregateType { get; set; }
        public virtual byte[] EventData { get; set; }
        public virtual DateTime PublishTime { get; set; }
        public virtual PublishStatus PublishStatus { get; set; }
    }
}