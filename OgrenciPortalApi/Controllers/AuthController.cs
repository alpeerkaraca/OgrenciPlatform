using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;
using log4net;
using Newtonsoft.Json;
using OgrenciPortalApi.Models;
using OgrenciPortalApi.Utils;
using Shared.DTO;
using Shared.Enums;

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

        [AllowAnonymous]
        [HttpPost]
        [Route("sso-login")]
        public async Task<HttpResponseMessage> SsoLogin([FromBody] SsoLoginRequestDTO reqDto)
        {
            try
            {
                string name, surname;
                var tuple = SsoLoginRequestDTO.ParseNameCompatible(reqDto.Name);
                var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == reqDto.Email);
                if (user == null)
                {
                    user = new Users
                    {
                        UserId = Guid.NewGuid(),
                        Name = tuple.Item1,
                        Surname = tuple.Item2,
                        Email = reqDto.Email,
                        Role = (int)Roles.Admin,
                        IsActive = true,
                        IsFirstLogin = true,
                        Password = BCrypt.Net.BCrypt.HashPassword(Guid.NewGuid().ToString()),
                        CreatedAt = DateTime.Now,
                        CreatedBy = "Single Sign On",
                        UpdatedAt = DateTime.Now,
                        UpdatedBy = "Single Sign On"
                    };
                    _db.Users.Add(user);
                }

                var claims = TokenManager.GetClaimsFromUser(user);
                var accessToken = TokenManager.GenerateAccessToken(claims);
                var refreshToken = TokenManager.GenerateRefreshToken();
                user.RefreshToken = refreshToken;
                user.RefreshTokenExpTime = DateTime.UtcNow.AddDays(Int32.Parse(AppSettings.RefreshTokenExpDays));
                await _db.SaveChangesAsync();

                var cookie = new CookieHeaderValue("RefreshToken", refreshToken)
                {
                    Domain = Request.RequestUri.Host,
                    Expires = DateTime.UtcNow.AddDays(Convert.ToInt32(AppSettings.RefreshTokenExpDays)),
                    Path = "/",
                    HttpOnly = true,
                    Secure = true
                };
                var response = Request.CreateResponse(HttpStatusCode.OK,
                    new { Message = "Giriş Başarılı", Token = accessToken });
                response.Headers.AddCookies(new[] { cookie });

                return response;
            }
            catch (Exception ex)
            {
                Logger.Error("SSO Girişi yapılırken hata oluştu.", ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError,
                    new { Message = "SSO Girişi yapılırken bir sunucu hatası oluştu." });
            }
        }
    }
}