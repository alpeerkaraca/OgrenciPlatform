using log4net;
using OgrenciPortalApi.Attributes;
using OgrenciPortalApi.Models;
using OgrenciPortalApi.Utils;
using OgrenciPortali.DTOs;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IdentityModel.Claims;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace OgrenciPortalApi.Controllers
{
    [RoutePrefix("api/courses")]
    public class CourseController : ApiController
    {
        private readonly ogrenci_portalEntities _db = new ogrenci_portalEntities();
        private static readonly ILog Logger = LogManager.GetLogger(typeof(CourseController));

        [HttpGet]
        //[JwtAuth]
        [Route("list")]
        public async Task<IHttpActionResult> GetCourses()
        {
            List<ListCoursesDTO> courses = (await _db.Courses.Where(c => !c.IsDeleted)
                .Include(c => c.Departments)
                .Select(c => new ListCoursesDTO()
                {
                    CourseCode = c.CourseCode,
                    CourseId = c.CourseId,
                    CourseName = c.CourseName,
                    Credits = c.Credits,
                    DepartmentName = c.Departments.Name
                }).ToListAsync());
            return Ok(courses);
        }

        [HttpGet]
        [JwtAuth]
        [Route("add")]
        public async Task<IHttpActionResult> GetAddCourse()
        {
            var depList = await FillDepartmentsList();

            var dto = new AddCourseDTO()
            {
                DepartmentsList = depList
            };

            return Ok(dto);
        }

        [HttpPost]
        [JwtAuth]
        [Route("add")]
        public async Task<IHttpActionResult> PostAddCourse(AddCourseDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    Logger.Error("Ders ekleme işlemi sırasında model geçersiz.");
                    return BadRequest(ModelState);
                }

                if (await _db.Courses.AnyAsync(c => c.CourseCode == dto.CourseCode) ||
                    await _db.Courses.AnyAsync(c => c.CourseName == dto.CourseName))
                    return Conflict();

                var course = new Courses()
                {
                    CourseId = Guid.NewGuid(),
                    CourseName = dto.CourseName,
                    CourseCode = dto.CourseCode,
                    Credits = dto.Credits,
                    DepartmentId = dto.DepartmentId,
                    CreatedAt = DateTime.Now,
                    CreatedBy = GetActiveUserId(),
                    UpdatedAt = DateTime.Now,
                    UpdatedBy = GetActiveUserId(),
                };

                _db.Courses.Add(course);
                await _db.SaveChangesAsync();
                Logger.Info($"Ders Eklendi: {course.CourseName}");
                return Ok(new { Message = "Ders Eklendi" });
            }
            catch (Exception e)
            {
                Logger.Error("Ders ekleme işlemi sırasında bir hata oluştu: ", e);
                return InternalServerError(e);
            }
        }

        [HttpGet]
        [JwtAuth]
        [Route("edit/{id:guid}")]
        public async Task<IHttpActionResult> GetEditCourse(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest("Geçersiz ders ID'si.");
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

        [HttpPost]
        [JwtAuth]
        [Route("edit")]
        public async Task<IHttpActionResult> PostEditCourse(EditCourseDTO dto)
        {
            if (await _db.Courses.AnyAsync(c =>
                    c.CourseCode.ToLower() == dto.CourseCode.ToLower() ||
                    c.CourseName.ToLower() == dto.CourseName.ToLower()))
                return Conflict();

            var courseInDb = await _db.Courses.FindAsync(dto.CourseId);

            if (courseInDb == null || courseInDb.IsDeleted)
                return NotFound();

            courseInDb.CourseCode = dto.CourseCode;
            courseInDb.CourseName = dto.CourseName;
            courseInDb.Credits = dto.Credits;
            courseInDb.UpdatedBy = GetActiveUserId();
            courseInDb.UpdatedAt = DateTime.Now;
            await _db.SaveChangesAsync();
            return Ok("Kullanıcı Başarıyla Güncellendi");
        }

        [HttpGet]
        [JwtAuth]
        [Route("delete/{id:guid}")]
        public async Task<IHttpActionResult> GetDeleteCourse(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest("Geçersiz ders ID'si.");
            var course = await _db.Courses.FirstOrDefaultAsync(c => c.CourseId == id && !c.IsDeleted);
            if (course == null)
                return NotFound();
            course.IsDeleted = true;
            course.UpdatedAt = DateTime.Now;
            course.UpdatedBy = GetActiveUserId();
            await _db.SaveChangesAsync();
            Logger.Info($"Ders Silindi: {course.CourseName}");
            return Ok(new { Message = "Ders Silindi" });
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