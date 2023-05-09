using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;

namespace RadicalR
{
    public static class ConfigutrationExtensions
    {
        public static IServiceConfiguration BuildConfiguration(this IConfiguration config)
        {
            return new ServiceConfiguration(config);
        }

        public static IServiceConfiguration BuildConfiguration(this IConfiguration config, IServiceCollection services)
        {
            var _config = new ServiceConfiguration(config);
            _config.Services = services;
            return _config;
        }
    }

    public class ServiceConfiguration : IServiceConfiguration
    {
        private IConfiguration config;
        public IServiceCollection Services;

        private IdentityConfiguration _identity;
        public IdentityConfiguration Identity => GetIdentityConfiguration();

        public string this[string key] { get => config[key]; set => config[key] = value; }

        public ServiceConfiguration()
        {
            config = ConfigurationHelper.BuildConfiguration();
        }
        public ServiceConfiguration(IConfiguration config)
        {
            this.config = config;
        }
        public ServiceConfiguration(IServiceCollection services)
        {
            config = ConfigurationHelper.BuildConfiguration();
            this.Services = services;
        }
        public ServiceConfiguration(IConfiguration config, IServiceCollection services)
        {
            this.config = config;
            this.Services = services;
        }

        public IServiceConfiguration Configure<TOptions>(string sectionName) where TOptions : class
        {
            Services.Configure<TOptions>(config.GetSection(sectionName));
            return this;
        }
        public IServiceConfiguration Configure<TOptions>(string sectionName, Action<BinderOptions> configureOptions) where TOptions : class
        {
            Services.Configure<TOptions>(config.GetSection(sectionName), configureOptions);
            return this;
        }
        public IServiceConfiguration Configure<TOptions>(Action<TOptions> configureOptions) where TOptions : class
        {
            Services.Configure<TOptions>(configureOptions);
            return this;
        }

        public string Version => config["Version"];
        public string Title => config["Title"];
        public string Description => config["Description"];

        public string DataServiceRoutes(string name)
        {
            return config.GetSection("StoreRoutes")[name];
        }

        public IConfigurationSection Repository()
        {
            return config.GetSection("Repository");
        }

        public IConfigurationSection DataCacheLifeTime()
        {
            return config.GetSection("DataCache");
        }

        public IEnumerable<IConfigurationSection> Endpoints()
        {
            return config.GetSection("Repository").GetSection("Endpoints").GetChildren();
        }
        public IEnumerable<IConfigurationSection> Clients()
        {
            return Repository().GetSection("Clients").GetChildren();
        }

        public IConfigurationSection Endpoint(string name)
        {
            return Repository()?.GetSection("Endpoints")?.GetSection(name);
        }

        public string EndpointConnectionString(string name)
        {
            return Endpoint(name)["ConnectionString"];
        }
        public string ClientConnectionString(string name)
        {
            return Client(name)["ConnectionString"];
        }

        public string EndpointConnectionString(IConfigurationSection endpoint)
        {
            string connStr = Environment.GetEnvironmentVariable(endpoint.Key);
            if (!String.IsNullOrEmpty(connStr))
            {
                Console.WriteLine($"Connection string for {endpoint.Key} from environment variable");
            }
            var result = connStr ?? endpoint["ConnectionString"];
            return result;
        }
        public string ClientConnectionString(IConfigurationSection client)
        {
            return client["ConnectionString"];
        }

        public IConfigurationSection Client(string name)
        {
            return Repository()?.GetSection("Clients")?.GetSection(name);
        }

        public EndpointProvider EndpointProvider(string name)
        {
            Enum.TryParse(Endpoint(name)["EndpointProvider"], out EndpointProvider result);
            return result;
        }
        public ClientProvider ClientProvider(string name)
        {
            Enum.TryParse(Client(name)["ClientProvider"], out ClientProvider result);
            return result;
        }

        public EndpointProvider EndpointProvider(IConfigurationSection endpoint)
        {
            Enum.TryParse(endpoint["EndpointProvider"], out EndpointProvider result);
            return result;
        }
        public ClientProvider ClientProvider(IConfigurationSection client)
        {
            Enum.TryParse(client["ClientProvider"], out ClientProvider result);
            return result;
        }

        public int EndpointPoolSize(IConfigurationSection endpoint)
        {
            return endpoint.GetValue<int>("PoolSize");
        }
        public int ClientPoolSize(IConfigurationSection client)
        {
            return client.GetValue<int>("PoolSize");
        }

        public IEnumerable<IConfigurationSection> GetChildren()
        {
            return config.GetChildren();
        }

        public IChangeToken GetReloadToken()
        {
            return config.GetReloadToken();
        }

        public IConfigurationSection GetSection(string key)
        {
            return config.GetSection(key);
        }

        public IConfigurationSection IdentityServer()
        {
            return config.GetSection("IdentityServer");
        }
        public string IdentityServerBaseUrl()
        {
            return IdentityServer().GetValue<string>("BaseUrl");
        }
        public string IdentityServerApiName()
        {
            return IdentityServer().GetValue<string>("ApiName");
        }
        public string[] IdentityServerScopes()
        {
            return IdentityServer()?.GetValue<string[]>("Scopes");
        }
        public string[] IdentityServerRoles()
        {
            return IdentityServer()?.GetValue<string[]>("Roles");
        }

        public IdentityConfiguration GetIdentityConfiguration()
        {
            if (_identity != null)
                return _identity;
            _identity = new IdentityConfiguration();
            config.Bind("IdentityServer", _identity);
            return _identity;
        }
    }
}
