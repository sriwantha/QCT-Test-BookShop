using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Bookstore.Domain.Customers;
using Microsoft.Owin;
using Microsoft.AspNetCore.Http;

namespace Bookstore.Web.Helpers
{
    public class LocalAuthenticationMiddleware : OwinMiddleware
    {
        private const string UserId = "FB6135C7-1464-4A72-B74E-4B63D343DD09";

        private readonly ICustomerService _customerService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LocalAuthenticationMiddleware(OwinMiddleware next, ICustomerService customerService, IHttpContextAccessor httpContextAccessor) : base(next)
        {
            _customerService = customerService;
            _httpContextAccessor = httpContextAccessor;
        }

        public override async Task Invoke(IOwinContext context)
        {
            var httpContext = _httpContextAccessor.HttpContext;

            if (context.Request.Path.Value.StartsWith("/Authentication/Login"))
            {
                CreateClaimsPrincipal(context);

                await SaveCustomerDetailsAsync();

                httpContext.Response.Cookies.Append("LocalAuthentication", "true", new CookieOptions { Expires = DateTime.Now.AddDays(1) });

                context.Response.Redirect("/");
            }
            else if (httpContext.Request.Cookies.ContainsKey("LocalAuthentication"))
            {
                CreateClaimsPrincipal(context);

                await SaveCustomerDetailsAsync();

                await Next.Invoke(context);
            }
            else
            {
                await Next.Invoke(context);
            }
        }

        private void CreateClaimsPrincipal(IOwinContext context)
        {
            var identity = new ClaimsIdentity("Application");

            identity.AddClaim(new Claim(ClaimTypes.Name, "bookstoreuser"));
            identity.AddClaim(new Claim("nameidentifier", UserId));
            identity.AddClaim(new Claim("given_name", "Bookstore"));
            identity.AddClaim(new Claim("family_name", "User"));
            identity.AddClaim(new Claim(ClaimTypes.Role, "Administrators"));

            context.Request.User = new ClaimsPrincipal(identity);
        }

        private async Task SaveCustomerDetailsAsync()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var identity = httpContext.User.Identity as ClaimsIdentity;

            if (identity != null)
            {
                var dto = new CreateOrUpdateCustomerDto(
                    identity.FindFirst("nameidentifier")?.Value,
                    identity.Name,
                    identity.FindFirst("given_name")?.Value,
                    identity.FindFirst("family_name")?.Value);

                await _customerService.CreateOrUpdateCustomerAsync(dto);
            }
        }
    }
}