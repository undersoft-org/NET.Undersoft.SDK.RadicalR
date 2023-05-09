using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace RadicalR
{
    public interface IServiceConfiguration : IConfiguration
    {
        string Version { get; }
        IServiceConfiguration Configure<TOptions>(string sectionName) where TOptions : class;
        IConfigurationSection Client(string name);
        IConfigurationSection DataCacheLifeTime();
        int ClientPoolSize(IConfigurationSection endpoint);
        string ClientConnectionString(IConfigurationSection client);
        string ClientConnectionString(string name);
        ClientProvider ClientProvider(IConfigurationSection client);
        ClientProvider ClientProvider(string name);
        IEnumerable<IConfigurationSection> Clients();
        string Description { get; }
        string DataServiceRoutes(string name);
        IConfigurationSection Endpoint(string name);
        int EndpointPoolSize(IConfigurationSection endpoint);
        string EndpointConnectionString(IConfigurationSection endpoint);
        string EndpointConnectionString(string name);
        EndpointProvider EndpointProvider(IConfigurationSection endpoint);
        EndpointProvider EndpointProvider(string name);
        IEnumerable<IConfigurationSection> Endpoints();
        string Title { get; }
        IConfigurationSection Repository();
        IConfigurationSection IdentityServer();
        string IdentityServerBaseUrl();
        string IdentityServerApiName();
        string[] IdentityServerScopes();
        string[] IdentityServerRoles();
        IdentityConfiguration GetIdentityConfiguration();
        IdentityConfiguration Identity { get; }
    }
}