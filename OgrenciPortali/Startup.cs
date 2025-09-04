using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using Newtonsoft.Json;
using OgrenciPortali.Utils;
using Owin;
using Shared.DTO;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Web;
using System.Web.UI;
using SameSiteMode = System.Web.SameSiteMode;

[assembly: OwinStartup(typeof(OgrenciPortali.Startup))]

namespace OgrenciPortali
{
    public class Startup
    {
        private static readonly string clientId = AppSettings.ClientId;
        private static readonly string tenant = AppSettings.TenantId;
        private static readonly string authority = $"https://login.microsoftonline.com/{tenant}/v2.0";

        public void Configuration(IAppBuilder app)
        {
            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);
            app.UseCookieAuthentication(new CookieAuthenticationOptions());

            app.UseOpenIdConnectAuthentication(
                new OpenIdConnectAuthenticationOptions
                {
                    ClientId = clientId,
                    Authority = authority,
                    RedirectUri = "https://localhost:3000",
                    PostLogoutRedirectUri = "https://localhost:3000",
                    Scope = "openid profile email",
                    Notifications = new OpenIdConnectAuthenticationNotifications
                    {
                        SecurityTokenValidated = async context =>
                        {
                            var identity = context.AuthenticationTicket.Identity;

                            var email = identity.FindFirst("email")?.Value;
                            var name = identity.FindFirst("name")?.Value;

                            var ssoReq = new SsoLoginRequestDTO
                            {
                                Email = email,
                                Name = name,
                            };

                            using (var httpClient = new HttpClient())
                            {
                                var apiBaseUrl =
                                    AppSettings.ApiBaseAddress; // Bu ayarın Web.config'de olduğundan emin olun.

                                // 1. DTO nesnesini manuel olarak JSON string'ine çeviriyoruz.
                                var jsonPayload = JsonConvert.SerializeObject(ssoReq);

                                // 2. İçeriği, doğru format ve karakter kodlamasıyla oluşturuyoruz.
                                var httpContent = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                                // 3. İsteği PostAsync metodu ile gönderiyoruz.
                                var response =
                                    await httpClient.PostAsync($"{apiBaseUrl}/api/auth/sso-login", httpContent);

                                if (response.IsSuccessStatusCode)
                                {
                                    // API'den gelen { "Token": "..." } formatındaki yanıtı oku.
                                    var responseBody = await response.Content.ReadAsStringAsync();

                                    // Yanıtı deserialize et. LoginSuccessResponse sınıfını daha önce oluşturmuştuk.
                                    var result = JsonConvert.DeserializeObject<LoginSuccessResponse>(responseBody);
                                    var accessToken = result.Token;

                                    if (!string.IsNullOrEmpty(accessToken))
                                    {
                                        var handler = new JwtSecurityTokenHandler();
                                        var token = handler.ReadJwtToken(accessToken);

                                        var appIdentity = new ClaimsIdentity(token.Claims,
                                            CookieAuthenticationDefaults.AuthenticationType);
                                        var idpClaim = identity.FindFirst("idp");
                                        if (idpClaim != null)
                                            appIdentity.AddClaim(idpClaim);
                                        var owinContext = HttpContext.Current.GetOwinContext();
                                        owinContext.Authentication.SignOut(CookieAuthenticationDefaults.AuthenticationType);
                                        owinContext.Authentication.SignIn(new AuthenticationProperties { IsPersistent = true }, appIdentity);
                                        // Access Token'ı içeren AuthToken cookie'sini oluştur.
                                        var cookie = new HttpCookie("AuthToken", accessToken)
                                        {
                                            HttpOnly = true,
                                            Secure = HttpContext.Current.Request.IsSecureConnection,
                                            Expires = DateTime.Now.AddMinutes(15),
                                            Path = "/",
                                            SameSite = SameSiteMode.Strict
                                        };
                                        HttpContext.Current.Response.Cookies.Add(cookie);
                                    }
                                }
                            }
                        }
                    }
                });
        }
    }
}