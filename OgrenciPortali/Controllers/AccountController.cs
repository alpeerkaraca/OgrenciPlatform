using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using OgrenciPortali.Utils;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using log4net;

namespace OgrenciPortali.Controllers
{
    public class AccountController : BaseController
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(AccountController));

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
        public ActionResult SignOut()
        {
            try
            {
                HttpContext.GetOwinContext().Authentication.SignOut(
                    CookieAuthenticationDefaults.AuthenticationType,
                    OpenIdConnectAuthenticationDefaults.AuthenticationType);
            }
            catch (Exception ex)
            {
                Logger.Error("Owin SignOut sırasında hata oluştu.", ex);
            }
            finally
            {
                if (Request.Cookies["AuthToken"] != null)
                {
                    var authTokenCookie = new HttpCookie("AuthToken", "")
                    {
                        Expires = DateTime.Now.AddDays(-1),
                        HttpOnly = true, // Güvenlik!
                        Path = "/"
                    };
                    Response.Cookies.Set(authTokenCookie);
                }

                if (Request.Cookies["RefreshToken"] != null)
                {
                    var refreshTokenCookie = new HttpCookie("RefreshToken", "")
                    {
                        Expires = DateTime.Now.AddDays(-1),
                        HttpOnly = true, // Güvenlik!
                        Path = "/"
                    };
                    Response.Cookies.Set(refreshTokenCookie);
                }

                // 3. Session'ı temizle
                Session.Clear();
                Session.Abandon();
            }

            // 4. Kullanıcıyı giriş sayfasına yönlendir
            return RedirectToAction("Login", "User");
        }
    }
}