using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Builder;

namespace Bookstore.Web.Areas
{
    public static class AdminAreaRegistration
    {
        public static string AreaName => "Admin";

        public static void RegisterAdminArea(this IApplicationBuilder app)
        {
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapAreaControllerRoute(
                    name: "Admin_default",
                    areaName: AreaName,
                    pattern: "Admin/{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}