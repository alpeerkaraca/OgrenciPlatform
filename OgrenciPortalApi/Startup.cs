using Microsoft.IdentityModel.Tokens;
using Microsoft.Owin;
using Microsoft.Owin.Security.Jwt;
using OgrenciPortalApi.Utils;
using Owin;
using System;
using System.Text;
using System.Web.Http;
using Microsoft.Owin.Security;

[assembly: OwinStartup(typeof(OgrenciPortalApi.Startup))]

namespace OgrenciPortalApi
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
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