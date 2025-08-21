using OgrenciPortalApi;
using Swashbuckle.Application;
using Swashbuckle.Swagger;
using System.Collections.Generic;
using System.Linq; // LINQ kullanýmý için ekleyin
using System.Web.Http;
using System.Web.Http.Description;
using WebActivatorEx;

[assembly: PreApplicationStartMethod(typeof(SwaggerConfig), "Register")]

namespace OgrenciPortalApi
{
    public class SwaggerConfig
    {
        public static void Register()
        {
            var thisAssembly = typeof(SwaggerConfig).Assembly;

            GlobalConfiguration.Configuration
                .EnableSwagger(c =>
                {
                    c.SingleApiVersion("v1", "OgrenciPortalApi");
                    var xmlFile = $"{System.AppDomain.CurrentDomain.BaseDirectory}\\bin\\OgrenciPortalApi.XML";
                    c.IncludeXmlComments(xmlFile);

                    c.ApiKey("Authorization").In("Header").Name("Autharization")
                        .Description(
                            "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"");

                    c.OperationFilter<SecurityRequirementsOperationFilter>();
                })
                .EnableSwaggerUi(c =>
                {
                    c.EnableApiKeySupport("Authorization", "header");
                    c.InjectJavaScript(thisAssembly, "OgrenciPortalApi.SwaggerUI.custom.js");
                });
        }
    }

    public class SecurityRequirementsOperationFilter : IOperationFilter
    {
        public void Apply(Operation operation, SchemaRegistry schemaRegistry, ApiDescription apiDescription)
        {
            if (apiDescription.ActionDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any() ||
                apiDescription.ActionDescriptor.ControllerDescriptor.GetCustomAttributes<AllowAnonymousAttribute>()
                    .Any())
            {
                return;
            }

            if (apiDescription.ActionDescriptor.GetCustomAttributes<AuthorizeAttribute>().Any() ||
                apiDescription.ActionDescriptor.ControllerDescriptor.GetCustomAttributes<AuthorizeAttribute>().Any())
            {
                if (operation.security == null)
                {
                    operation.security = new List<IDictionary<string, IEnumerable<string>>>();
                }

                // Yukarýda `c.ApiKey` içinde tanýmladýðýmýz "Authorization" ismiyle eþleþmeli.
                var securityRequirement = new Dictionary<string, IEnumerable<string>>
                {
                    { "Authorization", new string[0] }
                };

                operation.security.Add(securityRequirement);
            }
        }
    }
}