using System;

namespace RadicalR
{
    public interface IEvent : IEntity
    {
        uint EventVersion { get; set; }
        string EventType { get; set; }
        byte[] EventData { get; set; }
        long AggregateId { get; set; }
        string AggregateType { get; set; }
        DateTime PublishTime { get; set; }
        PublishStatus PublishStatus { get; set; }
    }
}