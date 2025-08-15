using System.Net;
using System.Net.Http;
using System.Threading; // Eklenecek
using System.Web; // Eklenecek
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using OgrenciPortalApi.Utils;

namespace OgrenciPortalApi.Attributes
{
    public class JwtAuth : AuthorizationFilterAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            if (actionContext.Request.Headers.Authorization == null ||
                string.IsNullOrEmpty(actionContext.Request.Headers.Authorization.Parameter))
            {
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized, "Yetkilendirme token'ı bulunamadı.");
                return;
            }

            var tokenKey = actionContext.Request.Headers.Authorization.Parameter;
            var principal = TokenManager.GetPrincipal(token: tokenKey);

            if (principal == null)
            {
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized, "Geçersiz token.");
            }
            else
            {
                Thread.CurrentPrincipal = principal;
                if (HttpContext.Current != null)
                {
                    HttpContext.Current.User = principal;
                }
            }
        }
    }
}