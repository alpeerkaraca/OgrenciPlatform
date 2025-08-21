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
    [RoutePrefix("api/departments")]
    public class DepartmentController : BaseApiController
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(DepartmentController));

        /// <summary>
        /// Tüm aktif departmanları listeler.
        /// </summary>
        /// <returns>Departman listesini içeren bir HTTP yanıtı döner.</returns>
        [HttpGet]
        [Route("")]
        [ResponseType(typeof(List<ListDepartmentsDTO>))]
        public async Task<IHttpActionResult> GetDepartments()
        {
            try
            {
                var departments = await _db.Departments.Where(d => !d.IsDeleted)
                    .Select(d => new ListDepartmentsDTO()
                    {
                        DepartmentId = d.DepartmentId,
                        DepartmentName = d.Name,
                        DepartmentCode = d.DepartmentCode,
                        IsActive = d.IsActive
                    })
                    .ToListAsync();

                return Ok(departments);
            }
            catch (Exception ex)
            {
                Logger.Error("Departman listesi alınırken hata oluştu.", ex);
                return InternalServerError(new Exception("Departman listesi alınırken bir hata oluştu."));
            }
        }

        /// <summary>
        /// Yeni bir departman ekler.
        /// </summary>
        /// <param name="dto">Eklenecek departmanın bilgilerini içeren DTO.</param>
        /// <returns>Oluşturma durumunu bildiren bir HTTP yanıtı döner.</returns>
        [HttpPost]
        [Route("add")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> AddDepartment(AddDepartmentDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                if (await _db.Departments.AnyAsync(d =>
                        (d.Name == dto.DepartmentName || d.DepartmentCode == dto.DepartmentCode) && !d.IsDeleted))
                {
                    return Conflict();
                }

                var department = new Departments
                {
                    DepartmentId = Guid.NewGuid(),
                    Name = dto.DepartmentName,
                    DepartmentCode = dto.DepartmentCode,
                    IsActive = true,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    CreatedBy = GetActiveUserIdString(),
                    UpdatedBy = GetActiveUserIdString(),
                };

                _db.Departments.Add(department);
                await _db.SaveChangesAsync();

                Logger.Info($"Departman eklendi: {department.Name}");
                return CreatedAtRoute("DefaultApi", new { id = department.DepartmentId },
                    new { Message = "Departman başarıyla eklendi." });
            }
            catch (Exception ex)
            {
                Logger.Error("Departman eklenirken hata oluştu.", ex);
                return InternalServerError(new Exception("Departman eklenirken bir hata oluştu."));
            }
        }

        /// <summary>
        /// Belirtilen ID'ye sahip departman bilgilerini getirir.
        /// </summary>
        /// <param name="id">Düzenlenecek departmanın ID'si.</param>
        /// <returns>Departman bilgilerini içeren bir HTTP yanıtı döner.</returns>
        [HttpGet]
        [Route("edit/{id:guid}")]
        [ResponseType(typeof(EditDepartmentDTO))]
        public async Task<IHttpActionResult> GetDepartment(Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest("Geçersiz departman ID'si.");
            }

            try
            {
                var department = await _db.Departments.FindAsync(id);
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
                Logger.Error($"ID'si {id} olan departman düzenleme için alınırken hata oluştu.", ex);
                return InternalServerError(new Exception("Departman bilgileri alınırken bir hata oluştu."));
            }
        }

        /// <summary>
        /// Mevcut bir departmanın bilgilerini günceller.
        /// </summary>
        /// <param name="dto">Güncellenecek departmanın yeni bilgilerini içeren DTO.</param>
        /// <returns>Güncelleme durumunu bildiren bir HTTP yanıtı döner.</returns>
        [HttpPut]
        [Route("edit")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> EditDepartment(EditDepartmentDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var department = await _db.Departments.FindAsync(dto.DepartmentId);
                if (department == null || department.IsDeleted)
                    return NotFound();

                if (await _db.Departments.AnyAsync(d =>
                        (d.Name == dto.DepartmentName || d.DepartmentCode == dto.DepartmentCode) &&
                        d.DepartmentId != dto.DepartmentId && !d.IsDeleted))
                {
                    return Conflict();
                }

                department.Name = dto.DepartmentName;
                department.DepartmentCode = dto.DepartmentCode;
                department.IsActive = dto.IsActive;
                department.UpdatedAt = DateTime.Now;
                department.UpdatedBy = GetActiveUserIdString();

                await _db.SaveChangesAsync();
                Logger.Info($"Departman güncellendi: {department.Name}");
                return Ok(new { Message = "Bölüm başarıyla güncellendi." });
            }
            catch (Exception ex)
            {
                Logger.Error($"ID'si {dto.DepartmentId} olan departman güncellenirken hata oluştu.", ex);
                return InternalServerError(new Exception("Departman güncellenirken bir hata oluştu."));
            }
        }

        /// <summary>
        /// Belirtilen ID'ye sahip departmanı siler (soft delete).
        /// </summary>
        /// <param name="id">Silinecek departmanın ID'si.</param>
        /// <returns>Silme işleminin durumunu bildiren bir HTTP yanıtı döner.</returns>
        [HttpDelete]
        [Route("delete/{id:guid}")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> DeleteDepartment(Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest("Geçersiz departman ID'si.");
            }

            try
            {
                var department = await _db.Departments.FindAsync(id);
                if (department == null || department.IsDeleted)
                    return NotFound();

                department.IsDeleted = true;
                department.UpdatedAt = DateTime.Now;
                department.UpdatedBy = GetActiveUserIdString();

                await _db.SaveChangesAsync();
                Logger.Info($"Departman silindi: {department.Name}");
                return Ok(new { Message = "Bölüm başarıyla silindi." });
            }
            catch (Exception ex)
            {
                Logger.Error($"ID'si {id} olan departman silinirken hata oluştu.", ex);
                return InternalServerError(new Exception("Departman silinirken bir hata oluştu."));
            }
        }
    }
}