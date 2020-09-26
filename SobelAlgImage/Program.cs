using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SobelAlgImage.Data;
using SobelAlgImage.Helpers;
using System;
using System.IO;
using System.Runtime.InteropServices.ComTypes;

namespace SobelAlgImage
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var loggerFactory = services.GetRequiredService<ILoggerFactory>();
                var webHost = services.GetRequiredService<IWebHostEnvironment>();

                try
                {
                    var context = services.GetRequiredService<StoreContext>();

                    // add migrations if db does not exists
                    context.Database.EnsureCreated();

                    // generate folders
                    string webRootPath = webHost.WebRootPath;
                    var postsPath = Path.Combine(webRootPath, HelperConstants.OriginalImageBasePath.TrimStart('\\'));
                    var usersPath = Path.Combine(webRootPath, HelperConstants.TransformImageBasePath.TrimStart('\\'));

                    if (!Directory.Exists(postsPath))
                        Directory.CreateDirectory(postsPath);

                    if (!Directory.Exists(usersPath))
                        Directory.CreateDirectory(usersPath);
                }
                catch (Exception ex)
                {
                    var logger = loggerFactory.CreateLogger<Program>();
                    logger.LogError(ex, "An error occured during migrations.");
                }
            }
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
