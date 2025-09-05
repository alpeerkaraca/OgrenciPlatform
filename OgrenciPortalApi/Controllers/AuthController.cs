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

        [HttpPost]
        [AllowAnonymous]
        [Route("sso-login")]
        public async Task<HttpResponseMessage> SsoLogin([FromBody] SsoLoginRequestDTO reqDto)
        {
            try
            {
                if (reqDto == null || string.IsNullOrEmpty(reqDto.Email))
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { Message = "E-posta adresi gerekli." });
                }

                // Senin mevcut mantığını koruyoruz: Kullanıcıyı bul veya oluştur.
                var user = await _db.Users.Include(u => u.Departments)
                    .FirstOrDefaultAsync(u => u.Email == reqDto.Email && !u.IsDeleted);

                // Eğer kullanıcı sistemde yoksa, senin iş kurallarına göre yeni bir kullanıcı oluşturuyoruz.
                if (user == null)
                {
                    // İsim ve soyismi ayıran kendi metodunu kullanıyoruz.
                    var tuple = SsoLoginRequestDTO.ParseNameCompatible(reqDto.Name);
                    user = new Users
                    {
                        UserId = Guid.NewGuid(),
                        Name = tuple.Item1,
                        Surname = tuple.Item2,
                        Email = reqDto.Email,
                        Role = (int)Roles.Öğrenci, // YENİ KULLANICI İÇİN VARSAYILAN ROL! Burayı 'Admin' yerine 'Öğrenci' olarak değiştirdim, daha güvenli.
                        IsActive = true,
                        IsFirstLogin = true,
                        Password = BCrypt.Net.BCrypt.HashPassword(Guid.NewGuid().ToString()), // Rastgele şifre oluşturma.
                        CreatedAt = DateTime.Now,
                        CreatedBy = "Single Sign On",
                        UpdatedAt = DateTime.Now,
                        UpdatedBy = "Single Sign On"
                    };
                    _db.Users.Add(user);
                }

                // Mevcut kullanıcının aktif olduğundan emin ol.
                user.IsActive = true;

                // Kullanıcı için kendi token'larımızı oluşturalım.
                var claims = TokenManager.GetClaimsFromUser(user);
                var accessToken = TokenManager.GenerateAccessToken(claims);
                var refreshToken = TokenManager.GenerateRefreshToken(); // URL-safe token ürettiğimizden eminiz.

                user.RefreshToken = refreshToken;
                user.RefreshTokenExpTime = DateTime.UtcNow.AddDays(Convert.ToInt32(AppSettings.RefreshTokenExpDays));
                await _db.SaveChangesAsync();

                // Startup.cs'in okuyabilmesi için token'ları body'de dönüyoruz.
                var responseBody = new LoginSuccessResponse(){RefreshToken = refreshToken, AccessToken = accessToken, Message = "Giriş başarılı."};
                var response = Request.CreateResponse(HttpStatusCode.OK, responseBody);

                // --- EN İYİ UYGULAMA: Cookie'leri de burada oluşturuyoruz ---
                // AccessToken için HttpOnly cookie (Güvenlik!)
                var accessTokenCookie = new CookieHeaderValue("AuthToken", accessToken)
                {
                    Expires = DateTimeOffset.UtcNow.AddMinutes(Convert.ToInt32(AppSettings.AccessTokenExpMins)),
                    Path = "/",
                    HttpOnly = true,
                    Secure = true,
                    // SameSite = SameSiteMode.Lax // Tarayıcı uyumluluğu için .NET Framework'te bu şekilde eklenemeyebilir. Web.config'den ayarlanması daha iyidir.
                };

                // RefreshToken için HttpOnly cookie (Güvenlik!)
                var refreshTokenCookie = new CookieHeaderValue("RefreshToken", refreshToken)
                {
                    Expires = DateTimeOffset.UtcNow.AddDays(Convert.ToInt32(AppSettings.RefreshTokenExpDays)),
                    Path = "/",
                    HttpOnly = true,
                    Secure = true
                };

                response.Headers.AddCookies(new[] { accessTokenCookie, refreshTokenCookie });

                return response;
            }
            catch (Exception ex)
            {
                Logger.Error("SSO girişi sırasında hata oluştu.", ex);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "SSO girişi sırasında bir sunucu hatası oluştu.");
            }
        }
    }
}