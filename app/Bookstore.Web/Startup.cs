using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Bookstore.Common;

namespace Bookstore.Web
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(builder =>
            {
                builder.AddConfiguration(Configuration.GetSection("Logging"));
                builder.AddConsole();
            });

            ConfigureConfiguration(services);

            ConfigureDependencyInjection(services);

            ConfigureAuthentication(services);
        }

        private void ConfigureConfiguration(IServiceCollection services)
        {
            // Add configuration setup logic here
        }

        private void ConfigureDependencyInjection(IServiceCollection services)
        {
            // Add dependency injection setup logic here
        }

        private void ConfigureAuthentication(IServiceCollection services)
        {
            // Add authentication setup logic here
        }

        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            // Configure the HTTP request pipeline here
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}