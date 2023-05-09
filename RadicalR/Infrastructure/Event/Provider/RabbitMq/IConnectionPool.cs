using System;
using RabbitMQ.Client;

namespace RadicalR
{
    public interface IConnectionPool : IDisposable
    {
        IConnection Get(string connectionName = null);
    }
}