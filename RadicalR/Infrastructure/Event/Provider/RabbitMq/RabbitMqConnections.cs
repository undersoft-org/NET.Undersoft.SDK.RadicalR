using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Series;
using JetBrains.Annotations;
using RabbitMQ.Client;

namespace RadicalR
{
    [Serializable]
    public class RabbitMqConnections : Catalog<ConnectionFactory>
    {
        public const string DefaultConnectionName = "Default";

        [DisallowNull]
        public ConnectionFactory Default
        {
            get => this[DefaultConnectionName];
            set => this[DefaultConnectionName] = value;
        }

        public RabbitMqConnections()
        {
            Default = new ConnectionFactory();
        }

        public ConnectionFactory GetOrDefault(string connectionName)
        {
            if (TryGet(connectionName, out ConnectionFactory connectionFactory))
            {
                return connectionFactory;
            }

            return Default;
        }
    }
}