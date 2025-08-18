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

namespace OgrenciPortalApi.Controllers
{
    [JwtAuth]
    [RoutePrefix("api/semesters")]
    public class SemesterController : BaseApiController
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(SemesterController));

        /// <summary>
        /// Sistemdeki tüm dönemleri listeler.
        /// </summary>
        /// <returns>Dönemlerin listesini içeren bir HTTP yanıtı döner.</returns>
        [HttpGet]
        [Route("")]
        [ResponseType(typeof(List<ListSemestersDTO>))]
        public async Task<IHttpActionResult> GetSemesters()
        {
            try
            {
                var semesters = await _db.Semesters
                    .Where(s => !s.IsDeleted)
                    .Select(s => new ListSemestersDTO
                    {
                        SemesterId = s.SemesterId,
                        SemesterName = s.SemesterName,
                        StartDate = s.StartDate,
                        EndDate = s.EndDate,
                        IsActive = s.IsActive,
                    })
                    .ToListAsync();

                return Ok(semesters);
            }
            catch (Exception ex)
            {
                Logger.Error("Dönemler alınırken hata oluştu.", ex);
                return InternalServerError(new Exception("Dönemler alınırken bir hata oluştu."));
            }
        }

        /// <summary>
        /// Sisteme yeni bir dönem ekler.
        /// </summary>
        /// <param name="dto">Eklenecek dönemin bilgilerini içeren DTO.</param>
        /// <returns>Oluşturma durumunu bildiren bir HTTP yanıtı döner.</returns>
        [HttpPost]
        [Route("add")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> AddSemester(AddSemesterDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                if (await _db.Semesters.AnyAsync(s => s.IsActive) && dto.IsActive)
                    return BadRequest(
                        "Aktif bir dönem bulunmakta lütfen diğer dönemi sonlandırın ya da dönemi pasif olarak ekleyin.");
                if (await _db.Semesters.AnyAsync(s => s.SemesterName.ToLower() == dto.SemesterName.ToLower()))
                    return BadRequest("Bu isme ait bir dönem mevcut. Yeni bir isim giriniz.");
                var semester = new Semesters
                {
                    SemesterId = Guid.NewGuid(),
                    SemesterName = dto.SemesterName,
                    StartDate = dto.StartDate,
                    EndDate = dto.EndDate,
                    IsActive = dto.IsActive,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = GetActiveUserIdString(),
                    UpdatedAt = DateTime.Now,
                    UpdatedBy = GetActiveUserIdString(),
                    IsDeleted = false,
                };
                _db.Semesters.Add(semester);
                await _db.SaveChangesAsync();
                Logger.Info($"Yeni dönem eklendi: {dto.SemesterName}");
                return Ok(new { Message = "Dönem başarıyla eklendi." });
            }
            catch (Exception ex)
            {
                Logger.Error("Yeni dönem eklenirken hata oluştu.", ex);
                return InternalServerError(new Exception("Dönem eklenirken bir hata oluştu."));
            }
        }

        /// <summary>
        /// ID'si belirtilen dönemin düzenleme bilgilerini getirir.
        /// </summary>
        /// <param name="id">Düzenlenecek dönemin ID'si.</param>
        /// <returns>Dönemin düzenleme bilgilerini içeren bir HTTP yanıtı döner.</returns>
        [HttpGet]
        [Route("edit/{id:guid}")]
        [ResponseType(typeof(EditSemesterDTO))]
        public async Task<IHttpActionResult> GetSemester(Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest("Geçersiz dönem ID'si.");
            }

            try
            {
                var semester = await _db.Semesters.FirstOrDefaultAsync(s => s.SemesterId == id && !s.IsDeleted);
                if (semester == null)
                {
                    return NotFound();
                }

                var dto = new EditSemesterDTO
                {
                    SemesterId = semester.SemesterId,
                    SemesterName = semester.SemesterName,
                    StartDate = semester.StartDate,
                    EndDate = semester.EndDate,
                    IsActive = semester.IsActive
                };
                return Ok(dto);
            }
            catch (Exception ex)
            {
                Logger.Error($"ID'si {id} olan dönem bilgileri alınırken hata oluştu.", ex);
                return InternalServerError(new Exception("Dönem bilgileri alınırken bir hata oluştu."));
            }
        }

        /// <summary>
        /// Mevcut bir dönemin bilgilerini günceller.
        /// </summary>
        /// <param name="dto">Güncellenecek dönemin yeni bilgilerini içeren DTO.</param>
        /// <returns>Güncelleme durumunu bildiren bir HTTP yanıtı döner.</returns>
        [HttpPut]
        [Route("edit")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> EditSemester(EditSemesterDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var semester =
                    await _db.Semesters.FirstOrDefaultAsync(s => s.SemesterId == dto.SemesterId && !s.IsDeleted);
                if (semester == null)
                {
                    return NotFound();
                }

                if (await _db.Semesters.AnyAsync(s => s.SemesterName.ToLower() == dto.SemesterName.ToLower()))
                    return BadRequest("Bu isme sahip bir dönem bulunmakta. Lütfen başka bir isim giriniz.");
                if (dto.IsActive && await _db.Semesters.AnyAsync(s => s.IsActive && !s.IsDeleted))
                    return BadRequest(
                        "Aktif bir dönem zaten mevcut. Lütfen diğer dönemi pasif yapın ve tekrar deneyin.");

                semester.SemesterName = dto.SemesterName;
                semester.StartDate = dto.StartDate;
                semester.EndDate = dto.EndDate;
                semester.IsActive = dto.IsActive;
                semester.UpdatedAt = DateTime.UtcNow;
                semester.UpdatedBy = GetActiveUserIdString();
                await _db.SaveChangesAsync();
                Logger.Info($"Dönem güncellendi: {dto.SemesterName}");
                return Ok(new { Message = "Dönem başarıyla güncellendi." });
            }
            catch (Exception ex)
            {
                Logger.Error($"ID'si {dto.SemesterId} olan dönem güncellenirken hata oluştu.", ex);
                return InternalServerError(new Exception("Dönem güncellenirken bir hata oluştu."));
            }
        }

        /// <summary>
        /// Belirtilen ID'ye sahip dönemi siler (soft delete).
        /// </summary>
        /// <param name="id">Silinecek dönemin ID'si.</param>
        /// <returns>Silme işleminin durumunu bildiren bir HTTP yanıtı döner.</returns>
        [HttpDelete]
        [Route("delete/{id:guid}")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> DeleteSemester(Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest("Geçersiz dönem ID'si.");
            }

            try
            {
                var semester = await _db.Semesters.FirstOrDefaultAsync(s => s.SemesterId == id && !s.IsDeleted);
                if (semester == null)
                {
                    return NotFound();
                }

                semester.IsDeleted = true;
                semester.UpdatedAt = DateTime.UtcNow;
                semester.UpdatedBy = GetActiveUserIdString();
                semester.DeletedAt = DateTime.Now;
                semester.DeletedBy = GetActiveUserIdString();
                await _db.SaveChangesAsync();
                Logger.Info($"Dönem silindi: {semester.SemesterName}");
                return Ok("Dönem Başarıyla Silindi");
            }
            catch (Exception ex)
            {
                Logger.Error($"ID'si {id} olan dönem silinirken hata oluştu.", ex);
                return InternalServerError(new Exception("Dönem silinirken bir hata oluştu."));
            }
        }
    }
}