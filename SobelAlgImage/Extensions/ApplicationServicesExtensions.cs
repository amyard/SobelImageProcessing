using Microsoft.Extensions.DependencyInjection;
using SobelAlgImage.Infrastructure.Interfaces;
using SobelAlgImage.Infrastructure.Managers;
using SobelAlgImage.Infrastructure.Repository;
using SobelAlgImage.Infrastructure.Services;

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
