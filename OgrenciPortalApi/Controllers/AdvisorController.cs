using log4net;
using OgrenciPortalApi.Models;
using OgrenciPortalApi.Utils;
using OgrenciPortali.DTOs;
using OgrenciPortali.Models;
using OgrenciPortali.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IdentityModel.Claims;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace OgrenciPortalApi.Controllers
{
    //[JwtAuth]
    [RoutePrefix("api/advisor")]
    public class AdvisorController : ApiController
    {
        private readonly ogrenci_portalEntities _db = new ogrenci_portalEntities();
        private static readonly ILog Logger = LogManager.GetLogger(typeof(AdvisorController));

        [HttpGet]
        [Route("approvals")]
        public async Task<IHttpActionResult> GetApprovals()
        {
            try
            {
                var advisorId = Guid.Parse(GetActiveUserId());

                var approvalsFromDb = await _db.StudentCourses
                    .Where(sc =>
                        sc.OfferedCourses.TeacherId == advisorId &&
                        sc.ApprovalStatus == (int)ApprovalStatus.Bekliyor)
                    .Select(sc => new ApprovalRequestDto
                    {
                        StudentId = sc.StudentId,
                        OfferedCourseId = sc.OfferedCourseId,
                        StudentNo = sc.Users.StudentNo,
                        StudentName = sc.Users.Name + " " + sc.Users.Surname,
                        CourseCode = sc.OfferedCourses.Courses.CourseCode,
                        CourseName = sc.OfferedCourses.Courses.CourseName,
                        Credits = sc.OfferedCourses.Courses.Credits,
                        SemesterName = sc.OfferedCourses.Semesters.SemesterName,
                        DayOfWeek = ((DaysOfWeek)sc.OfferedCourses.DayOfWeek).ToString(),
                        StartTime = sc.OfferedCourses.StartTime,
                        EndTime = sc.OfferedCourses.EndTime,
                        ApprovalStatus = sc.ApprovalStatus,
                        RequestDate = sc.CreatedAt
                    })
                    .OrderByDescending(x => x.RequestDate)
                    .ToListAsync();

                var dto = new AdvisorApprovalDTO
                {
                    PendingApprovals = approvalsFromDb ?? new List<ApprovalRequestDto>()
                };
                return Ok(dto);
            }
            catch (Exception ex)
            {
                Logger.Error("Error fetching approvals", ex);
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("students")]
        public async Task<IHttpActionResult> GetAdvisedStudents()
        {
            try
            {
                var advisorId = Guid.Parse(GetActiveUserId());

                var students = await _db.Users
                    .Include(u => u.Departments)
                    .Where(u => u.AdvisorId == advisorId && !u.IsDeleted && u.Role == (int)Roles.Öğrenci)
                    .OrderBy(u => u.StudentNo)
                    .Select(u => new StudentInfoDTO()
                    {
                        StudentId = u.UserId,
                        StudentFullName = u.Name + " " + u.Surname,
                        StudentDepartmentName = u.Departments.Name,
                        StudentEmail = u.Email,
                        StudentCreateDate = u.CreatedAt,
                        StudentNo = u.StudentNo
                    })
                    .ToListAsync();
                var dto = new AdvisedStudentsDTO
                {
                    AdvisedStudents = students
                };

                return Ok(dto);
            }
            catch (Exception ex)
            {
                Logger.Error("Error fetching advised students", ex);
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("student/{id:guid}")]
        public async Task<IHttpActionResult> GetStudent(Guid id)
        {
            try
            {
                var advisorId = Guid.Parse(GetActiveUserId());
                var student = await _db.Users.Include(s => s.Departments).FirstOrDefaultAsync(u =>
                    u.UserId == id && u.AdvisorId == advisorId && !u.IsDeleted && u.IsActive);
                if (student == null)
                    return NotFound();
                var studentCourses = await _db.StudentCourses.Where(s => s.StudentId == id && !s.IsDeleted)
                    .Include(s => s.OfferedCourses.Courses)
                    .Include(s => s.OfferedCourses.Semesters)
                    .OrderByDescending(s => s.CreatedAt)
                    .Select(s => new StudentCourseInfoDto
                    {
                        CourseCode = s.OfferedCourses.Courses.CourseCode,
                        CourseName = s.OfferedCourses.Courses.CourseName,
                        SemesterName = s.OfferedCourses.Semesters.SemesterName,
                        Credits = s.OfferedCourses.Courses.Credits,
                        OfferedCourseId = s.OfferedCourseId,
                        ApprovalStatus = s.ApprovalStatus,
                        RequestDate = s.CreatedAt,
                        StudentId = s.StudentId
                    }).ToListAsync();
                var studentDetailDto = new StudentDetailDto
                {
                    FullName = student.Name + " " + student.Surname,
                    StudentNo = student.StudentNo,
                    Email = student.Email,
                    DepartmentName = student.Departments.Name,
                    CreatedAt = student.CreatedAt,
                    StudentCourses = studentCourses
                };
                return Ok(studentDetailDto);
            }
            catch (Exception ex)
            {
                Logger.Error("Öğrenci detayları alınırken bir hata oluştu: ", ex);
                return InternalServerError();
            }
        }

        [HttpPost]
        [Route("update-approval-status")]
        public async Task<IHttpActionResult> PostUpdateStatus(ApprovalRequestModel model)
        {
            var advisor = await _db.Users.FindAsync(Guid.Parse(GetActiveUserId()));
            if (model.StudentIds == null || model.OfferedCourseIds == null ||
                model.StudentIds.Count != model.OfferedCourseIds.Count)
            {
                return Json(new { success = false, message = "Geçersiz istek parametreleri." });
            }

            using (var transaction = _db.Database.BeginTransaction())
            {
                try
                {
                    var successCount = 0;
                    var failCount = 0;

                    for (int i = 0; i < model.StudentIds.Count; i++)
                    {
                        var studentId = model.StudentIds[i];
                        var offeredCourseId = model.OfferedCourseIds[i];

                        var studentCourse = _db.StudentCourses
                            .Include(sc => sc.Users)
                            .Include(sc => sc.OfferedCourses)
                            .FirstOrDefault(sc => sc.StudentId == studentId &&
                                                  sc.OfferedCourseId == offeredCourseId &&
                                                  sc.Users.AdvisorId == advisor.UserId);

                        if (studentCourse != null)
                        {
                            if (studentCourse.ApprovalStatus == (int)ApprovalStatus.Bekliyor)
                            {
                                studentCourse.ApprovalStatus = (int)model.NewStatus;
                                studentCourse.UpdatedBy = GetActiveUserId();
                                studentCourse.UpdatedAt = DateTime.UtcNow;
                                if (model.NewStatus == ApprovalStatus.Reddedildi)
                                {
                                    if (studentCourse.OfferedCourses.CurrentUserCount > 0)
                                    {
                                        studentCourse.OfferedCourses.CurrentUserCount--;
                                    }
                                }

                                successCount++;
                            }
                            else
                            {
                                failCount++;
                            }
                        }
                        else
                        {
                            failCount++;
                        }
                    }

                    await _db.SaveChangesAsync();
                    transaction.Commit();

                    var statusText = model.NewStatus == ApprovalStatus.Onaylandı ? "onaylandı" : "reddedildi";
                    var message = $"{successCount} ders kaydı başarıyla {statusText}.";

                    if (failCount > 0)
                    {
                        message += $" {failCount} kayıt işlenemedi.";
                    }

                    return Json(new { success = true, message = message });
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return Json(new { success = false, message = "Bir hata oluştu: " + ex.Message });
                }
            }
        }

        private string GetActiveUserId()
        {
            return TokenManager.GetPrincipal(Request.Headers.Authorization.Parameter)
                .FindFirst(ClaimTypes.NameIdentifier).Value;
        }

        private Task<List<DepartmentSelectionDTO>> FillDepartmentsList()
        {
            return _db.Departments
                .Where(d => !d.IsDeleted)
                .Select(d => new DepartmentSelectionDTO()
                {
                    DepartmentId = d.DepartmentId,
                    DepartmentName = d.Name
                }).ToListAsync();
        }
    }
}