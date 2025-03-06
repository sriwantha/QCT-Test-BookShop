using System;
using BobsBookstoreClassic.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace Bookstore.Web.Controllers
{
    public class AuthenticationController : Controller
    {
        public ActionResult Login(string redirectUri = null)
        {
            if(string.IsNullOrWhiteSpace(redirectUri)) return RedirectToAction("Index", "Home");

            return Redirect(redirectUri);
        }

        public ActionResult LogOut()
        {
            return BookstoreConfiguration.Get("Services/Authentication") == "aws" ? CognitoSignOut() : LocalSignOut();
        }

        private ActionResult LocalSignOut()
        {
            if (HttpContext.Request.Cookies.TryGetValue("LocalAuthentication", out _))
            {
                HttpContext.Response.Cookies.Append("LocalAuthentication", "", new CookieOptions
                {
                    Expires = DateTime.Now.AddDays(-1)
                });
            }

            return RedirectToAction("Index", "Home");
        }

        private ActionResult CognitoSignOut()
        {
            if (HttpContext.Request.Cookies.TryGetValue(".AspNet.Cookies", out _))
            {
                HttpContext.Response.Cookies.Append(".AspNet.Cookies", "", new CookieOptions
                {
                    Expires = DateTime.Now.AddDays(-1)
                });
            }

            var domain = BookstoreConfiguration.Get("Authentication/Cognito/CognitoDomain");
            var clientId = BookstoreConfiguration.Get("Authentication/Cognito/LocalClientId");
            var logoutUri = $"{Request.Scheme}://{Request.Host}/";

            return Redirect($"{domain}/logout?client_id={clientId}&logout_uri={logoutUri}");
        }
    }
}