using log4net;
using OgrenciPortalApi.Attributes;
using System;
using System.Data.Entity;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Shared.Enums;

namespace OgrenciPortalApi.Controllers
{
    [JwtAuth]
    [RoutePrefix("api/dashboard")]
    public class DashboardController : BaseApiController
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(DashboardController));

        /// <summary>
        /// Kullanıcının rolüne göre ilgili dashboard verilerini getirir.
        /// </summary>
        /// <returns>Kullanıcı rolüne uygun dashboard verilerini içeren bir HTTP yanıtı döner.</returns>
        [HttpGet]
        [Route("data")]
        [ResponseType(typeof(object))]
        public async Task<IHttpActionResult> GetDashboardData()
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            if (claimsIdentity == null)
            {
                return StatusCode(HttpStatusCode.Unauthorized);
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

                return Ok(payload);
            }
            catch (Exception ex)
            {
                Logger.Error("Dashboard verileri alınırken bir hata oluştu.", ex);
                return InternalServerError(new Exception("Dashboard verileri alınırken bir sunucu hatası oluştu."));
            }
        }

        private async Task<object> GetAdminDashboardDataAsync()
        {
            var userCount = await _db.Users.CountAsync(u => !u.IsDeleted);
            var courseCount = await _db.Courses.CountAsync(c => !c.IsDeleted);
            var departmentCount = await _db.Departments.CountAsync(d => !d.IsDeleted);
            var pendingCount = await _db.StudentCourses.CountAsync(sc =>
                sc.ApprovalStatus == (int)ApprovalStatus.Bekliyor && !sc.IsDeleted);

            return new
            {
                userCount,
                courseCount,
                departmentCount,
                pendingCount
            };
        }

        private async Task<object> GetAdvisorDashboardDataAsync(Guid advisorId)
        {
            var advisedStudentCount = await _db.Users.CountAsync(u => u.AdvisorId == advisorId && !u.IsDeleted);
            var pendingApprovalCount = await _db.StudentCourses
                .CountAsync(sc => sc.Users.AdvisorId == advisorId &&
                                  sc.ApprovalStatus == (int)ApprovalStatus.Bekliyor && !sc.IsDeleted);
            var approvedCount = await _db.StudentCourses
                .CountAsync(sc => sc.Users.AdvisorId == advisorId &&
                                  sc.ApprovalStatus == (int)ApprovalStatus.Onaylandı && !sc.IsDeleted);

            return new
            {
                advisedStudentCount,
                pendingApprovalCount,
                approvedCount
            };
        }

        private async Task<object> GetStudentDashboardDataAsync(Guid studentId)
        {
            var enrolledCourseCount = await _db.StudentCourses
                .CountAsync(sc => sc.StudentId == studentId && !sc.IsDeleted);
            var pendingCourseCount = await _db.StudentCourses
                .CountAsync(sc => sc.StudentId == studentId &&
                                  sc.ApprovalStatus == (int)ApprovalStatus.Bekliyor && !sc.IsDeleted);
            var approvedCourseCount = await _db.StudentCourses
                .CountAsync(sc => sc.StudentId == studentId &&
                                  sc.ApprovalStatus == (int)ApprovalStatus.Onaylandı && !sc.IsDeleted);

            return new
            {
                enrolledCourseCount,
                pendingCourseCount,
                approvedCourseCount
            };
        }
    }
}
