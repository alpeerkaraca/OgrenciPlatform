using log4net;
using OgrenciPortalApi.Attributes;
using OgrenciPortalApi.Models;
using OgrenciPortalApi.Utils;
using OgrenciPortali.DTOs;
using System;
using System.Data.Entity;
using System.IdentityModel.Claims;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace OgrenciPortalApi.Controllers
{
    [JwtAuth]
    [RoutePrefix("api/semesters")]
    public class SemesterController : ApiController
    {
        private readonly ogrenci_portalEntities db = new ogrenci_portalEntities();
        private static readonly ILog Logger = LogManager.GetLogger(typeof(SemesterController));

        [HttpGet]
        [Route("")]
        public async Task<IHttpActionResult> GetSemesters()
        {
            try
            {
                var semesters = await db.Semesters
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
                if (semesters.Count == 0)
                {
                    return NotFound();
                }

                Logger.Info("Dönemler çekildi.");
                return Ok(semesters);
            }
            catch (Exception ex)
            {
                Logger.Error("Dönemler çekilirken hata oluştu.", ex);
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("add")]
        public async Task<IHttpActionResult> PostAddSemester(AddSemesterDTO dto)
        {
            try
            {
                if (dto == null)
                {
                    return BadRequest("Dönem bilgileri eksik.");
                }

                var semester = new Semesters
                {
                    SemesterId = Guid.NewGuid(),
                    SemesterName = dto.SemesterName,
                    StartDate = dto.StartDate,
                    EndDate = dto.EndDate,
                    IsActive = dto.IsActive,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = GetActiveUserId(),
                    UpdatedAt = DateTime.Now,
                    UpdatedBy = GetActiveUserId(),
                    IsDeleted = false,
                };
                db.Semesters.Add(semester);
                await db.SaveChangesAsync();
                Logger.Info($"Yeni dönem eklendi: {dto.SemesterName}");
                return Ok("Dönem Başarıyla Eklendi !");
            }
            catch (Exception ex)
            {
                Logger.Error("Dönem ekleme sayfası yüklenirken hata oluştu.", ex);
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("edit/{id:guid}")]
        public async Task<IHttpActionResult> GetEditSemester(Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest("Geçersiz dönem ID'si.");
            }

            var semester = await db.Semesters.FirstOrDefaultAsync(s => s.SemesterId == id && !s.IsDeleted);
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

        [HttpPost]
        [Route("edit")]
        public async Task<IHttpActionResult> PostEditSemester(EditSemesterDTO dto)
        {
            try
            {
                if (dto == null)
                {
                    return BadRequest("Dönem bilgileri eksik.");
                }

                var semester =
                    await db.Semesters.FirstOrDefaultAsync(s => s.SemesterId == dto.SemesterId && !s.IsDeleted);
                if (semester == null)
                {
                    return NotFound();
                }

                semester.SemesterName = dto.SemesterName;
                semester.StartDate = dto.StartDate;
                semester.EndDate = dto.EndDate;
                semester.IsActive = dto.IsActive;
                semester.UpdatedAt = DateTime.UtcNow;
                semester.UpdatedBy = GetActiveUserId();
                await db.SaveChangesAsync();
                Logger.Info($"Dönem güncellendi: {dto.SemesterName}");
                return Ok("Dönem Başarıyla Güncellendi !");
            }
            catch (Exception ex)
            {
                Logger.Error("Dönem güncelleme sayfası yüklenirken hata oluştu.", ex);
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("delete/{id:guid}")]
        public async Task<IHttpActionResult> DeleteSemester(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    return BadRequest("Geçersiz dönem ID'si.");
                }

                var semester = await db.Semesters.FirstOrDefaultAsync(s => s.SemesterId == id && !s.IsDeleted);
                if (semester == null)
                {
                    return NotFound();
                }

                semester.IsDeleted = true;
                semester.UpdatedAt = DateTime.UtcNow;
                semester.UpdatedBy = GetActiveUserId();
                semester.DeletedAt = DateTime.Now;
                semester.DeletedBy = GetActiveUserId();
                await db.SaveChangesAsync();
                Logger.Info($"Dönem silindi: {semester.SemesterName}");
                return Ok("Dönem Başarıyla Silindi !");
            }
            catch (Exception ex)
            {
                Logger.Error("Dönem silinirken hata oluştu.", ex);
                return InternalServerError(ex);
            }
        }

        public string GetActiveUserId()
        {
            return TokenManager.GetPrincipal(Request.Headers.Authorization.Parameter)
                .FindFirst(ClaimTypes.NameIdentifier).Value;
        }
    }
}