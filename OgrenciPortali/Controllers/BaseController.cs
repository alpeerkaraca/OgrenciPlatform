using System.Net.Http.Headers;
using System.Security.Claims;
using System.Web.Mvc;

namespace OgrenciPortali.Controllers
{
    public abstract class BaseController : Controller
    {
        protected readonly string ApiBaseAddress = Utils.AppSettings.ApiBaseAddress;

        /// <summary>
        /// Gets the Bearer token from session/TempData
        /// </summary>
        private string GetBearerToken()
        {
            var token = TempData["BearerToken"] as string ?? Session["BearerToken"] as string;

            if (!string.IsNullOrEmpty(token))
            {
                TempData.Keep("BearerToken");
            }

            return token;
        }

        /// <summary>
        /// Sets Authorization header for HttpClient with Bearer token
        /// </summary>
        protected void SetAuthorizationHeader(System.Net.Http.HttpClient client)
        {
            var token = GetBearerToken();
            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        /// <summary>
        /// Gets current user ID from claims
        /// </summary>
        protected string GetCurrentUserId()
        {
            var currentUser = User.Identity as ClaimsIdentity;
            return currentUser?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }

        /// <summary>
        /// Gets current user role from claims
        /// </summary>
        protected string GetCurrentUserRole()
        {
            var currentUser = User.Identity as ClaimsPrincipal;
            return currentUser?.FindFirst(ClaimTypes.Role)?.Value;
        }
    }
}