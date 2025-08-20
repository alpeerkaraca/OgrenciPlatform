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
using Autofac;
using Autofac.Integration.Mvc;

namespace OgrenciPortali
{
    public class MvcApplication : HttpApplication
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(MvcApplication));
        private static TokenValidationParameters tokenValidationParameters;

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
            InitializeTokenValidationParameters();

            var builder = new ContainerBuilder();
            builder.RegisterControllers(typeof(MvcApplication).Assembly);
            builder.RegisterType(typeof(ApiClient)).AsSelf().InstancePerRequest();
            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
            try
            {
                var tokenCookie = HttpContext.Current.Request.Cookies["AuthToken"]; // Bulam�yor NULL geliyor.
                if (tokenCookie == null || string.IsNullOrEmpty(tokenCookie.Value))
                {
                    return;
                }

                var token = tokenCookie.Value;

                var tokenHandler = new JwtSecurityTokenHandler();
                var principal =
                    tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken validatedToken);

                HttpContext.Current.User = principal;
                Thread.CurrentPrincipal = principal;
            }
            catch (SecurityTokenExpiredException ex)
            {
                Logger.Warn("Kullan�c�n�n token s�resi dolmu� (Global.asax). Cookie temizlenecek.", ex);
                var expiredCookie = new HttpCookie("AuthToken", "") { Expires = DateTime.Now.AddDays(-1) };
                HttpContext.Current.Response.Cookies.Add(expiredCookie);
            }
            catch (SecurityTokenException ex)
            {
                Logger.Error("Token do�rulamas� ba�ar�s�z oldu (Global.asax): ", ex);
            }
            catch (Exception ex)
            {
                Logger.Error("Authentication Request s�ras�nda genel hata (Global.asax): ", ex);
            }
        }

        private void InitializeTokenValidationParameters()
        {
            var key = AppSettings.JwtMasterKey;
            var symmetricKey = Encoding.UTF8.GetBytes(key);

            tokenValidationParameters = new TokenValidationParameters()
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
        }
    }
}