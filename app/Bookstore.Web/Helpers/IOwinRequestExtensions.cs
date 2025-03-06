using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Owin;

namespace Bookstore.Web.Helpers
{
    public static class OwinRequestExtensions
    {
        public static string GetReturnUrl(this IOwinRequest request)
        {
            var httpContext = request.Get<HttpContext>("owin.HttpContext");
            return $"{httpContext.Request.Scheme}://{httpContext.Request.Host}/signin-oidc";
        }
    }
}