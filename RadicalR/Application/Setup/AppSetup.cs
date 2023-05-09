using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProtoBuf.Grpc.Server;
using System.Logs;
using System.Series;

namespace RadicalR
{
    public partial class AppSetup : IAppSetup
    {
        public static bool usedExternal;

        IHostBuilder app;
        IHostEnvironment env;

        public AppSetup(IHostBuilder application) { app = application; }

        public AppSetup(IHostBuilder application, IHostEnvironment environment, bool useSwagger)
        {
            app = application;
            env = environment;
        }

        public AppSetup(IHostBuilder application, IHostEnvironment environment, string[] apiVersions = null)
        {
            app = application;
            env = environment;
        }

        public virtual IAppSetup RebuildProviders()
        {
            UseInternalProvider();
            return this;
        }

        public IAppSetup UseDataClients()
        {
            this.LoadClientEdms().ConfigureAwait(true);
            return this;
        }

        public IAppSetup UseDataMigrations()
        {
            using (IServiceScope scope = ServiceManager.GetProvider().CreateScope())
            {
                try
                {
                    IServicer us = scope.ServiceProvider.GetRequiredService<IServicer>();
                    us.GetEndpoints().ForEach(e => ((DbContext)e.Context).Database.Migrate());
                }
                catch (Exception ex)
                {
                    this.Error<Applog>("Data migration initial create - unable to connect the database engine", null, ex);
                }
            }

            return this;
        }

        public virtual IAppSetup UseInternalProvider()
        {
            IServiceManager sm = ServiceManager.GetManager();
            sm.Registry.MergeServices();
            app.UseServiceProviderFactory(ServiceManager.GetServiceFactory());
            usedExternal = false;
            return this;
        }      
    }
}