using System;
using System.Data.Entity;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using log4net;
using OgrenciPortalApi.Utils;

namespace OgrenciPortalApi.Controllers
{
    [RoutePrefix("api/auth")]
    public class AuthController : BaseApiController
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(UserController));

        [HttpPost]
        [AllowAnonymous]
        [Route("refresh-token")]
        public async Task<IHttpActionResult> RefreshToken()
        {
            try
            {
                var reqCookies = Request.Headers.GetCookies("RefreshToken").FirstOrDefault();
                if (reqCookies == null)
                    return Unauthorized();

                var refreshToken = reqCookies["RefreshToken"]?.Value;
                if (string.IsNullOrEmpty(refreshToken))
                    return Unauthorized();

                var user = await _db.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);
                if (user == null || user.RefreshTokenExpTime < DateTime.UtcNow)
                {
                    return Unauthorized();
                }

                var claims = TokenManager.GetClaimsFromUser(user);
                var newAccessToken = TokenManager.GenerateAccessToken(claims);
                var newRefreshToken = TokenManager.GenerateRefreshToken();

                user.RefreshToken = newRefreshToken;
                user.RefreshTokenExpTime = DateTime.UtcNow.AddDays(Convert.ToInt32(AppSettings.RefreshTokenExpDays));
                await _db.SaveChangesAsync();

                return Ok(new { AccessToken = newAccessToken });
            }
            catch (Exception ex)
            {
                Logger.Error("Erişim Tokeni Güncellenirken Hata: ", ex);
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Authorize]
        [Route("logout")]
        public async Task<IHttpActionResult> LogoutUser()
        {
            var requestCookies = Request.Headers.GetCookies("RefreshToken").FirstOrDefault();
            if (requestCookies == null)
                return Unauthorized();

            var refreshToken = requestCookies["RefreshToken"].Value;
            if (string.IsNullOrEmpty(refreshToken))
                return Unauthorized();

            var user = await _db.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);
            if (user == null)
                return Unauthorized();

            user.RefreshToken = null;
            user.RefreshTokenExpTime = null;
            await _db.SaveChangesAsync();
            return (Ok());
        }
    }
}