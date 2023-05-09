
using System.Collections.Generic;

namespace RadicalR
{
    public class EventBusOptions
    {
        public IList<IEventHandler> Handlers { get; }

        public EventBusOptions()
        {
            Handlers = new List<IEventHandler>();
        }
    }
}
