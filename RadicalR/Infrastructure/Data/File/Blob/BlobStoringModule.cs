using Microsoft.Extensions.DependencyInjection;


namespace RadicalR
{
    public class BlobStoringModule
    {
        public void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddTransient(
                typeof(IBlobContainer<>),
                typeof(BlobContainer<>)
            );

            context.Services.AddTransient(
                typeof(IBlobContainer),
                serviceProvider => serviceProvider
                    .GetRequiredService<IBlobContainer<DefaultContainer>>()
            );
        }
    }
}