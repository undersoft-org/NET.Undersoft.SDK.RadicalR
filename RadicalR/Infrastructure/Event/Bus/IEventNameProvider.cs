using System;

namespace RadicalR
{
    public interface IEventNameProvider
    {
        string GetName(Type eventType);
    }
}