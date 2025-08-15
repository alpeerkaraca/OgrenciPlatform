using log4net;
using Microsoft.IdentityModel.Tokens;
using OgrenciPortali.Utils;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace OgrenciPortali
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(MvcApplication));

        protected void Application_Start()
        {
            DotNetEnv.Env.Load(Path.Combine(Server.MapPath("~/"), ""));
            AppSettings.Load();
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimTypes.NameIdentifier;
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
            try
            {
                string token = null;
                var authHeader = HttpContext.Current.Request.Headers["Authorization"];
                if (authHeader != null && authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                {
                    token = authHeader.Substring("Bearer ".Length).Trim();
                }

                if (string.IsNullOrEmpty(token))
                {
                    return;
                }

                var tokenHandler = new JwtSecurityTokenHandler();
                var key = AppSettings.JwtMasterKey;
                var symmetricKey = Encoding.UTF8.GetBytes(key);

                var validationParameters = new TokenValidationParameters()
                {
                    RequireExpirationTime = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = AppSettings.JwtIssuer,
                    ValidAudience = AppSettings.JwtAudience,
                    IssuerSigningKey = new SymmetricSecurityKey(symmetricKey),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                SecurityToken validatedToken;
                var principal = tokenHandler.ValidateToken(token, validationParameters, out validatedToken);

                HttpContext.Current.User = principal;
                Thread.CurrentPrincipal = principal;
            }
            catch (SecurityTokenExpiredException ex)
            {
                Logger.Error("Token Süresi Dolmuþ (Global.asax): ", ex);
            }
            catch (SecurityTokenException ex)
            {
                Logger.Error("Token Doðrulanýrken Hata (Global.asax): ", ex);
            }
            catch (Exception ex)
            {
                Logger.Error("Authentication Request Hatasý (Global.asax): ", ex);
            }
        }
    }
}