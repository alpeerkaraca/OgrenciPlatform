using log4net;
using Shared.DTO;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Mvc;
using Shared.Enums;

namespace OgrenciPortalApi.Controllers
{
    [System.Web.Http.Authorize(Roles = nameof(Roles.Danışman))]
    [System.Web.Http.RoutePrefix("api/advisor")]
    public class AdvisorController : BaseApiController
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(AdvisorController));

        /// <summary>
        /// Danışmanın onayını bekleyen ders kayıtlarını listeler.
        /// </summary>
        /// <returns>Onay bekleyen ders kayıtlarının listesini içeren bir HTTP yanıtı döner.</returns>
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("approvals")]
        [ResponseType(typeof(AdvisorApprovalDTO))]
        public async Task<IHttpActionResult> GetApprovals()
        {
            try
            {
                var advisorId = GetActiveUserId();

                var approvalsFromDb = await _db.StudentCourses
                    .Where(sc =>
                        sc.OfferedCourses.TeacherId == advisorId &&
                        sc.ApprovalStatus == (int)ApprovalStatus.Bekliyor)
                    .Select(sc => new ApprovalRequestDto // Mevcut DTO'yu kullanmaya devam ediyoruz
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
                    .OrderBy(x => x.StudentName).ThenBy(x => x.CourseCode) // Öğrenci ve derse göre sırala
                    .ToListAsync();

                var groupedApprovals = approvalsFromDb
                    .GroupBy(a => a.StudentId)
                    .Select(g => new StudentApprovalGroupDto
                    {
                        StudentId = g.Key,
                        StudentName = g.First().StudentName,
                        StudentNo = g.First().StudentNo,
                        CourseRequests = g.ToList(),
                        TotalPendingCredits = g.Sum(c => c.Credits)
                    })
                    .ToList();

                var dto = new AdvisorApprovalDTO
                {
                    PendingStudentGroups = groupedApprovals ?? new List<StudentApprovalGroupDto>()
                };
                return Ok(dto);
            }
            catch (Exception ex)
            {
                Logger.Error("Danışman için onaylar alınırken hata oluştu.", ex);
                return InternalServerError(new Exception("Onaylar alınırken bir sunucu hatası oluştu."));
            }
        }

        /// <summary>
        /// Danışmana atı olan öğrencileri listeler.
        /// </summary>
        /// <returns>Danışmanın öğrencilerinin listesini içeren bir HTTP yanıtı döner.</returns>
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("students")]
        [ResponseType(typeof(AdvisedStudentsDTO))]
        public async Task<IHttpActionResult> GetAdvisedStudents()
        {
            try
            {
                var advisorId = GetActiveUserId();

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
                Logger.Error("Danışman öğrencileri alınırken hata oluştu.", ex);
                return InternalServerError(new Exception("Danışman öğrencileri alınırken bir sunucu hatası oluştu."));
            }
        }

        /// <summary>
        /// Belirtilen ID'ye sahip öğrencinin detaylarını getirir.
        /// </summary>
        /// <param name="id">Detayları görüntülenecek öğrencinin ID'si.</param>
        /// <returns>Öğrencinin detay bilgilerini içeren bir HTTP yanıtı döner.</returns>
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("student/{id:guid}")]
        [ResponseType(typeof(StudentDetailDto))]
        public async Task<IHttpActionResult> GetStudent(Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest("Geçersiz öğrenci ID'si.");
            }

            try
            {
                var advisorId = GetActiveUserId();
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
                Logger.Error($"ID'si {id} olan öğrencinin detayları alınırken hata oluştu.", ex);
                return InternalServerError(new Exception("Öğrenci detayları alınırken bir hata oluştu."));
            }
        }

        /// <summary>
        /// Öğrenci ders kayıtlarının onay durumunu günceller (Onayla/Reddet).
        /// </summary>
        /// <param name="model">Onay durumu güncellenecek kayıtların bilgilerini içeren model.</param>
        /// <returns>İşlem sonucunu bildiren bir HTTP yanıtı döner.</returns>
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("update-approval-status")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> UpdateApprovalStatus(ApprovalRequestModel model)
        {
            if (model == null || model.StudentIds == null || model.OfferedCourseIds == null ||
                model.StudentIds.Count != model.OfferedCourseIds.Count || model.StudentIds.Count == 0)
            {
                return BadRequest("Geçersiz veya eksik istek parametreleri.");
            }

            var advisorId = GetActiveUserId();
            using (var transaction = _db.Database.BeginTransaction())
            {
                try
                {
                    int successCount = 0;
                    var failedRecords = new List<string>();

                    for (int i = 0; i < model.StudentIds.Count; i++)
                    {
                        var studentId = model.StudentIds[i];
                        var offeredCourseId = model.OfferedCourseIds[i];

                        var studentCourse = await _db.StudentCourses
                            .Include(sc => sc.Users)
                            .Include(sc => sc.OfferedCourses)
                            .FirstOrDefaultAsync(sc => sc.StudentId == studentId &&
                                                       sc.OfferedCourseId == offeredCourseId &&
                                                       sc.Users.AdvisorId == advisorId);

                        if (studentCourse != null && studentCourse.ApprovalStatus == (int)ApprovalStatus.Bekliyor)
                        {
                            studentCourse.ApprovalStatus = (int)model.NewStatus;
                            studentCourse.UpdatedBy = advisorId.ToString();
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
                            failedRecords.Add($"Öğrenci ID: {studentId}, Ders ID: {offeredCourseId}");
                        }
                    }

                    await _db.SaveChangesAsync();
                    transaction.Commit();

                    var statusText = model.NewStatus == ApprovalStatus.Onaylandı ? "onaylandı" : "reddedildi";
                    var message = $"{successCount} ders kaydı başarıyla {statusText}.";

                    if (failedRecords.Any())
                    {
                        message +=
                            $" {failedRecords.Count} kayıt işlenemedi (ya daha önce işlenmiş ya da yetkiniz dışında).";
                        Logger.Warn(
                            $"Danışman {advisorId} için bazı onay kayıtları işlenemedi. Detaylar: {string.Join(", ", failedRecords)}");
                    }

                    return Ok(new { message });
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Logger.Error("Onay durumu güncellenirken bir hata oluştu.", ex);
                    return InternalServerError(new Exception("Onay durumu güncellenirken bir hata oluştu."));
                }
            }
        }




    }
}