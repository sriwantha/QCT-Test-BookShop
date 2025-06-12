using Microsoft.AspNetCore.Owin;
using Owin;
using Microsoft.Owin;
using NLog;

[assembly: OwinStartup(typeof(Bookstore.Web.Startup))]

namespace Bookstore.Web
{
    public static class LoggingSetup
    {
        public static void ConfigureLogging()
        {
            // Configure logging here
            var logger = LogManager.GetCurrentClassLogger();
            logger.Info("Logging initialized");
        }
    }

    public static class ConfigurationSetup
    {
        public static void ConfigureConfiguration()
        {
            // Configure application settings here
        }
    }

    public static class DependencyInjectionSetup
    {
        public static void ConfigureDependencyInjection(IAppBuilder app)
        {
            // Configure dependency injection here
        }
    }

    public static class AuthenticationConfig
    {
        public static void ConfigureAuthentication(IAppBuilder app)
        {
            // Configure authentication here
        }
    }

    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            LoggingSetup.ConfigureLogging();

            ConfigurationSetup.ConfigureConfiguration();

            DependencyInjectionSetup.ConfigureDependencyInjection(app);

            AuthenticationConfig.ConfigureAuthentication(app);
        }
    }
}