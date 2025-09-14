using log4net;
using OgrenciPortalApi.Models;
using OgrenciPortalApi.Utils;
using Shared.DTO;
using Shared.Enums;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using MailKit;
using MailKit.Net.Smtp;
using MimeKit;
using MailService = OgrenciPortalApi.Services.MailService;

namespace OgrenciPortalApi.Controllers
{
    [Authorize(Roles = nameof(Roles.Öğrenci))]
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
            Logger.Debug("Hey hey buradayım" + typeof(StudentController));
            try
            {
                var userId = GetActiveUserId();
                var user = await _db.Users.FindAsync(userId);
                if (user == null) return Unauthorized();

                var enrolledOrPendingIds = await _db.StudentCourses
                    .Where(sc => sc.StudentId == userId && sc.OfferedCourses.Semesters.IsActive)
                    .Select(sc => sc.OfferedCourseId)
                    .ToListAsync();

                var enrolledOrPendingCourseIds = new HashSet<Guid>(enrolledOrPendingIds);
                new HashSet<Guid>();

                var pendingCoursesData = await _db.StudentCourses
                    .Where(sc =>
                        enrolledOrPendingIds.Contains(sc.OfferedCourseId) &&
                        sc.ApprovalStatus == (int)ApprovalStatus.Bekliyor)
                    .Select(sc => new
                    {
                        sc.OfferedCourseId,
                        sc.OfferedCourses.Courses.CourseCode,
                        sc.OfferedCourses.Courses.CourseName,
                        Credit = sc.OfferedCourses.Courses.Credits,
                        TeacherName = sc.OfferedCourses.Users.Name + " " + sc.OfferedCourses.Users.Surname,
                        sc.OfferedCourses.DayOfWeek,
                        sc.OfferedCourses.StartTime,
                        sc.OfferedCourses.EndTime,
                        sc.OfferedCourses.Classroom,
                        sc.OfferedCourses.Quota,
                        sc.OfferedCourses.CurrentUserCount
                    })
                    .ToListAsync();

                var pendingCourses = pendingCoursesData.Select(sc => new EnrollDTO
                {
                    OfferedCourseId = sc.OfferedCourseId,
                    CourseCode = sc.CourseCode,
                    CourseName = sc.CourseName,
                    Credit = sc.Credit,
                    TeacherName = sc.TeacherName,
                    DayOfWeek = ((DaysOfWeek)sc.DayOfWeek).ToString(),
                    StartTime = sc.StartTime.ToString(@"hh\:mm"),
                    EndTime = sc.EndTime.ToString(@"hh\:mm"),
                    Classroom = sc.Classroom,
                    Quota = sc.Quota,
                    CurrentUserCount = sc.CurrentUserCount
                }).ToList();

                var availableCoursesData = await _db.OfferedCourses
                    .Where(oc => !oc.IsDeleted && oc.Courses.DepartmentId == user.DepartmentId && oc.Semesters.IsActive)
                    .Where(oc => !enrolledOrPendingCourseIds.Contains(oc.Id))
                    .Select(s => new
                    {
                        s.Id,
                        s.CourseId,
                        s.Courses.CourseCode,
                        s.Courses.CourseName,
                        Credit = s.Courses.Credits,
                        s.Quota,
                        s.CurrentUserCount,
                        TeacherName = s.Users.Name + " " + s.Users.Surname,
                        s.DayOfWeek,
                        s.StartTime,
                        s.EndTime,
                        s.Classroom
                    })
                    .ToListAsync();

                var availableCourses = availableCoursesData.Select(s => new EnrollDTO
                {
                    OfferedCourseId = s.Id,
                    CourseId = s.CourseId,
                    CourseCode = s.CourseCode,
                    CourseName = s.CourseName,
                    Credit = s.Credit,
                    Quota = s.Quota,
                    CurrentUserCount = s.CurrentUserCount,
                    TeacherName = s.TeacherName,
                    DayOfWeek = ((DaysOfWeek)s.DayOfWeek).ToString(),
                    StartTime = s.StartTime.ToString(@"hh\:mm"),
                    EndTime = s.EndTime.ToString(@"hh\:mm"),
                    Classroom = s.Classroom
                }).ToList();
                var activeSemester = await _db.Semesters.FirstOrDefaultAsync(x => x.IsActive);
                Logger.Debug(
                    $"Available Courses: {availableCourses}, Pending Courses: {pendingCourses}, ActiveSemesterName: {activeSemester?.SemesterName ?? "Aktif Dönem Bulunamadı"}");

                var totalApprovedCreditsList = await _db.StudentCourses
                    .Where(sc =>
                        sc.StudentId == userId &&
                        sc.ApprovalStatus == (int)ApprovalStatus.Onaylandı &&
                        sc.OfferedCourses.Semesters.IsActive)
                    .Select(sc => sc.OfferedCourses.Courses.Credits == null ? 0 : sc.OfferedCourses.Courses.Credits).ToListAsync();
                Logger.Debug($"Total Approved Credits: {totalApprovedCreditsList}");
                var totalApprovedCredits = 0;
                if (totalApprovedCreditsList.Any())
                    totalApprovedCredits = totalApprovedCreditsList.Sum();
                var remainingCredits = Math.Max(0, 30 - totalApprovedCredits);

                Logger.Debug($"Remaining Credits: {remainingCredits}");

                var viewModel = new EnrollPageDTO
                {
                    EnrollableList = availableCourses,
                    PendingCourses = pendingCourses,
                    ActiveSemesterName = activeSemester?.SemesterName ?? "Aktif Dönem Bulunamadı",
                    RemainingCredits = remainingCredits
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
        [Route("reset-pending-enrollments")]
        [ResponseType(typeof(object))]
        public async Task<IHttpActionResult> ResetPendingEnrollments()
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
                        return Ok(new { Message = "Sıfırlanacak, onay bekleyen bir ders kaydınız bulunmamaktadır." });
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

                    return Ok(new
                    {
                        Message =
                            "Onay bekleyen tüm dersleriniz başarıyla sıfırlandı. Şimdi yeniden seçim yapabilirsiniz."
                    });
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Logger.Error("Onay bekleyen ders kayıtları sıfırlanırken hata oluştu.", ex);
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
            var totalCredits = 0;
            if (selectedOfferedCourseIds == null || !selectedOfferedCourseIds.Any())
                return BadRequest("Lütfen en az bir ders seçin.");

            var studentId = GetActiveUserId();
            var student = await _db.Users.FindAsync(studentId);
            var advisor = await _db.Users.FirstOrDefaultAsync(u => u.UserId == student.AdvisorId);

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
                totalCredits += course.Courses.Credits;
                if (totalCredits > 30)
                    return BadRequest("30 olan kredi sınırını aştınız.");
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

                    try
                    {
                        string templatePath =
                            System.Web.Hosting.HostingEnvironment.MapPath("~/Views/CourseApprovalMail.html");
                        string htmlTemplate = File.ReadAllText(templatePath);
                        htmlTemplate =
                            htmlTemplate.Replace("[Öğrenci Adı Soyadı]", student.Name + " " + student.Surname);
                        htmlTemplate = htmlTemplate.Replace("[Öğrenci Numarası]", student.StudentNo);
                        htmlTemplate = htmlTemplate.Replace("[Onay_Linki]",
                            AppSettings.JwtAudience + "Advisor/CourseApprovals");
                        await MailService.Instance.SendEmailAsync(advisor.Email, "Yeni Ders Kaydı", htmlTemplate);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error("Ders onay bilgilendirme maili gönderilirken hata oluştu.", ex);
                    }

                    return Ok(new
                    {
                        Message =
                            "Ders seçimleriniz başarıyla danışman onayına gönderilmiştir ve danışmanınız bigilendirilmiştir."
                    });
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
                            studentCoursesInDb.Count(c => c.ApprovalStatus == (int)ApprovalStatus.Reddedildi),
                        // YENİ EKLENEN HESAPLAMALAR
                        ApprovedCredits = studentCoursesInDb
                            .Where(c => c.ApprovalStatus == (int)ApprovalStatus.Onaylandı)
                            .Sum(c => c.OfferedCourses.Courses.Credits),
                        PendingCredits = studentCoursesInDb
                            .Where(c => c.ApprovalStatus == (int)ApprovalStatus.Bekliyor)
                            .Sum(c => c.OfferedCourses.Courses.Credits)
                    },
                    Courses = studentCoursesInDb.Select(sc => new MyCourseDto
                        {
                            CourseId = sc.OfferedCourseId,
                            CourseCode = sc.OfferedCourses.Courses.CourseCode,
                            CourseName = sc.OfferedCourses.Courses.CourseName,
                            Credits = sc.OfferedCourses.Courses.Credits,
                            TeacherFullName = sc.OfferedCourses.Users.Name + " " + sc.OfferedCourses.Users.Surname,
                            SemesterName = sc.OfferedCourses.Semesters.SemesterName,
                            DayOfWeek = ((DaysOfWeek)sc.OfferedCourses.DayOfWeek).ToString(),
                            TimeSlot = $"{sc.OfferedCourses.StartTime:hh\\:mm} - {sc.OfferedCourses.EndTime:hh\\:mm}",
                            ApprovalStatus = sc.ApprovalStatus,
                            ApprovalStatusText = ((ApprovalStatus)sc.ApprovalStatus).ToString(),
                            CreatedAt = sc.CreatedAt,
                            Classroom = sc.OfferedCourses.Classroom,
                            Description = sc.OfferedCourses.Courses.Description
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

        [HttpPost]
        [Route("check-conflict")]
        [ResponseType(typeof(object))]
        public async Task<IHttpActionResult> CheckConflict([FromBody] CheckConflictDto dto)
        {
            var studentId = GetActiveUserId();
            var offeredCourseGuid = Guid.Parse(dto.offeredCourseId);
            var selectedCourse = await _db.OfferedCourses
                .Where(oc => oc.Id == offeredCourseGuid)
                .Select(oc => new ConflictCheckDTO
                {
                    CourseName = oc.Courses.CourseName,
                    DayOfWeek = oc.DayOfWeek,
                    StartTime = oc.StartTime,
                    EndTime = oc.EndTime
                }).FirstOrDefaultAsync();

            if (selectedCourse == null)
                return BadRequest("Ders bulunamadı.");

            var conflictChecker = new ConflictCheck();
            var conflictingCourse = conflictChecker.GetConflictingCourse(selectedCourse, studentId);

            if (conflictingCourse != null)
            {
                return Ok(new
                {
                    hasConflict = true,
                    Message = $"Seçtiğiniz ders, '{conflictingCourse.CourseName}' dersi ile çakışıyor."
                });
            }

            return Ok(new { hasConflict = false });
        }
    }
}