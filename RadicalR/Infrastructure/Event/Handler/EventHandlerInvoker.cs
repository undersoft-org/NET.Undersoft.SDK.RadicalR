using System;
using System.Collections.Concurrent;
using System.Series;
using System.Threading.Tasks;

namespace RadicalR
{

    public class EventHandlerInvoker : IEventHandlerInvoker
    {
        private readonly Catalog<IEventHandlerMethodExecutor> _cache;

        public EventHandlerInvoker()
        {
            _cache = new Catalog<IEventHandlerMethodExecutor>();
        }

        public async Task InvokeAsync(IEventHandler eventHandler, object eventData, Type eventType)
        {
            var cacheItem = _cache.EnsureGet($"{eventHandler.GetType().FullName}-{eventType.FullName}", _ =>
            {
                return (IEventHandlerMethodExecutor)typeof(EventHandlerMethodExecutor<>).MakeGenericType(eventType).New();
            });



            if (cacheItem.Value != null)
            {
                await cacheItem.Value.ExecutorAsync(eventHandler, eventData);
            }
            else
            { 
                throw new Exception("The object instance is not an event handler. Object type: " + eventHandler.GetType().AssemblyQualifiedName);
            }
        }
    }
}