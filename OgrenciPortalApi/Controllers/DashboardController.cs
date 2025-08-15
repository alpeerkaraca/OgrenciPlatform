using OgrenciPortalApi.Attributes;
using OgrenciPortalApi.Models;
using OgrenciPortali.Models;
using System;
using System.Data.Entity;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;

namespace OgrenciPortalApi.Controllers
{
    [RoutePrefix("api/dashboard")]
    public class DashboardController : ApiController
    {
        private readonly ogrenci_portalEntities db = new ogrenci_portalEntities();

        [HttpGet]
        [JwtAuth]
        [Route("data")]
        public async Task<IHttpActionResult> GetDashboardDataAsync()
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            if (claimsIdentity == null || !claimsIdentity.IsAuthenticated)
            {
                return Content(HttpStatusCode.Unauthorized,
                    new { success = false, message = "Geçerli bir oturum bulunamadı." });
            }

            try
            {
                var userIdClaim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                var userRoleClaim = claimsIdentity.FindFirst("user_role");

                if (userIdClaim == null || string.IsNullOrEmpty(userIdClaim.Value) ||
                    userRoleClaim == null || string.IsNullOrEmpty(userRoleClaim.Value))
                {
                    return BadRequest("Token bilgileri (kullanıcı ID veya rol) eksik ya da geçersiz.");
                }

                if (!Guid.TryParse(userIdClaim.Value, out Guid currentUserId))
                {
                    return BadRequest("Kullanıcı ID formatı geçersiz.");
                }

                string currentUserRole = userRoleClaim.Value;
                object payload;

                switch (currentUserRole)
                {
                    case "1": // Admin
                        payload = await GetAdminDashboardDataAsync();
                        break;
                    case "2": // Danışman
                        payload = await GetAdvisorDashboardDataAsync(currentUserId);
                        break;
                    case "3": // Öğrenci
                        payload = await GetStudentDashboardDataAsync(currentUserId);
                        break;
                    default:
                        return BadRequest("Bilinmeyen kullanıcı rolü.");
                }

                return Ok(new { success = true, data = payload });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Admin için dashboard verilerini asenkron olarak getirir.
        /// </summary>
        private async Task<object> GetAdminDashboardDataAsync()
        {
            // Sorguları sıralı olarak bekliyoruz (await).
            var userCount = await db.Users.CountAsync(u => !u.IsDeleted);
            var courseCount = await db.Courses.CountAsync(c => !c.IsDeleted);
            var departmentCount = await db.Departments.CountAsync(d => !d.IsDeleted);
            var pendingCount = await db.StudentCourses.CountAsync(sc =>
                sc.ApprovalStatus == (int)ApprovalStatus.Bekliyor && !sc.IsDeleted);

            return new
            {
                userCount,
                courseCount,
                departmentCount,
                pendingCount
            };
        }

        /// <summary>
        /// Danışman için dashboard verilerini asenkron olarak getirir.
        /// </summary>
        private async Task<object> GetAdvisorDashboardDataAsync(Guid advisorId)
        {
            var advisedStudentCount = await db.Users.CountAsync(u => u.AdvisorId == advisorId && !u.IsDeleted);

            var pendingApprovalCount = await db.StudentCourses
                .CountAsync(sc => sc.Users.AdvisorId == advisorId &&
                                  sc.ApprovalStatus == (int)ApprovalStatus.Bekliyor && !sc.IsDeleted);

            var approvedCount = await db.StudentCourses
                .CountAsync(sc => sc.Users.AdvisorId == advisorId &&
                                  sc.ApprovalStatus == (int)ApprovalStatus.Onaylandı && !sc.IsDeleted);

            return new
            {
                advisedStudentCount,
                pendingApprovalCount,
                approvedCount
            };
        }

        /// <summary>
        /// Öğrenci için dashboard verilerini asenkron olarak getirir.
        /// </summary>
        private async Task<object> GetStudentDashboardDataAsync(Guid studentId)
        {
            var enrolledCourseCount = await db.StudentCourses
                .CountAsync(sc => sc.StudentId == studentId && !sc.IsDeleted);

            var pendingCourseCount = await db.StudentCourses
                .CountAsync(sc => sc.StudentId == studentId &&
                                  sc.ApprovalStatus == (int)ApprovalStatus.Bekliyor && !sc.IsDeleted);

            var approvedCourseCount = await db.StudentCourses
                .CountAsync(sc => sc.StudentId == studentId &&
                                  sc.ApprovalStatus == (int)ApprovalStatus.Onaylandı && !sc.IsDeleted);

            return new
            {
                enrolledCourseCount,
                pendingCourseCount,
                approvedCourseCount
            };
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}