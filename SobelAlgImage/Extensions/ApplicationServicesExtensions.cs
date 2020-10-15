using Microsoft.Extensions.DependencyInjection;

namespace SobelAlgImage.Extensions
{
    public static class ApplicationServicesExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IImageAlgorithmRepo, ImageAlgorithmRepo>();
            services.AddTransient<IFileManager, FileManager>();
            services.AddTransient<IGeneralService, GeneralService>();

            return services;
        }
    }
}
