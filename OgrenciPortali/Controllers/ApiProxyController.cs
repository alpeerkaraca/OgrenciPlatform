using OgrenciPortali.Attributes;
using OgrenciPortali.Utils;
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace OgrenciPortali.Controllers
{
    public class ApiProxyController : BaseController
    {
        private readonly ApiClient _apiClient;

        public ApiProxyController()
        {
            _apiClient = new ApiClient();
        }

        /// <summary>
        /// GET istekleri için proxy metodu.
        /// </summary>
        /// <param name="apiUrl">Hedef API'nin tam yolu (örn: "api/dashboard/data")</param>
        [HttpGet]
        public async Task<ActionResult> Get(string apiUrl)
        {
            if (string.IsNullOrEmpty(apiUrl))
                return new HttpStatusCodeResult(400, "API URL belirtilmedi.");

            var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);
            var response = await _apiClient.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return Content(content, "application/json");
            }

            return new HttpStatusCodeResult((int)response.StatusCode, content);
        }

        /// <summary>
        /// POST istekleri için proxy metodu.
        /// </summary>
        /// <param name="apiUrl">Hedef API'nin tam yolu</param>
        [HttpPost]
        [ValidateJsonAntiForgeryToken] // Bu attribute hala gerekli
        [BufferRequestBody]
        public async Task<ActionResult> Post(string apiUrl)
        {
            if (string.IsNullOrEmpty(apiUrl))
                return new HttpStatusCodeResult(400, "API URL belirtilmedi.");

            var body = BufferRequestBodyAttribute.GetBody(this.HttpContext);

            var originalMethod = new HttpMethod(Request.HttpMethod);
            var request = new HttpRequestMessage(originalMethod, apiUrl);

            if (!string.IsNullOrEmpty(body))
            {
                request.Content = new StringContent(body, Encoding.UTF8, "application/json");
            }

            var response = await _apiClient.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            Response.StatusCode = (int)response.StatusCode;
            return Content(content, "application/json");
        }
    }
}