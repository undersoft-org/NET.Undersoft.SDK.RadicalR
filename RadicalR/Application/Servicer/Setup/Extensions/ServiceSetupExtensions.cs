using RadicalR;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceSetupExtensions
    {
        public static IServiceSetup AddServiceSetup(this IServiceCollection services, IMvcBuilder mvcBuilder = null)
        {
            return new ServiceSetup(services);
        }
    }
}
