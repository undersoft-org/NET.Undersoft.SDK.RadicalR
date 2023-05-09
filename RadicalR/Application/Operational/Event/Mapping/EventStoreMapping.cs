using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace RadicalR
{
    public class EventStoreMapping : EntityTypeMapping<Event>
    {
        private const string TABLE_NAME = "EventStore";

        public override void Configure(EntityTypeBuilder<Event> builder)
        {
            builder.ToTable(TABLE_NAME, "Event");

            builder.Property(p => p.PublishTime)
                .HasColumnType("timestamp");
        }
    }
}