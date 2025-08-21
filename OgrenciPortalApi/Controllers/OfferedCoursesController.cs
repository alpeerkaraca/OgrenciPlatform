using log4net;
using OgrenciPortalApi.Models;
using Shared.DTO;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Mvc;
using Shared.Enums;

namespace OgrenciPortalApi.Controllers
{
    [System.Web.Http.Authorize(Roles = nameof(Roles.Admin))]
    [System.Web.Http.RoutePrefix("api/offered")]
    public class OfferedCoursesController : BaseApiController
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(OfferedCoursesController));

        /// <summary>
        /// Sistemde açılmış olan tüm dersleri listeler.
        /// </summary>
        /// <returns>Açılmış derslerin listesini içeren bir HTTP yanıtı döner.</returns>
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("")]
        [ResponseType(typeof(ListOfferedCoursesDTO))]
        public async Task<IHttpActionResult> GetOfferedCourses()
        {
            try
            {
                var offeredCourses = await _db.OfferedCourses
                    .Include(o => o.Courses.Departments)
                    .Include(o => o.Semesters)
                    .Include(o => o.Users)
                    .Where(o => !o.IsDeleted).Select(o => new ListOfferedCoursesDTO
                    {
                        OfferedCourseId = o.Id,
                        DepartmentName = o.Courses.Departments.Name,
                        CourseCode = o.Courses.CourseCode,
                        CourseName = o.Courses.CourseName,
                        SemesterName = o.Semesters.SemesterName,
                        TeacherFullName = o.Users.Name + " " + o.Users.Surname,
                        Capacity = o.Quota,
                        EnrolledCount = o.CurrentUserCount,
                        StartTime = o.StartTime,
                        EndTime = o.EndTime,
                        DayOfWeek = (DaysOfWeek)o.DayOfWeek,
                    }).ToListAsync();

                Logger.Info("Açılan ders listesi başarıyla çekildi.");
                return Ok(offeredCourses);
            }
            catch (Exception ex)
            {
                Logger.Error("Açılan dersler listesi alınırken hata oluştu.", ex);
                return InternalServerError(new Exception("Açılan dersler listesi alınırken bir hata oluştu."));
            }
        }

        /// <summary>
        /// Yeni bir ders açma sayfası için gerekli verileri (danışman, dönem, ders listeleri vb.) getirir.
        /// </summary>
        /// <returns>Gerekli listeleri içeren bir HTTP yanıtı döner.</returns>
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("add")]
        [ResponseType(typeof(AddOfferedCourseDTO))]
        public async Task<IHttpActionResult> GetAddOfferedCourses()
        {
            try
            {
                var dto = new AddOfferedCourseDTO
                {
                    AdvisorList = _db.Users
                        .Where(u => u.Role == (int)Roles.Danışman && !u.IsDeleted && u.IsActive)
                        .Select(u => new SelectListItem
                            { Value = u.UserId.ToString(), Text = u.Name + " " + u.Surname }),
                    SemesterList = _db.Semesters
                        .Where(s => !s.IsDeleted && s.IsActive)
                        .Select(s => new SelectListItem { Value = s.SemesterId.ToString(), Text = s.SemesterName }),
                    CourseList = _db.Courses
                        .Where(c => !c.IsDeleted)
                        .Select(c => new SelectListItem() { Value = c.CourseId.ToString(), Text = c.CourseName }),
                    DaysOfWeek = Enum.GetValues(typeof(DaysOfWeek))
                        .Cast<DaysOfWeek>()
                        .Select(d => new SelectListItem { Value = ((int)d).ToString(), Text = d.ToString() }),
                };
                return Ok(dto);
            }
            catch (Exception ex)
            {
                Logger.Error("Ders açma sayfası için veriler alınırken hata oluştu.", ex);
                return InternalServerError(new Exception("Ders açma sayfası için veriler alınırken bir hata oluştu."));
            }
        }

        /// <summary>
        /// Sisteme yeni bir açılan ders ekler.
        /// </summary>
        /// <param name="dto">Açılacak dersin bilgilerini içeren DTO.</param>
        /// <returns>Oluşturma durumunu bildiren bir HTTP yanıtı döner.</returns>
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("add")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> AddOfferedCourse(AddOfferedCourseDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Form verisi geçersiz.");
            }

            try
            {
                if (await _db.OfferedCourses.AnyAsync(u =>
                        u.CourseId == dto.CourseId && u.SemesterId == dto.SemesterId && !u.IsDeleted))
                {
                    return BadRequest("Bu ders bu döneme zaten eklenmiş durumda. Tekrar ekleyemezsiniz.");
                }

                var offeredCourse = new OfferedCourses
                {
                    Id = Guid.NewGuid(),
                    CourseId = dto.CourseId,
                    SemesterId = dto.SemesterId,
                    TeacherId = dto.AdvisorId,
                    Quota = dto.Quota,
                    StartTime = dto.StartTime,
                    EndTime = dto.EndTime,
                    DayOfWeek = (int)dto.DayOfWeek,
                    CreatedAt = DateTime.Now,
                    CreatedBy = GetActiveUserIdString(),
                    UpdatedAt = DateTime.Now,
                    UpdatedBy = GetActiveUserIdString(),
                    CurrentUserCount = 0
                };

                _db.OfferedCourses.Add(offeredCourse);
                await _db.SaveChangesAsync();
                Logger.Info("Yeni açılan ders başarıyla eklendi.");

                return Ok(new { Message = "Dönem dersi başarıyla eklendi" });
            }
            catch (Exception ex)
            {
                Logger.Error("Yeni ders açılırken bir hata oluştu.", ex);
                return InternalServerError(new Exception("Ders açılırken bir hata oluştu."));
            }
        }

        /// <summary>
        /// ID'si belirtilen açılmış dersin düzenleme bilgilerini getirir.
        /// </summary>
        /// <param name="id">Düzenlenecek olan açılmış dersin ID'si.</param>
        /// <returns>Açılmış dersin düzenleme bilgilerini içeren bir HTTP yanıtı döner.</returns>
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("edit/{id:guid}")]
        [ResponseType(typeof(EditOfferedCoursesDTO))]
        public async Task<IHttpActionResult> GetEditOfferedCourse([FromUri]Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest("Geçersiz ID.");
            }

            try
            {
                var offeredCourseInDb = await _db.OfferedCourses.FindAsync(id);
                if (offeredCourseInDb == null || offeredCourseInDb.IsDeleted)
                {
                    return NotFound();
                }

                var course = await _db.Courses.FindAsync(offeredCourseInDb.CourseId);
                if (course == null)
                {
                    return NotFound();
                }

                var dto = new EditOfferedCoursesDTO
                {
                    OfferedCourseId = id,
                    CourseId = offeredCourseInDb.CourseId,
                    DayOfWeek = offeredCourseInDb.DayOfWeek,
                    AdvisorId = offeredCourseInDb.TeacherId,
                    SemesterId = offeredCourseInDb.SemesterId,
                    StartTime = offeredCourseInDb.StartTime,
                    EndTime = offeredCourseInDb.EndTime,
                    Quota = offeredCourseInDb.Quota,
                    CourseCode = course.CourseCode,
                    AdvisorList = _db.Users.Where(u => u.Role == (int)Roles.Danışman && !u.IsDeleted).Select(u =>
                        new SelectListItem { Value = u.UserId.ToString(), Text = u.Name + " " + u.Surname }),
                    SemesterList = _db.Semesters.Select(s => new SelectListItem
                        { Value = s.SemesterId.ToString(), Text = s.SemesterName }),
                    CourseList = _db.Courses.Select(c => new SelectListItem()
                        { Value = c.CourseId.ToString(), Text = c.CourseName }),
                    DaysOfWeek = Enum.GetValues(typeof(DaysOfWeek)).Cast<DaysOfWeek>().Select(d => new SelectListItem
                        { Value = ((int)d).ToString(), Text = d.ToString() }),
                };
                return Ok(dto);
            }
            catch (Exception ex)
            {
                Logger.Error($"ID'si {id} olan açılmış dersin bilgileri alınırken hata oluştu.", ex);
                return InternalServerError(new Exception("Açılan ders bilgileri alınırken bir hata oluştu."));
            }
        }

        /// <summary>
        /// Mevcut bir açılmış dersin bilgilerini günceller.
        /// </summary>
        /// <param name="dto">Güncellenecek açılmış dersin yeni bilgilerini içeren DTO.</param>
        /// <returns>Güncelleme durumunu bildiren bir HTTP yanıtı döner.</returns>
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("edit")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> EditOfferedCourse(EditOfferedCoursesDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var offeredCourseInDb = await _db.OfferedCourses.FindAsync(dto.OfferedCourseId);
                if (offeredCourseInDb == null || offeredCourseInDb.IsDeleted)
                {
                    return NotFound();
                }

                offeredCourseInDb.CourseId = dto.CourseId;
                offeredCourseInDb.SemesterId = dto.SemesterId;
                offeredCourseInDb.TeacherId = dto.AdvisorId;
                offeredCourseInDb.Quota = dto.Quota;
                offeredCourseInDb.StartTime = dto.StartTime;
                offeredCourseInDb.EndTime = dto.EndTime;
                offeredCourseInDb.DayOfWeek = dto.DayOfWeek;
                offeredCourseInDb.UpdatedAt = DateTime.Now;
                offeredCourseInDb.UpdatedBy = GetActiveUserIdString();
                await _db.SaveChangesAsync();
                Logger.Info($"ID'si {dto.OfferedCourseId} olan açılmış ders başarıyla güncellendi.");
                return Ok(new { Message = "Açılan ders başarıyla güncellendi." });
            }
            catch (Exception ex)
            {
                Logger.Error($"ID'si {dto.OfferedCourseId} olan açılmış ders güncellenirken hata oluştu.", ex);
                return InternalServerError(new Exception("Açılan ders güncellenirken bir hata oluştu."));
            }
        }

        /// <summary>
        /// Belirtilen ID'ye sahip açılmış dersi siler (soft delete).
        /// </summary>
        /// <param name="id">Silinecek açılmış dersin ID'si.</param>
        /// <returns>Silme işleminin durumunu bildiren bir HTTP yanıtı döner.</returns>
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("delete/{id:guid}")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> DeleteOfferedCourse(Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest("Geçersiz ID.");
            }

            try
            {
                var offeredCourseInDb = await _db.OfferedCourses.FindAsync(id);
                if (offeredCourseInDb == null)
                {
                    return NotFound();
                }

                if (offeredCourseInDb.IsDeleted)
                {
                    return BadRequest("Bu ders zaten silinmiş.");
                }

                if (offeredCourseInDb.CurrentUserCount > 0)
                {
                    return BadRequest("Bu derse kayıtlı öğrenciler bulunduğu için silinemez.");
                }

                offeredCourseInDb.IsDeleted = true;
                offeredCourseInDb.UpdatedAt = DateTime.Now;
                offeredCourseInDb.UpdatedBy = GetActiveUserIdString();
                offeredCourseInDb.DeletedAt = DateTime.Now;
                offeredCourseInDb.DeletedBy = GetActiveUserIdString();
                await _db.SaveChangesAsync();
                Logger.Info($"ID'si {id} olan açılmış ders başarıyla silindi.");
                return Ok(new { Message = "Açılan ders başarıyla silindi." });
            }
            catch (Exception ex)
            {
                Logger.Error($"ID'si {id} olan açılmış ders silinirken hata oluştu.", ex);
                return InternalServerError(new Exception("Açılan ders silinirken bir hata oluştu."));
            }
        }
    }
}