using System.Web.Mvc;
using System.Web;

public class NoCacheAttribute : ActionFilterAttribute
{
    public override void OnResultExecuting(ResultExecutingContext filterContext)
    {
        // Tarayıcıya ve proxy'lere yanıtı önbelleğe almamalarını söyle
        filterContext.HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
        filterContext.HttpContext.Response.Cache.SetNoStore();

        // Önbellek geçmişini geçersiz kıl
        filterContext.HttpContext.Response.Cache.SetExpires(System.DateTime.UtcNow.AddYears(-1));
        filterContext.HttpContext.Response.Cache.SetValidUntilExpires(false);
        filterContext.HttpContext.Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);

        // Eski tarayıcılar için ek başlıklar
        filterContext.HttpContext.Response.Headers.Add("Pragma", "no-cache");

        base.OnResultExecuting(filterContext);
    }
}