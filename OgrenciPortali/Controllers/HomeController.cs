using System.Web.Mvc;
using OgrenciPortali.Attributes;

namespace OgrenciPortali.Controllers
{
    /// <summary>
    /// Ana sayfa ve dashboard işlemlerini yöneten controller sınıfı
    /// </summary>
    public class HomeController : BaseController
    {

        /// <summary>
        /// Ana sayfa görünümünü döndürür
        /// </summary>
        [CustomAuth]
        public ActionResult Index()
        {
            return View();
        }

        

        /// <summary>
        /// Hakkında sayfası
        /// </summary>
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            return View();
        }

        /// <summary>
        /// İletişim sayfası
        /// </summary>
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            return View();
        }

    }
}