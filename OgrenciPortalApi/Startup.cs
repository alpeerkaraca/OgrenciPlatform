using Hangfire;
using Hangfire.SqlServer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Jwt;
using OgrenciPortalApi.Utils;
using Owin;
using System;
using System.Data.SqlClient;
using System.Text;
using System.Web.Http;

[assembly: OwinStartup(typeof(OgrenciPortalApi.Startup))]

namespace OgrenciPortalApi
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var builder = new SqlConnectionStringBuilder
            {
                DataSource = AppSettings.DbServer,
                InitialCatalog = AppSettings.DbName,
                UserID = AppSettings.DbUser,
                Password = AppSettings.DbPass,
                TrustServerCertificate = true,
                MultipleActiveResultSets = true,
            };

            // TODO: Productionda bunu aktifleştir.
            var dashboardOptions = new DashboardOptions
            {
                Authorization = new[] { new HangfireDashboardAuthorizationFilter() }
            };
            Hangfire.GlobalConfiguration.Configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(builder.ConnectionString, new SqlServerStorageOptions
                {
                    SchemaName = "hangfire"
                });
            // TODO: İşte burada
            //app.UseHangfireDashboard("/hangfire",dashboardOptions);
            app.UseHangfireDashboard("/hangfire");
            app.UseHangfireServer();
            BackgroundJob.Enqueue(() => CheckUserData.CacheUserAddressesAsync());
            RecurringJob.AddOrUpdate(
                "cache-user-addresses",
                () => CheckUserData.CacheUserAddressesAsync(),
                "*/15 * * * *"
            );


            ConfigureAuth(app);
            HttpConfiguration config = new HttpConfiguration();

            WebApiConfig.Register(config);
            app.UseWebApi(config);
        }

        public void ConfigureAuth(IAppBuilder app)
        {
            var issuer = AppSettings.JwtIssuer;
            var audience = AppSettings.JwtAudience;
            var secret = AppSettings.JwtMasterKey;
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
                ValidateIssuer = true,
                ValidIssuer = issuer,
                ValidateAudience = true,
                ValidAudience = audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
            app.UseJwtBearerAuthentication(new JwtBearerAuthenticationOptions
            {
                AuthenticationMode = AuthenticationMode.Active,
                TokenValidationParameters = tokenValidationParameters
            });
        }
    }
}