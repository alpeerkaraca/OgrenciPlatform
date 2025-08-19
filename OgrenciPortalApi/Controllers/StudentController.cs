using log4net;
using OgrenciPortalApi.Attributes;
using OgrenciPortalApi.Models;
using Shared.DTO;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Shared.Enums;

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
                    .Where(sc =>
                        sc.StudentId == userId && sc.ApprovalStatus == (int)ApprovalStatus.Bekliyor &&
                        !sc.OfferedCourses.IsDeleted && sc.OfferedCourses.Semesters.IsActive)
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

                var pendingCourseIds = new HashSet<Guid>(pendingStudentCourses.Select(p => p.CourseId.Value)) ??
                                       new HashSet<Guid>();

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
                    EnrollableList = allDepartmentCourses ?? new List<EnrollDTO>(),
                    PendingCourses = pendingStudentCourses ?? new List<EnrollDTO>()
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

                    return Ok(
                        "Onay bekleyen tüm dersleriniz sıfırlandı. Şimdi yeniden seçim yapabilirsiniz.");
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

            var existingSchedule = await _db.StudentCourses
                .Where(sc => sc.StudentId == studentId && !sc.IsDeleted && sc.OfferedCourses.Semesters.IsActive)
                .Select(sc => new ConflictCheckDTO
                {
                    CourseName = sc.OfferedCourses.Courses.CourseName,
                    DayOfWeek = sc.OfferedCourses.DayOfWeek,
                    StartTime = sc.OfferedCourses.StartTime,
                    EndTime = sc.OfferedCourses.EndTime
                }).ToListAsync();

            var selectedCourses = await _db.OfferedCourses
                .Include(oc => oc.Courses)
                .Where(oc => selectedOfferedCourseIds.Contains(oc.Id) && !oc.IsDeleted)
                .ToListAsync();

            if (selectedCourses.Count != selectedOfferedCourseIds.Count)
                return BadRequest("Seçilen derslerden bazıları sistemde bulunamadı veya silinmiş.");

            var alreadyEnrolledCourseIds = await _db.StudentCourses
                .Where(sc => sc.StudentId == studentId && selectedOfferedCourseIds.Contains(sc.OfferedCourseId))
                .Select(sc => sc.OfferedCourseId)
                .ToListAsync();

            foreach (var course in selectedCourses)
            {
                if (alreadyEnrolledCourseIds.Contains(course.Id))
                    return BadRequest($"'{course.Courses.CourseName}' dersine zaten kayıtlısınız veya onay bekliyor.");

                if (course.CurrentUserCount >= course.Quota)
                    return BadRequest($"'{course.Courses.CourseName}' dersinin kontenjanı dolu.");
            }

            var newScheduleItems = selectedCourses.Select(c => new ConflictCheckDTO
            {
                CourseName = c.Courses.CourseName,
                DayOfWeek = c.DayOfWeek,
                StartTime = c.StartTime,
                EndTime = c.EndTime
            });

            var fullSchedule = existingSchedule.Concat(newScheduleItems).ToList();

            var scheduleByDay = fullSchedule
                .GroupBy(c => c.DayOfWeek)
                .ToDictionary(g => g.Key, g => g.ToList());

            foreach (var dayGroup in scheduleByDay.Values)
            {
                if (dayGroup.Count < 2) continue;

                for (int i = 0; i < dayGroup.Count; i++)
                {
                    for (int j = i + 1; j < dayGroup.Count; j++)
                    {
                        var course1 = dayGroup[i];
                        var course2 = dayGroup[j];

                        if (course1.StartTime < course2.EndTime && course2.StartTime < course1.EndTime)
                        {
                            return BadRequest(
                                $"Dersler arasında zaman çakışması var: '{course1.CourseName}' ve '{course2.CourseName}'");
                        }
                    }
                }
            }


            using (var transaction = _db.Database.BeginTransaction())
            {
                try
                {
                    var newEnrollments = selectedCourses.Select(course =>
                    {
                        course.CurrentUserCount++;
                        return new StudentCourses
                        {
                            StudentId = studentId,
                            OfferedCourseId = course.Id,
                            ApprovalStatus = (int)ApprovalStatus.Bekliyor,
                            CreatedAt = DateTime.Now,
                            CreatedBy = studentId.ToString(),
                            UpdatedAt = DateTime.Now,
                            UpdatedBy = studentId.ToString(),
                        };
                    }).ToList();

                    _db.StudentCourses.AddRange(newEnrollments);

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
                Logger.Error("Öğrencinin dersleri alınırken hata oluştu.", ex);
                return InternalServerError(new Exception("Dersleriniz alınırken bir hata oluştu."));
            }
        }
    }
}