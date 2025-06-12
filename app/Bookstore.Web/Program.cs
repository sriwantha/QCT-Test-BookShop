
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

    namespace Bookstore
    {
        public class Program
        {
            public static void Main(string[] args)
            {
                var builder = WebApplication.CreateBuilder(args);
                
                // Configure database connection string
                builder.Configuration["ConnectionStrings:BookstoreDatabaseConnection"] =
                    "Server=(localdb)\\MSSQLLocalDB;Initial Catalog=BookStoreClassic;MultipleActiveResultSets=true;Integrated Security=SSPI;";

                // Store configuration in static ConfigurationManager
                ConfigurationManager.Configuration = builder.Configuration;

                // Configure app settings from Web.config
                builder.Configuration["Environment"] = "Development";
                builder.Configuration["Services:Authentication"] = "local";
                builder.Configuration["Services:Database"] = "local";
                builder.Configuration["Services:FileService"] = "local";
                builder.Configuration["Services:ImageValidationService"] = "local";
                builder.Configuration["Services:LoggingService"] = "local";

                // Authentication settings
                builder.Configuration["Authentication:Cognito:LocalClientId"] =
                    "[Retrieved from AWS Systems Manager Parameter Store when Services/Authentication == 'aws']";
                builder.Configuration["Authentication:Cognito:AppRunnerClientId"] =
                    "[Retrieved from AWS Systems Manager Parameter Store when Services/Authentication == 'aws']";
                builder.Configuration["Authentication:Cognito:MetadataAddress"] =
                    "[Retrieved from AWS Systems Manager Parameter Store when Services/Authentication == 'aws']";
                builder.Configuration["Authentication:Cognito:CognitoDomain"] =
                    "[Retrieved from AWS Systems Manager Parameter Store when Services/Authentication == 'aws']";

                // File service settings
                builder.Configuration["Files:BucketName"] =
                    "[Retrieved from AWS Systems Manager Parameter Store when Services/FileService == 'aws']";
                builder.Configuration["Files:CloudFrontDomain"] =
                    "[Retrieved from AWS Systems Manager Parameter Store when Services/FileService == 'aws']";

                // Add services to the container (formerly ConfigureServices)
                builder.Services.AddControllersWithViews()
                    .AddRazorPagesOptions(options =>
                    {
                        // Client validation settings from Web.config
                        // ClientValidationEnabled was set to true in Web.config
                    });

                // Add MVC features
                builder.Services.AddRazorPages();

                var app = builder.Build();

                // Register logging
                var loggerFactory = app.Services.GetRequiredService<ILoggerFactory>();
                var logger = loggerFactory.CreateLogger<Program>();
                
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
                
                app.UseRouting();

                // Register areas
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllerRoute(
                        name: "areas",
                        pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
                    );
                });

                app.UseAuthorization();
                
                app.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                // Error handling
                app.Use(async (context, next) =>
                {
                    try
                    {
                        await next();
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "An unhandled exception occurred");
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
    }