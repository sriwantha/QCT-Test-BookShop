using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Owin;


namespace Bookstore.Web.Helpers
{
    public static class OwinRequestExtensions
    {
        public static string GetReturnUrl(this IOwinContext context)
        {
            return $"{context.Request.Scheme}://{context.Request.Host}/signin-oidc";
        }
    }
}