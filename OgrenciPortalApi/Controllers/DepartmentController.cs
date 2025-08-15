using log4net;
using OgrenciPortalApi.Attributes;
using OgrenciPortalApi.Models;
using OgrenciPortalApi.Utils;
using OgrenciPortali.DTOs;
using System;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.IdentityModel.Claims;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace OgrenciPortalApi.Controllers
{
    [RoutePrefix("api/departments")]
    [JwtAuth]
    public class DepartmentController : ApiController
    {
        private readonly ogrenci_portalEntities db = new ogrenci_portalEntities();
        private static readonly ILog Logger = LogManager.GetLogger(typeof(DepartmentController));

        [HttpGet]
        [Route("")]
        public async Task<IHttpActionResult> GetDepartments()
        {
            try
            {
                var departments = await db.Departments.Where(d => !d.IsDeleted)
                    .Select(d => new ListDepartmentsDTO()
                    {
                        DepartmentId = d.DepartmentId,
                        DepartmentName = d.Name,
                        DepartmentCode = d.DepartmentCode,
                        IsActive = d.IsActive
                    })
                    .ToListAsync();
                if (!departments.Any())
                    return NotFound();
                return Ok(departments);
            }
            catch (Exception ex)
            {
                Logger.Error("Departman Listesi Gönderilirken Hata: ", ex);
                return InternalServerError();
            }
        }

        [HttpPost]
        [Route("add")]
        public async Task<IHttpActionResult> GetAddDepartment(AddDepartmentDTO dto)
        {
            try
            {
                if (await db.Departments.AnyAsync(d =>
                        d.Name == dto.DepartmentName || d.DepartmentCode == dto.DepartmentCode))
                    return Conflict();

                var department = new Departments
                {
                    DepartmentId = Guid.NewGuid(),
                    Name = dto.DepartmentName,
                    DepartmentCode = dto.DepartmentCode,
                    IsActive = true,
                    IsDeleted = false,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    CreatedBy = GetActiveUserId(),
                    UpdatedBy = GetActiveUserId(),
                };

                db.Departments.Add(department);
                await db.SaveChangesAsync();
                return Ok("Kullanıcı kaydedildi");
            }
            catch (Exception ex)
            {
                Logger.Error("Departman Ekleme İşlemi Sırasında Hata: ", ex);
                return InternalServerError();
            }
        }

        [HttpGet]
        [Route("edit/{id:guid}")]
        public async Task<IHttpActionResult> GetEditDepartment(Guid id)
        {
            try
            {
                var department = await db.Departments.FindAsync(id);
                if (department == null || department.IsDeleted)
                    return NotFound();
                var dto = new EditDepartmentDTO
                {
                    DepartmentId = department.DepartmentId,
                    DepartmentName = department.Name,
                    DepartmentCode = department.DepartmentCode,
                    IsActive = department.IsActive
                };
                return Ok(dto);
            }
            catch (Exception ex)
            {
                Logger.Error("Departman Düzenleme İşlemi Sırasında Hata: ", ex);
                return InternalServerError();
            }
        }

        [HttpPost]
        [Route("edit")]
        public async Task<IHttpActionResult> PostEditDepartment(EditDepartmentDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                var department = await db.Departments.FindAsync(dto.DepartmentId);
                if (department == null || department.IsDeleted)
                    return NotFound();
                if (await db.Departments.AnyAsync(d =>
                        d.Name == dto.DepartmentName && d.DepartmentId != dto.DepartmentId && d.IsActive &&
                        !d.IsDeleted))
                    return Conflict();

                department.Name = dto.DepartmentName;
                department.DepartmentCode = dto.DepartmentCode;
                department.IsActive = dto.IsActive;
                db.Departments.AddOrUpdate(department);
                await db.SaveChangesAsync();
                return Ok("Bölüm güncellendi");
            }
            catch (Exception ex)
            {
                Logger.Error("Departman Güncelleme İşlemi Sırasında Hata: ", ex);
                return InternalServerError();
            }
        }

        [HttpGet]
        [Route("delete/{id:guid}")]
        public async Task<IHttpActionResult> GetDeleteDepartment(Guid id)
        {
            try
            {
                var department = await db.Departments.FindAsync(id);
                if (department == null || department.IsDeleted)
                    return NotFound();
                department.IsDeleted = true;
                department.UpdatedAt = DateTime.Now;
                department.UpdatedBy = GetActiveUserId();
                db.Departments.AddOrUpdate(department);
                await db.SaveChangesAsync();
                return Ok("Bölüm silindi");
            }
            catch (Exception ex)
            {
                Logger.Error("Departman Silme İşlemi Sırasında Hata: ", ex);
                return InternalServerError();
            }
        }


        public string GetActiveUserId()
        {
            return TokenManager.GetPrincipal(Request.Headers.Authorization.Parameter)
                .FindFirst(ClaimTypes.NameIdentifier).Value;
        }
    }
}