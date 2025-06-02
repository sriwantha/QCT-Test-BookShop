
    using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Data.Entity.SqlServer;

    namespace Bookstore
    {
        public class Program
        {
            public static void Main(string[] args)
            {
                var builder = WebApplication.CreateBuilder(args);

                // Add connection strings from web.config
                builder.Configuration.AddJsonFile("appsettings.json", optional: true)
                    .AddEnvironmentVariables();

                // Store configuration in static ConfigurationManager
                ConfigurationManager.Configuration = builder.Configuration;

                // Add connection string from web.config
                // Using System.Data.Entity for EF6
                var connectionString = builder.Configuration.GetConnectionString("BookstoreDatabaseConnection");

                // Add services to the container (formerly ConfigureServices)
                builder.Services.AddControllersWithViews();

                // Add App settings from web.config
                builder.Services.Configure<AppSettings>(options => {
                    options.Environment = builder.Configuration["AppSettings:Environment"] ?? "Development";
                    options.ServicesAuthentication = builder.Configuration["AppSettings:Services/Authentication"] ?? "local";
                    options.ServicesDatabase = builder.Configuration["AppSettings:Services/Database"] ?? "local";
                    options.ServicesFileService = builder.Configuration["AppSettings:Services/FileService"] ?? "local";
                    options.ServicesImageValidation = builder.Configuration["AppSettings:Services/ImageValidationService"] ?? "local";
                    options.ServicesLogging = builder.Configuration["AppSettings:Services/LoggingService"] ?? "local";
                });

                // Register areas
                builder.Services.AddMvc()
                    .AddMvcOptions(options =>
                    {
                        // Configure MVC options that were previously in FilterConfig
                        options.EnableEndpointRouting = true;
                    });

                // Configure client validation from web.config appSettings
                builder.Services.AddMvc().AddViewOptions(options => {
                    options.HtmlHelperOptions.ClientValidationEnabled = true;
                });

                var app = builder.Build();

                // Configure the HTTP request pipeline (formerly Configure method)
                if (app.Environment.IsDevelopment())
                {
                    app.UseDeveloperExceptionPage();
                }
                else
                {
                    app.UseExceptionHandler("/Home/Error");
                    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                    app.UseHsts();
                }

                app.UseHttpsRedirection();
                app.UseStaticFiles();

                // Bundling and minification in .NET Core works differently,
                // see https://docs.microsoft.com/en-us/aspnet/core/client-side/bundling-and-minification

                app.UseRouting();

                app.UseAuthorization();

                // Register routes (previously in RouteConfig)
                app.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                // Register area routes
                app.MapControllerRoute(
                    name: "areas",
                    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

                // Configure global exception handling with logging
                app.Use(async (context, next) =>
                {
                    try
                    {
                        await next();
                    }
                    catch (Exception ex)
                    {
                        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
                        logger.LogError(ex, "Unhandled exception occurred");
                        throw;
                    }
                });

                app.Run();
            }
    }

    public class ConfigurationManager
    {
        public static IConfiguration Configuration { get; set; }
    }

    public class AppSettings
    {
        public string Environment { get; set; }
        public string ServicesAuthentication { get; set; }
        public string ServicesDatabase { get; set; }
        public string ServicesFileService { get; set; }
        public string ServicesImageValidation { get; set; }
        public string ServicesLogging { get; set; }
    }
}