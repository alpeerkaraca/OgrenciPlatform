using System.Net.Http.Headers;
using System.Security.Claims;
using System.Web.Mvc;

namespace OgrenciPortali.Controllers
{
    [NoCache]
    public abstract class BaseController : Controller
    {
        protected readonly string ApiBaseAddress = Utils.AppSettings.ApiBaseAddress;
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