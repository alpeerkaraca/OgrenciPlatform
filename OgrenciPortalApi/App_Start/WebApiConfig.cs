using System.Web.Http;
using System.Web.Http.Cors;

namespace OgrenciPortalApi
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            var cors = new EnableCorsAttribute(
                origins: "https://ogrenciportal.alpeerkaraca.site",
                headers: "*",
                methods: "*"
            )
            {
                SupportsCredentials = true
            };

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