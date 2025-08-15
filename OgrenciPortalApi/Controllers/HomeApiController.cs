using System.Web.Mvc;

namespace OgrenciPortalApi.Controllers
{
    public class HomeApiController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            return View();
        }
    }
}
