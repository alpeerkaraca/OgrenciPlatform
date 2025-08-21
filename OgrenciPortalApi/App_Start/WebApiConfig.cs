using System.Web.Http;
using System.Web.Http.Cors;

namespace OgrenciPortalApi
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes

            var cors = new EnableCorsAttribute(
                origins: "https://localhost:3000",
                headers: "*",
                methods: "*"
            );
            cors.SupportsCredentials = true;

            config.EnableCors(cors);

            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}