using Microsoft.Extensions.Hosting;

namespace RadicalR.Server
{
    public static class AppSetupExtensions
    {
        public static IAppSetup UseAppSetup(this IHostBuilder app, IHostEnvironment env)
        {
            return new AppSetup(app, env);
        }             

        public static IHostBuilder UseInternalProvider(this IHostBuilder app)
        {
            new AppSetup(app).UseInternalProvider();
            return app;
        }

        public static IHostBuilder RebuildProviders(this IHostBuilder app)
        {
            new AppSetup(app).RebuildProviders();
            return app;
        }
    }
}