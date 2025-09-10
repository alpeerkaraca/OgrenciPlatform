using Microsoft.IdentityModel.Protocols.OpenIdConnect;
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
using System.Threading.Tasks;
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
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                CookieName = ".AspNet." + CookieAuthenticationDefaults.AuthenticationType,
                Provider = new CookieAuthenticationProvider()
                {
                    OnApplyRedirect = context =>
                    {
                        bool isAjax = context.Request.Headers["X-Requested-With"] == "XMLHttpRequest" ||
                                      (context.Request.Accept != null &&
                                       context.Request.Accept.Contains("application/json"));
                        if (isAjax)
                        {
                            context.Response.StatusCode = 401;
                            context.Response.Write("");
                        }
                        else
                            context.Response.Redirect(context.RedirectUri);
                    }
                }
            });

            app.UseOpenIdConnectAuthentication(
                new OpenIdConnectAuthenticationOptions
                {
                    ClientId = clientId,
                    Authority = authority,
                    RedirectUri = AppSettings.RedirectUri,
                    PostLogoutRedirectUri = AppSettings.PostLogoutRedirectUri,
                    Scope = "openid profile email",
                    Notifications = new OpenIdConnectAuthenticationNotifications
                    {
                        RedirectToIdentityProvider = notification =>
                        {
                            if (notification.ProtocolMessage.RequestType == OpenIdConnectRequestType.Logout)
                            {
                                var idTokenHint = notification.OwinContext.Authentication.User.FindFirst("id_token");
                                if(idTokenHint != null)
                                {
                                    notification.ProtocolMessage.IdTokenHint = idTokenHint.Value;
                                }
                            }
                            return Task.FromResult(0);
                        },
                        SecurityTokenValidated = async context =>
                        {
                            var ssoIdentity = context.AuthenticationTicket.Identity;
                            var email = ssoIdentity.FindFirst(ClaimTypes.Email)?.Value ??
                                        ssoIdentity.FindFirst("email")?.Value;
                            var name = ssoIdentity.FindFirst(ClaimTypes.Name)?.Value ??
                                       ssoIdentity.FindFirst("name")?.Value;

                            var ssoReq = new SsoLoginRequestDTO { Email = email, Name = name };

                            using (var httpClient = new HttpClient())
                            {
                                var apiBaseUrl = AppSettings.ApiBaseAddress;
                                var jsonPayload = JsonConvert.SerializeObject(ssoReq);
                                var httpContent = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                                var response =
                                    await httpClient.PostAsync($"{apiBaseUrl}/api/auth/sso-login", httpContent);

                                if (response.IsSuccessStatusCode)
                                {
                                    // 1. API'den gelen token'ları içeren body'yi oku
                                    var responseBody = await response.Content.ReadAsStringAsync();
                                    var tokenResult = JsonConvert.DeserializeObject<LoginSuccessResponse>(responseBody);

                                    // 2. Access token'dan gelen claim'lerle kendi kimliğimizi oluşturalım
                                    var handler = new JwtSecurityTokenHandler();
                                    var token = handler.ReadJwtToken(tokenResult.AccessToken);
                                    var appIdentity = new ClaimsIdentity(token.Claims,
                                        CookieAuthenticationDefaults.AuthenticationType);

                                    // SSO ile giriş yapıldığını belirtmek için "idp" claim'ini ekleyelim
                                    var idpClaim = ssoIdentity.FindFirst("idp");
                                    if (idpClaim != null)
                                    {
                                        appIdentity.AddClaim(idpClaim);
                                    }
                                    appIdentity.AddClaim(new Claim("id_token", context.ProtocolMessage.IdToken));

                                    // 3. Owin oturumunu bizim API'mizin kimliği ile tamamen değiştir
                                    context.AuthenticationTicket = new AuthenticationTicket(appIdentity,
                                        context.AuthenticationTicket.Properties);

                                    // --- GÜVENLİ VE DOĞRUDAN YÖNTEM ---
                                    // 4. Cookie'leri API yanıtından okunan token'lar ile burada manuel oluşturalım.
                                    var accessTokenCookie = new HttpCookie("AuthToken", tokenResult.AccessToken)
                                    {
                                        HttpOnly = true,
                                        Secure = HttpContext.Current.Request.IsSecureConnection,
                                        Expires = DateTime.UtcNow.AddMinutes(15),
                                        Path = "/"
                                    };
                                    var refreshTokenCookie = new HttpCookie("RefreshToken", tokenResult.RefreshToken)
                                    {
                                        HttpOnly = true,
                                        Secure = HttpContext.Current.Request.IsSecureConnection,
                                        Expires = DateTime.UtcNow.AddDays(7),
                                        Path = "/"
                                    };

                                    HttpContext.Current.Response.Cookies.Add(accessTokenCookie);
                                    HttpContext.Current.Response.Cookies.Add(refreshTokenCookie);
                                }
                                else
                                {
                                    context.HandleResponse();
                                    context.Response.Redirect("/User/Login?error=sso_failed");
                                }
                            }
                        }
                    }
                });
        }
    }
}