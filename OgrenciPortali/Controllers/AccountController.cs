using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using OgrenciPortali.Utils;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace OgrenciPortali.Controllers
{
    public class AccountController : Controller
    {
        /// <summary>
        /// Kicks off the SSO sign-in process by redirecting the user to Microsoft.
        /// </summary>
        public void SignIn()
        {
            if (!Request.IsAuthenticated)
            {
                HttpContext.GetOwinContext().Authentication.Challenge(
                    new AuthenticationProperties { RedirectUri = "/" },
                    OpenIdConnectAuthenticationDefaults.AuthenticationType);
            }
        }

        /// <summary>
        /// Performs a complete sign-out from all sessions.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult> SignOut()
        {

            var apiClient = new ApiClient();
            var logoutRequest = new HttpRequestMessage(HttpMethod.Post, "api/auth/logout");
            await apiClient.SendAsync(logoutRequest);

            if (Request.Cookies["AuthToken"] != null)
            {
                var authTokenCookie = new HttpCookie("AuthToken")
                {
                    Expires = DateTime.Now.AddDays(-1),
                    Path = "/"
                };
                Response.Cookies.Set(authTokenCookie);
            }
            if (Request.Cookies["RefreshToken"] != null)
            {
                var refreshTokenCookie = new HttpCookie("RefreshToken")
                {
                    Expires = DateTime.Now.AddDays(-1),
                    Path = "/"
                };
                Response.Cookies.Set(refreshTokenCookie);
            }


            HttpContext.GetOwinContext().Authentication.SignOut(
                 CookieAuthenticationDefaults.AuthenticationType,
                 OpenIdConnectAuthenticationDefaults.AuthenticationType);

            return RedirectToAction("Login", "User");
        }
    }
}