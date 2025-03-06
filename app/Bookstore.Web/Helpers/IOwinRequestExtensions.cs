using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Owin;

namespace Bookstore.Web.Helpers
{
    public static class OwinRequestExtensions
    {
        public static string GetReturnUrl(this IOwinContext context)
        {
            var request = context.Request;
            return $"{request.Scheme}://{request.Host}/signin-oidc";
        }
    }
}