using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Bookstore.Domain.Customers;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;

namespace Bookstore.Web.Helpers
{
    public class LocalAuthenticationMiddleware : OwinMiddleware
    {
        private const string UserId = "FB6135C7-1464-4A72-B74E-4B63D343DD09";

        private readonly ICustomerService _customerService;

        public LocalAuthenticationMiddleware(OwinMiddleware next, ICustomerService customerService) : base(next)
        {
            _customerService = customerService;
        }

        public override async Task Invoke(IOwinContext context)
        {
            if (context.Request.Path.Value.StartsWith("/Authentication/Login"))
            {
                await CreateClaimsPrincipalAsync(context);

                await SaveCustomerDetailsAsync(context);

                context.Response.Cookies.Append("LocalAuthentication", "true", new Microsoft.Owin.CookieOptions { Expires = DateTime.Now.AddDays(1) });

                context.Response.Redirect("/");
            }
            else if (context.Request.Cookies["LocalAuthentication"] != null)
            {
                await CreateClaimsPrincipalAsync(context);

                await SaveCustomerDetailsAsync(context);

                await Next.Invoke(context);
            }
            else
            {
                await Next.Invoke(context);
            }
        }

        private async Task CreateClaimsPrincipalAsync(IOwinContext context)
        {
            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationType);

            identity.AddClaim(new Claim(ClaimTypes.Name, "bookstoreuser"));
            identity.AddClaim(new Claim("nameidentifier", UserId));
            identity.AddClaim(new Claim("given_name", "Bookstore"));
            identity.AddClaim(new Claim("family_name", "User"));
            identity.AddClaim(new Claim(ClaimTypes.Role, "Administrators"));

            var principal = new ClaimsPrincipal(identity);
            context.Authentication.SignIn(identity);
            context.Request.User = principal;
        }

        private async Task SaveCustomerDetailsAsync(IOwinContext context)
        {
            var identity = context.Authentication.User.Identity as ClaimsIdentity;

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