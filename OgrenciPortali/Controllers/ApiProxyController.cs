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

        [HttpGet]
        public async Task<ActionResult> Get(string apiUrl)
        {
            if (string.IsNullOrEmpty(apiUrl))
            {
                Response.StatusCode = 400;
                return Json("API URL belirtilmedi.", JsonRequestBehavior.AllowGet);
            }

            var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);
            var response = await _apiClient.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            Response.StatusCode = (int)response.StatusCode;

            return Content(content, "application/json");

        }

        [HttpPost]
        [ValidateJsonAntiForgeryToken]
        [BufferRequestBody]
        public async Task<ActionResult> Post(string apiUrl)
        {
            if (string.IsNullOrEmpty(apiUrl))
            {
                Response.StatusCode = 400;
                Response.TrySkipIisCustomErrors = true;
                return Json(new { message = "API URL belirtilmedi." });
            }

            var body = BufferRequestBodyAttribute.GetBody(this.HttpContext);
            var request = new HttpRequestMessage(new HttpMethod(Request.HttpMethod), apiUrl);

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