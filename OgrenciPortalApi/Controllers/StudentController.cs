using log4net;
using OgrenciPortalApi.Attributes;
using OgrenciPortalApi.Models;
using OgrenciPortalApi.Utils;
using OgrenciPortali.DTOs;
using OgrenciPortali.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IdentityModel.Claims;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace OgrenciPortalApi.Controllers
{
    [JwtAuth]
    [RoutePrefix("api/students")]
    public class StudentController : ApiController
    {
        private readonly ogrenci_portalEntities _db = new ogrenci_portalEntities();
        private ILog Logger = LogManager.GetLogger(typeof(StudentController));

        [HttpGet]
        [Route("enroll")]
        public async Task<IHttpActionResult> GetEnroll()
        {
            try
            {
                var userId = Guid.Parse(GetActiveUserId());
                var user = await _db.Users.FindAsync(userId);

                var pendingStudentCourses = await _db.StudentCourses
                    .Where(sc =>
                        sc.StudentId == userId && sc.ApprovalStatus == (int)ApprovalStatus.Bekliyor &&
                        !sc.OfferedCourses.IsDeleted)
                    .Select(sc => new EnrollDTO
                    {
                        CourseId = sc.OfferedCourses.CourseId,
                        CourseCode = sc.OfferedCourses.Courses.CourseCode,
                        CourseName = sc.OfferedCourses.Courses.CourseName,
                        Credit = sc.OfferedCourses.Courses.Credits,
                        TeacherName = sc.OfferedCourses.Users.Name + " " + sc.OfferedCourses.Users.Surname,
                        OfferedCourseId = sc.OfferedCourseId,
                    })
                    .ToListAsync();

                var pendingCourseIds = new HashSet<Guid>(pendingStudentCourses
                    .Where(p => p.CourseId.HasValue)
                    .Select(p => p.CourseId.Value));

                var allDepartmentCourses = await _db.OfferedCourses
                    .Where(oc => !oc.IsDeleted && oc.Courses.DepartmentId == user.DepartmentId)
                    .Where(oc => !pendingCourseIds.Contains(oc.CourseId))
                    .Select(s => new EnrollDTO
                    {
                        CourseId = s.CourseId,
                        CourseCode = s.Courses.CourseCode,
                        CourseName = s.Courses.CourseName,
                        Quota = s.Quota,
                        CurrentUserCount = s.CurrentUserCount,
                        OfferedCourseId = s.Id,
                        SemesterName = s.Semesters.SemesterName,
                        Credit = s.Courses.Credits,
                        TeacherName = s.Users.Name + " " + s.Users.Surname,
                        DepartmentName = s.Courses.Departments.Name
                    })
                    .ToListAsync();

                var viewModel = new EnrollPageDTO
                {
                    EnrollableList = allDepartmentCourses,
                    PendingCourses = pendingStudentCourses
                };

                return Ok(viewModel);
            }
            catch (Exception ex)
            {
                Logger.Error("Seçilebilir dersler çekilirken hata", ex);
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("reset-enrollments")]
        public async Task<IHttpActionResult> PostResetEnrollments()
        {
            var studentId = Guid.Parse(GetActiveUserId());
            using (var transaction = _db.Database.BeginTransaction())
            {
                try
                {
                    var enrollmentsToReset = await _db.StudentCourses
                        .Where(sc => sc.StudentId == studentId && sc.ApprovalStatus == (int)ApprovalStatus.Bekliyor)
                        .ToListAsync();

                    if (!enrollmentsToReset.Any())
                    {
                        return Json(new { success = true, message = "Sıfırlanacak ders kaydınız bulunmamaktadır." });
                    }

                    foreach (var enrollment in enrollmentsToReset)
                    {
                        var offeredCourse = await _db.OfferedCourses.FindAsync(enrollment.OfferedCourseId);
                        if (offeredCourse != null)
                        {
                            offeredCourse.CurrentUserCount--;
                        }

                        _db.StudentCourses.Remove(enrollment);
                    }

                    await _db.SaveChangesAsync();
                    transaction.Commit();

                    return Json(new
                    {
                        success = true,
                        message = "Onay bekleyen tüm dersleriniz sıfırlandı. Şimdi yeniden seçim yapabilirsiniz."
                    });
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Logger.Error("Ders sıfırlama sırasında hata.", ex);
                    return Json(new
                        { success = false, message = "Dersler sıfırlanırken bir hata oluştu: " + ex.Message });
                }
            }
        }


        [HttpPost]
        [Route("enroll")]
        public async Task<IHttpActionResult> PostEnrollments([FromBody] List<Guid> selectedCourseIds)
        {
            var student = await _db.Users.FindAsync(Guid.Parse(GetActiveUserId()));
            if (selectedCourseIds == null || !selectedCourseIds.Any())
                return BadRequest("Lütfen en az bir ders seçin.");
            using (var transaction = _db.Database.BeginTransaction())
            {
                try
                {
                    foreach (var courseId in selectedCourseIds)
                    {
                        var offeredCourse = _db.OfferedCourses.Include(offeredCourse1 => offeredCourse1.Courses)
                            .FirstOrDefault(oc => oc.CourseId == courseId && !oc.IsDeleted);
                        if (offeredCourse == null)
                        {
                            throw new Exception("Seçilen derslerden biri sistemde bulunamadı: " + courseId);
                        }

                        var isAlreadyEnrolled = _db.StudentCourses
                            .Any(sc => sc.StudentId == student.UserId && sc.OfferedCourseId == offeredCourse.Id);

                        if (isAlreadyEnrolled)
                        {
                            throw new Exception("'" + offeredCourse.Courses.CourseName +
                                                "' dersine zaten kayıtlısınız veya onay bekliyor.");
                        }

                        if (offeredCourse.CurrentUserCount >= offeredCourse.Quota)
                        {
                            throw new Exception("'" + offeredCourse.Courses.CourseName +
                                                "' dersinin kontenjanı dolu.");
                        }

                        var studentCourse = new StudentCourses()
                        {
                            StudentId = student.UserId,
                            OfferedCourseId = offeredCourse.Id,
                            ApprovalStatus = (int)ApprovalStatus.Bekliyor,
                            CreatedBy = GetActiveUserId(),
                            UpdatedBy = GetActiveUserId(),
                            CreatedAt = DateTime.Now,
                            UpdatedAt = DateTime.Now,
                        };
                        _db.StudentCourses.Add(studentCourse);
                        offeredCourse.CurrentUserCount++;
                    }

                    await _db.SaveChangesAsync();
                    transaction.Commit();


                    return Json(new
                    {
                        success = true,
                        message = "Ders seçimleriniz başarıyla danışman onayına gönderilmiştir."
                    });
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return Json(new { success = false, message = ex.Message });
                }
            }
        }

        [HttpGet]
        [Route("my-courses")]
        public async Task<IHttpActionResult> GetMyCourses()
        {
            try
            {
                var studentId = Guid.Parse(GetActiveUserId());
                var studentCoursesInDb = await _db.StudentCourses
                    .Include(sc => sc.OfferedCourses.Courses)
                    .Include(sc => sc.OfferedCourses.Users) // Öğretmen bilgisi
                    .Include(sc => sc.OfferedCourses.Semesters)
                    .Where(sc => sc.StudentId == studentId && !sc.IsDeleted)
                    .ToListAsync();
                var pageDto = new MyCoursesDTO
                {
                    Summary = new SummaryDto
                    {
                        TotalCount = studentCoursesInDb.Count,
                        ApprovedCount =
                            studentCoursesInDb.Count(c => c.ApprovalStatus == (int)ApprovalStatus.Onaylandı),
                        PendingCount = studentCoursesInDb.Count(c => c.ApprovalStatus == (int)ApprovalStatus.Bekliyor),
                        RejectedCount =
                            studentCoursesInDb.Count(c => c.ApprovalStatus == (int)ApprovalStatus.Reddedildi)
                    },
                    Courses = studentCoursesInDb.Select(sc => new MyCourseDto
                        {
                            CourseCode = sc.OfferedCourses.Courses.CourseCode,
                            CourseName = sc.OfferedCourses.Courses.CourseName,
                            Credits = sc.OfferedCourses.Courses.Credits,
                            TeacherFullName = sc.OfferedCourses.Users.Name + " " + sc.OfferedCourses.Users.Surname,
                            SemesterName = sc.OfferedCourses.Semesters.SemesterName,
                            DayOfWeek = ((DaysOfWeek)sc.OfferedCourses.DayOfWeek).ToString(),
                            TimeSlot = $"{sc.OfferedCourses.StartTime:hh\\:mm} - {sc.OfferedCourses.EndTime:hh\\:mm}",
                            ApprovalStatus = sc.ApprovalStatus,
                            ApprovalStatusText = ((ApprovalStatus)sc.ApprovalStatus).ToString(),
                            CreatedAt = sc.CreatedAt
                        })
                        .OrderByDescending(c => c.CreatedAt)
                        .ToList()
                };
                return Ok(pageDto);
            }
            catch (Exception ex)
            {
                Logger.Error("Öğrencinin dersleri çekilirken hata oluştu.", ex);
                return InternalServerError(ex);
            }
        }

        public string GetActiveUserId()
        {
            return TokenManager.GetPrincipal(Request.Headers.Authorization.Parameter)
                .FindFirst(ClaimTypes.NameIdentifier).Value;
        }
    }
}