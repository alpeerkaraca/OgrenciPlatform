using log4net;
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
    [Authorize(Roles = nameof(Roles.Admin))]
    [RoutePrefix("api/courses")]
    public class CourseController : BaseApiController
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(CourseController));

        /// <summary>
        /// Sistemdeki tüm dersleri listeler.
        /// </summary>
        /// <returns>Ders listesini içeren bir HTTP yanıtı döner.</returns>
        [HttpGet]
        [Route("list")]
        [ResponseType(typeof(List<ListCoursesDTO>))]
        public async Task<IHttpActionResult> GetCourses()
        {
            try
            {
                var courses = await _db.Courses.Where(c => !c.IsDeleted)
                    .Include(c => c.Departments)
                    .Select(c => new ListCoursesDTO()
                    {
                        CourseCode = c.CourseCode,
                        CourseId = c.CourseId,
                        CourseName = c.CourseName,
                        Credits = c.Credits,
                        DepartmentName = c.Departments.Name
                    }).ToListAsync();
                return Ok(courses);
            }
            catch (Exception ex)
            {
                Logger.Error("Ders listesi alınırken hata oluştu.", ex);
                return InternalServerError(new Exception("Ders listesi alınırken bir hata oluştu."));
            }
        }

        /// <summary>
        /// Yeni ders ekleme sayfası için gerekli verileri (örn. departman listesi) getirir.
        /// </summary>
        /// <returns>Departman listesini içeren bir HTTP yanıtı döner.</returns>
        [HttpGet]
        [Route("add")]
        [ResponseType(typeof(AddCourseDTO))]
        public async Task<IHttpActionResult> GetAddCourse()
        {
            try
            {
                var depList = await FillDepartmentsList();
                var dto = new AddCourseDTO()
                {
                    DepartmentsList = depList
                };
                return Ok(dto);
            }
            catch (Exception ex)
            {
                Logger.Error("Ders ekleme sayfası için veriler alınırken hata oluştu.", ex);
                return InternalServerError(
                    new Exception("Ders ekleme sayfası için veriler alınırken bir hata oluştu."));
            }
        }

        /// <summary>
        /// Sisteme yeni bir ders ekler.
        /// </summary>
        /// <param name="dto">Eklenecek dersin bilgilerini içeren DTO.</param>
        /// <returns>Oluşturma durumunu bildiren bir HTTP yanıtı döner.</returns>
        [HttpPost]
        [Route("add")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> AddCourse(AddCourseDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                if (await _db.Courses.AnyAsync(c => c.CourseCode == dto.CourseCode && !c.IsDeleted) ||
                    await _db.Courses.AnyAsync(c => c.CourseName == dto.CourseName && !c.IsDeleted))
                {
                    return BadRequest("Aynı isim veya ders koduna sahip ders bulundu.");
                }

                if (dto.Credits > 10)
                    return BadRequest("Ders kredisi 10dan fazla olamaz.");
                if (!await _db.Departments.AnyAsync(d => d.DepartmentId == dto.DepartmentId))
                    return BadRequest("Seçilen bölüm kayıtlar arasında bulunamadı.");

                var course = new Courses()
                {
                    CourseId = Guid.NewGuid(),
                    CourseName = dto.CourseName,
                    CourseCode = dto.CourseCode,
                    Credits = dto.Credits,
                    DepartmentId = dto.DepartmentId,
                    CreatedAt = DateTime.Now,
                    CreatedBy = GetActiveUserIdString(),
                    UpdatedAt = DateTime.Now,
                    UpdatedBy = GetActiveUserIdString(),
                };

                _db.Courses.Add(course);
                await _db.SaveChangesAsync();
                Logger.Info($"Ders Eklendi: {course.CourseName}");
                return Ok(new { Message = "Ders başarıyla eklendi." });
            }
            catch (Exception ex)
            {
                Logger.Error("Ders eklenirken bir hata oluştu.", ex);
                return InternalServerError(new Exception("Ders eklenirken bir hata oluştu."));
            }
        }

        /// <summary>
        /// ID'si belirtilen dersin düzenleme bilgilerini getirir.
        /// </summary>
        /// <param name="id">Düzenlenecek dersin ID'si.</param>
        /// <returns>Dersin düzenleme bilgilerini içeren bir HTTP yanıtı döner.</returns>
        [HttpGet]
        [Route("edit/{id:guid}")]
        [ResponseType(typeof(EditCourseDTO))]
        public async Task<IHttpActionResult> GetEditCourse(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest("Geçersiz ders ID'si.");

            try
            {
                var course = await _db.Courses.FirstOrDefaultAsync(c => c.CourseId == id && !c.IsDeleted);
                if (course == null)
                    return NotFound();

                var depList = await FillDepartmentsList();
                var dto = new EditCourseDTO
                {
                    CourseId = course.CourseId,
                    CourseName = course.CourseName,
                    CourseCode = course.CourseCode,
                    Credits = course.Credits,
                    DepartmentId = course.DepartmentId,
                    DepartmentsList = depList
                };
                return Ok(dto);
            }
            catch (Exception ex)
            {
                Logger.Error($"ID'si {id} olan dersin bilgileri alınırken hata oluştu.", ex);
                return InternalServerError(new Exception("Ders bilgileri alınırken bir hata oluştu."));
            }
        }

        /// <summary>
        /// Mevcut bir dersin bilgilerini günceller.
        /// </summary>
        /// <param name="dto">Güncellenecek dersin yeni bilgilerini içeren DTO.</param>
        /// <returns>Güncelleme durumunu bildiren bir HTTP yanıtı döner.</returns>
        [HttpPost]
        [Route("edit")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> EditCourse(EditCourseDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                if (await _db.Courses.AnyAsync(c =>
                        (c.CourseCode.ToLower() == dto.CourseCode.ToLower() ||
                         c.CourseName.ToLower() == dto.CourseName.ToLower())
                        && c.CourseId != dto.CourseId && !c.IsDeleted))
                {
                    return Conflict();
                }

                var courseInDb = await _db.Courses.FindAsync(dto.CourseId);
                if (courseInDb == null || courseInDb.IsDeleted)
                    return NotFound();

                courseInDb.CourseCode = dto.CourseCode;
                courseInDb.CourseName = dto.CourseName;
                courseInDb.Credits = dto.Credits;
                courseInDb.DepartmentId = dto.DepartmentId;
                courseInDb.UpdatedBy = GetActiveUserIdString();
                courseInDb.UpdatedAt = DateTime.Now;
                await _db.SaveChangesAsync();

                Logger.Info($"Ders Güncellendi: {courseInDb.CourseName}");
                return Ok(new { Message = "Ders başarıyla güncellendi." });
            }
            catch (Exception ex)
            {
                Logger.Error($"ID'si {dto.CourseId} olan ders güncellenirken hata oluştu.", ex);
                return InternalServerError(new Exception("Ders güncellenirken bir hata oluştu."));
            }
        }

        /// <summary>
        /// Belirtilen ID'ye sahip dersi siler (soft delete).
        /// </summary>
        /// <param name="id">Silinecek dersin ID'si.</param>
        /// <returns>Silme işleminin durumunu bildiren bir HTTP yanıtı döner.</returns>
        [HttpPost]
        [Route("delete/{id:guid}")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> DeleteCourse(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest("Geçersiz ders ID'si.");

            try
            {
                var course = await _db.Courses.FirstOrDefaultAsync(c => c.CourseId == id && !c.IsDeleted);
                if (course == null)
                    return NotFound();

                course.IsDeleted = true;
                course.UpdatedAt = DateTime.Now;
                course.UpdatedBy = GetActiveUserIdString();
                await _db.SaveChangesAsync();
                Logger.Info($"Ders Silindi: {course.CourseName}");
                return Ok(new { Message = "Ders başarıyla silindi." });
            }
            catch (Exception ex)
            {
                Logger.Error($"ID'si {id} olan ders silinirken hata oluştu.", ex);
                return InternalServerError(new Exception("Ders silinirken bir hata oluştu."));
            }
        }

        private async Task<List<DepartmentSelectionDTO>> FillDepartmentsList()
        {
            return await _db.Departments
                .Where(d => !d.IsDeleted && d.IsActive)
                .Select(d => new DepartmentSelectionDTO()
                {
                    DepartmentId = d.DepartmentId,
                    DepartmentName = d.Name
                }).ToListAsync();
        }
    }
}