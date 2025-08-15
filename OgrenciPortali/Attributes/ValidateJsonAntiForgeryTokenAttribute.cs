using System;
using System.Web.Helpers;
using System.Web.Mvc;

namespace OgrenciPortali.Attributes // Kendi projenizin namespace'i ile değiştirin
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class ValidateJsonAntiForgeryTokenAttribute : FilterAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext == null)
            {
                throw new ArgumentNullException("filterContext");
            }

            var httpContext = filterContext.HttpContext;
            var cookie = httpContext.Request.Cookies[AntiForgeryConfig.CookieName];
            var headerToken = httpContext.Request.Headers["RequestVerificationToken"];

            // Gelen istekte header veya cookie yoksa hata ver
            if (string.IsNullOrEmpty(headerToken))
            {
                throw new HttpAntiForgeryException("RequestVerificationToken başlığı bulunamadı.");
            }

            try
            {
                // Cookie'den gelen token ile header'dan gelen token'ı doğrula
                AntiForgery.Validate(cookie != null ? cookie.Value : null, headerToken);
            }
            catch (HttpAntiForgeryException ex)
            {
                // Doğrulama başarısız olursa, isteği reddet
                throw new HttpAntiForgeryException("Anti-forgery token doğrulanamadı.", ex);
            }
        }
    }
}