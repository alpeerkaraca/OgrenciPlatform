using OgrenciPortalApi.Models;
using OgrenciPortalApi.Utils;
using System;
using System.IdentityModel.Claims;
using System.Web.Http;

namespace OgrenciPortalApi.Controllers
{
    /// <summary>
    /// Diğer tüm API controller'lar için temel sınıf.
    /// Veritabanı context'i ve ortak yardımcı metodları içerir.
    /// </summary>
    public class BaseApiController : ApiController
    {
        protected readonly ogrenci_portalEntities _db;

        public BaseApiController()
        {
            _db = new ogrenci_portalEntities();
        }

        /// <summary>
        /// JWT token'dan aktif kullanıcının ID'sini Guid olarak alır.
        /// </summary>
        /// <returns>Aktif kullanıcının Guid tipindeki ID'si.</returns>
        /// <exception cref="InvalidOperationException">Kullanıcı ID'si token'da bulunamazsa veya geçersizse fırlatılır.</exception>
        protected Guid GetActiveUserId()
        {
            var principal = TokenManager.GetPrincipal(Request.Headers.Authorization.Parameter);
            var userIdClaim = principal?.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid userId))
            {
                throw new InvalidOperationException("User ID could not be found or is invalid in the token.");
            }
            return userId;
        }

        /// <summary>
        /// JWT token'dan aktif kullanıcının ID'sini string olarak alır.
        /// </summary>
        /// <returns>Aktif kullanıcının string tipindeki ID'si.</returns>
        /// <exception cref="InvalidOperationException">Kullanıcı ID'si token'da bulunamazsa fırlatılır.</exception>
        protected string GetActiveUserIdString()
        {
            var principal = TokenManager.GetPrincipal(Request.Headers.Authorization.Parameter);
            var userIdClaim = principal?.FindFirst(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userIdClaim?.Value))
            {
                throw new InvalidOperationException("User ID could not be found in the token.");
            }
            return userIdClaim.Value;
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
