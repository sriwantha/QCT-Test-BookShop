using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Bookstore.Web.Areas
{
    public static class AdminAreaRegistration
    {
        public static void RegisterAdminArea(this IApplicationBuilder app)
        {
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapAreaControllerRoute(
                    name: "Admin_default",
                    areaName: "Admin",
                    pattern: "Admin/{controller=Home}/{action=Index}/{id?}",
                    defaults: new { area = "Admin" }
                );
            });
        }
    }
}