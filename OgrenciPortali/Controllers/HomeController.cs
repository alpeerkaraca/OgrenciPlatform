using OgrenciPortali.Attributes;
using OgrenciPortali.Utils;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;
using Shared.DTO;

namespace OgrenciPortali.Controllers
{
    /// <summary>
    /// Ana sayfa ve dashboard işlemlerini yöneten controller sınıfı
    /// </summary>
    public class HomeController : BaseController
    {
        private readonly ApiClient _apiClient;

        public HomeController(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        /// <summary>
        /// Ana sayfa görünümünü döndürür
        /// </summary>
        [CustomAuth]
        public async Task<ActionResult> Index()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "api/student/my-courses");
            var response = await _apiClient.SendAsync(request);
            var dto = new MyCoursesDTO();
            if (response.IsSuccessStatusCode)
            {
                dto = await response.Content.ReadAsAsync<MyCoursesDTO>();
            }

            return View(dto);
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