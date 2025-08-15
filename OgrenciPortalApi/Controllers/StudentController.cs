using log4net;
using OgrenciPortalApi.Attributes;
using OgrenciPortalApi.Models;
using OgrenciPortali.DTOs;
using OgrenciPortali.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace OgrenciPortalApi.Controllers
{
    [JwtAuth]
    [RoutePrefix("api/student")]
    public class StudentController : BaseApiController
    {
        private readonly ILog Logger = LogManager.GetLogger(typeof(StudentController));

        /// <summary>
        /// Öğrencinin kayıt olabileceği ve onaya gönderdiği dersleri listeler.
        /// </summary>
        /// <returns>Öğrencinin ders kayıt sayfasının verilerini içeren bir HTTP yanıtı döner.</returns>
        [HttpGet]
        [Route("enroll")]
        [ResponseType(typeof(EnrollPageDTO))]
        public async Task<IHttpActionResult> GetEnrollableCourses()
        {
            try
            {
                var userId = GetActiveUserId();
                var user = await _db.Users.FindAsync(userId);
                if (user == null)
                {
                    return Unauthorized();
                }

                var pendingStudentCourses = await _db.StudentCourses
                    .Where(sc => sc.StudentId == userId && sc.ApprovalStatus == (int)ApprovalStatus.Bekliyor && !sc.OfferedCourses.IsDeleted)
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

                var pendingCourseIds = new HashSet<Guid>(pendingStudentCourses.Select(p => p.CourseId.Value));

                var allDepartmentCourses = await _db.OfferedCourses
                    .Where(oc => !oc.IsDeleted && oc.Courses.DepartmentId == user.DepartmentId && oc.Semesters.IsActive)
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
                Logger.Error("Seçilebilir dersler alınırken hata oluştu.", ex);
                return InternalServerError(new Exception("Seçilebilir dersler alınırken bir hata oluştu."));
            }
        }

        /// <summary>
        /// Öğrencinin onay bekleyen tüm ders kayıtlarını sıfırlar.
        /// </summary>
        /// <returns>İşlem sonucunu bildiren bir HTTP yanıtı döner.</returns>
        [HttpPost]
        [Route("reset-enrollments")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> ResetEnrollments()
        {
            var studentId = GetActiveUserId();
            using (var transaction = _db.Database.BeginTransaction())
            {
                try
                {
                    var enrollmentsToReset = await _db.StudentCourses
                        .Where(sc => sc.StudentId == studentId && sc.ApprovalStatus == (int)ApprovalStatus.Bekliyor)
                        .ToListAsync();

                    if (!enrollmentsToReset.Any())
                    {
                        return Ok(new { message = "Sıfırlanacak, onay bekleyen bir ders kaydınız bulunmamaktadır." });
                    }

                    foreach (var enrollment in enrollmentsToReset)
                    {
                        var offeredCourse = await _db.OfferedCourses.FindAsync(enrollment.OfferedCourseId);
                        if (offeredCourse != null && offeredCourse.CurrentUserCount > 0)
                        {
                            offeredCourse.CurrentUserCount--;
                        }
                        _db.StudentCourses.Remove(enrollment);
                    }

                    await _db.SaveChangesAsync();
                    transaction.Commit();

                    return Ok(new { message = "Onay bekleyen tüm dersleriniz sıfırlandı. Şimdi yeniden seçim yapabilirsiniz." });
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Logger.Error("Ders kayıtları sıfırlanırken hata oluştu.", ex);
                    return InternalServerError(new Exception("Dersler sıfırlanırken bir hata oluştu."));
                }
            }
        }

        /// <summary>
        /// Öğrencinin seçtiği derslere kaydını yapar ve danışman onayına gönderir.
        /// </summary>
        /// <param name="selectedOfferedCourseIds">Kaydolunacak açılmış derslerin ID listesi.</param>
        /// <returns>İşlem sonucunu bildiren bir HTTP yanıtı döner.</returns>
        [HttpPost]
        [Route("enroll")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> EnrollCourses([FromBody] List<Guid> selectedOfferedCourseIds)
        {
            if (selectedOfferedCourseIds == null || !selectedOfferedCourseIds.Any())
                return BadRequest("Lütfen en az bir ders seçin.");

            var studentId = GetActiveUserId();
            using (var transaction = _db.Database.BeginTransaction())
            {
                try
                {
                    foreach (var offeredCourseId in selectedOfferedCourseIds)
                    {
                        var offeredCourse = await _db.OfferedCourses.Include(oc => oc.Courses)
                            .FirstOrDefaultAsync(oc => oc.Id == offeredCourseId && !oc.IsDeleted);

                        if (offeredCourse == null)
                            return BadRequest($"Seçilen derslerden biri sistemde bulunamadı: ID {offeredCourseId}");

                        if (await _db.StudentCourses.AnyAsync(sc => sc.StudentId == studentId && sc.OfferedCourseId == offeredCourse.Id))
                            return BadRequest($"'{offeredCourse.Courses.CourseName}' dersine zaten kayıtlısınız veya onay bekliyor.");

                        if (offeredCourse.CurrentUserCount >= offeredCourse.Quota)
                            return BadRequest($"'{offeredCourse.Courses.CourseName}' dersinin kontenjanı dolu.");

                        var studentCourse = new StudentCourses()
                        {
                            StudentId = studentId,
                            OfferedCourseId = offeredCourse.Id,
                            ApprovalStatus = (int)ApprovalStatus.Bekliyor,
                            CreatedBy = studentId.ToString(),
                            UpdatedBy = studentId.ToString(),
                            CreatedAt = DateTime.Now,
                            UpdatedAt = DateTime.Now,
                        };
                        _db.StudentCourses.Add(studentCourse);
                        offeredCourse.CurrentUserCount++;
                    }

                    await _db.SaveChangesAsync();
                    transaction.Commit();

                    return Ok(new { message = "Ders seçimleriniz başarıyla danışman onayına gönderilmiştir." });
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Logger.Error("Ders kaydı sırasında bir hata oluştu.", ex);
                    return InternalServerError(new Exception("Ders kaydı sırasında beklenmedik bir hata oluştu."));
                }
            }
        }

        /// <summary>
        /// Öğrencinin kayıtlı olduğu tüm dersleri ve durumlarını listeler.
        /// </summary>
        /// <returns>Öğrencinin derslerinin listesini ve özetini içeren bir HTTP yanıtı döner.</returns>
        [HttpGet]
        [Route("my-courses")]
        [ResponseType(typeof(MyCoursesDTO))]
        public async Task<IHttpActionResult> GetMyCourses()
        {
            try
            {
                var studentId = GetActiveUserId();
                var studentCoursesInDb = await _db.StudentCourses
                    .Include(sc => sc.OfferedCourses.Courses)
                    .Include(sc => sc.OfferedCourses.Users)
                    .Include(sc => sc.OfferedCourses.Semesters)
                    .Where(sc => sc.StudentId == studentId && !sc.IsDeleted)
                    .ToListAsync();

                var pageDto = new MyCoursesDTO
                {
                    Summary = new SummaryDto
                    {
                        TotalCount = studentCoursesInDb.Count,
                        ApprovedCount = studentCoursesInDb.Count(c => c.ApprovalStatus == (int)ApprovalStatus.Onaylandı),
                        PendingCount = studentCoursesInDb.Count(c => c.ApprovalStatus == (int)ApprovalStatus.Bekliyor),
                        RejectedCount = studentCoursesInDb.Count(c => c.ApprovalStatus == (int)ApprovalStatus.Reddedildi)
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
                Logger.Error("Öğrencinin dersleri alınırken hata oluştu.", ex);
                return InternalServerError(new Exception("Dersleriniz alınırken bir hata oluştu."));
            }
        }
    }
}
