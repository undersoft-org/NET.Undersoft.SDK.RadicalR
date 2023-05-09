using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Series;

namespace RadicalR
{
   
    public class ServiceConfigurationContext
    {
        public IServiceRegistry Services { get; }

        public IDeck<object> Items { get; }

        public object this[string key]
        {
            get => Items[key];
            set => Items[key] = value;
        }

        public ServiceConfigurationContext([DisallowNull] IServiceRegistry services)
        {
            Services = services;
            Items = new Catalog<object>();
        }
    }
}
