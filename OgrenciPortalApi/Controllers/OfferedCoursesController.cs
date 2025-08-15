using log4net;
using OgrenciPortalApi.Models;
using OgrenciPortalApi.Utils;
using OgrenciPortali.DTOs;
using OgrenciPortali.Models;
using System;
using System.Data.Entity;
using System.IdentityModel.Claims;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Mvc;
using OgrenciPortalApi.Attributes;

namespace OgrenciPortalApi.Controllers
{
    [JwtAuth]
    [System.Web.Http.RoutePrefix("api/offered")]
    public class OfferedCoursesController : ApiController
    {
        private readonly ogrenci_portalEntities db = new ogrenci_portalEntities();
        private static readonly ILog Logger = LogManager.GetLogger(typeof(DepartmentController));


        [System.Web.Http.HttpGet]
        [System.Web.Http.Route]
        public async Task<IHttpActionResult> GetOfferedCourses()
        {
            try
            {
                var asd = await db.OfferedCourses
                    .Include(o => o.Courses.Departments)
                    .Include(o => o.Semesters)
                    .Include(o => o.Users)
                    .Where(o => o.Users.UserId == o.TeacherId && !o.IsDeleted).Select(o => new ListOfferedCoursesDTO
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
                if (!asd.Any())
                {
                    Logger.Warn("Ders listesinde ders bulunamadı!");
                    return NotFound();
                }

                Logger.Info("Ders Listesi başarıyla çekildi.");
                return Ok(asd);
            }
            catch (Exception ex)
            {
                Logger.Error("Dönem dersleri çekilirken hata oluştu", ex);
                return InternalServerError(ex);
            }
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("add")]
        public async Task<IHttpActionResult> GetAddOfferedCourses()
        {
            var dto = new AddOfferedCourseDTO
            {
                AdvisorList = db.Users
                    .Where(u => u.Role == (int)Roles.Danışman && !u.IsDeleted)
                    .Select<Users, SelectListItem>(u => new SelectListItem
                        { Value = u.UserId.ToString(), Text = u.Name + " " + u.Surname }),
                SemesterList = db.Semesters
                    .Select<Semesters, SelectListItem>(s => new SelectListItem
                        { Value = s.SemesterId.ToString(), Text = s.SemesterName }),
                CourseList = db.Courses.Select<Courses, SelectListItem>(c => new SelectListItem()
                    { Value = c.CourseId.ToString(), Text = c.CourseName }),
                DaysOfWeek = Enum.GetValues(typeof(DaysOfWeek))
                    .Cast<DaysOfWeek>()
                    .Select(d => new SelectListItem
                    {
                        Value = ((int)d).ToString(),
                        Text = d.ToString()
                    }),
            };
            var a = dto;
            return Ok(dto);
        }


        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("add")]
        public async Task<IHttpActionResult> PostAddOfferedCourses(AddOfferedCourseDTO dto)
        {
            if (!ModelState.IsValid)
            {
                Logger.Warn("Ders ekleme işlemi sırasında model geçersiz.");
                return BadRequest(ModelState);
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
                CreatedBy = GetActiveUserId(),
                UpdatedAt = DateTime.Now,
                UpdatedBy = GetActiveUserId(),
                CurrentUserCount = 0
            };
            if (await db.OfferedCourses.AnyAsync(u =>
                    u.CourseId == dto.CourseId && u.SemesterId == dto.SemesterId && u.TeacherId == dto.AdvisorId))
                return BadRequest("Bu ders bu dönemde başka bir akademisyen tarafından verilmektedir.");
            if (await db.OfferedCourses.AnyAsync(u =>
                    u.CourseId == offeredCourse.CourseId && u.SemesterId == offeredCourse.SemesterId))
                return BadRequest("Bu ders bu döneme zaten eklenmiş durumda. Tekrar ekleyemezsiniz.");

            db.OfferedCourses.Add(offeredCourse);
            await db.SaveChangesAsync();
            Logger.Info("Yeni ders başarıyla eklendi.");
            return Ok(new { Message = "Ders başarıyla eklendi." });
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("edit/{id:guid}")]
        public async Task<IHttpActionResult> GetEditOfferedCourses(Guid id)
        {
            try
            {
                var offeredCourseInDb = await db.OfferedCourses.FindAsync(id);
                if (offeredCourseInDb == null)
                {
                    Logger.Warn("Ders bulunamadı.");
                    return NotFound();
                }

                if (offeredCourseInDb.IsDeleted)
                {
                    Logger.Warn("Silinmiş bir ders düzenlenmeye çalışıldı.");
                    return BadRequest("Bu ders silinmiş, düzenleme yapılamaz.");
                }

                var course = await db.Courses.FindAsync(offeredCourseInDb.CourseId);
                if (course == null)
                {
                    Logger.Warn("Ders kodu bulunamadı.");
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
                    AdvisorList = db.Users
                        .Where(u => u.Role == (int)Roles.Danışman && !u.IsDeleted)
                        .Select<Users, SelectListItem>(u => new SelectListItem
                            { Value = u.UserId.ToString(), Text = u.Name + " " + u.Surname }),
                    SemesterList = db.Semesters
                        .Select<Semesters, SelectListItem>(s => new SelectListItem
                            { Value = s.SemesterId.ToString(), Text = s.SemesterName }),
                    CourseList = db.Courses.Select<Courses, SelectListItem>(c => new SelectListItem()
                        { Value = c.CourseId.ToString(), Text = c.CourseName }),
                    DaysOfWeek = Enum.GetValues(typeof(DaysOfWeek))
                        .Cast<DaysOfWeek>()
                        .Select(d => new SelectListItem
                        {
                            Value = ((int)d).ToString(),
                            Text = d.ToString()
                        }),
                };
                return Ok(dto);
            }
            catch (Exception ex)
            {
                Logger.Error("Ders düzenleme sayfası yüklenirken hata oluştu", ex);
                return InternalServerError(ex);
            }
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("edit")]
        public async Task<IHttpActionResult> PostEditOfferedCourses(EditOfferedCoursesDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    Logger.Warn("Ders düzenleme işlemi sırasında model geçersiz.");
                    return BadRequest(ModelState);
                }

                var offeredCourseInDb = await db.OfferedCourses.FindAsync(dto.OfferedCourseId);
                if (offeredCourseInDb == null)
                {
                    Logger.Warn("Ders bulunamadı.");
                    return NotFound();
                }

                if (offeredCourseInDb.IsDeleted)
                {
                    Logger.Warn("Silinmiş bir ders düzenlenmeye çalışıldı.");
                    return BadRequest("Bu ders silinmiş, düzenleme yapılamaz.");
                }

                offeredCourseInDb.CourseId = dto.CourseId;
                offeredCourseInDb.SemesterId = dto.SemesterId;
                offeredCourseInDb.TeacherId = dto.AdvisorId;
                offeredCourseInDb.Quota = dto.Quota;
                offeredCourseInDb.StartTime = dto.StartTime;
                offeredCourseInDb.EndTime = dto.EndTime;
                offeredCourseInDb.DayOfWeek = (int)dto.DayOfWeek;
                offeredCourseInDb.UpdatedAt = DateTime.Now;
                offeredCourseInDb.UpdatedBy = GetActiveUserId();
                await db.SaveChangesAsync();
                Logger.Info("Ders başarıyla güncellendi.");
                return Ok("Ders başarıyla güncellendi.");
            }
            catch (Exception ex)
            {
                Logger.Error("Ders düzenleme işlemi sırasında hata oluştu", ex);
                return InternalServerError(ex);
            }
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("delete/{id:guid}")]
        public async Task<IHttpActionResult> DeleteOfferedCourse(Guid id)
        {
            try
            {
                var offeredCourseInDb = await db.OfferedCourses.FindAsync(id);
                if (offeredCourseInDb == null)
                {
                    Logger.Warn("Ders bulunamadı.");
                    return NotFound();
                }

                if (offeredCourseInDb.IsDeleted)
                {
                    Logger.Warn("Silinmiş bir ders tekrar silinmeye çalışıldı.");
                    return BadRequest("Bu ders zaten silinmiş.");
                }

                if (offeredCourseInDb.CurrentUserCount > 0)
                {
                    Logger.Warn("Ders silinmeye çalışıldı ancak kayıtlı öğrenciler var.");
                    return BadRequest("Bu dersin kayıtlı öğrencileri var, silinemez.");
                }

                offeredCourseInDb.IsDeleted = true;
                offeredCourseInDb.UpdatedAt = DateTime.Now;
                offeredCourseInDb.UpdatedBy = GetActiveUserId();
                offeredCourseInDb.DeletedAt = DateTime.Now;
                offeredCourseInDb.DeletedBy = GetActiveUserId();
                await db.SaveChangesAsync();
                Logger.Info("Ders başarıyla silindi.");
                return Ok(new { Message = "Ders başarıyla silindi." });
            }
            catch (Exception ex)
            {
                Logger.Error("Ders silme işlemi sırasında hata oluştu", ex);
                return InternalServerError(ex);
            }
        }

        private string GetActiveUserId()
        {
            return TokenManager.GetPrincipal(Request.Headers.Authorization.Parameter)
                .FindFirst(ClaimTypes.NameIdentifier).Value;
        }
    }
}