using log4net;
using OgrenciPortalApi.Attributes;
using OgrenciPortalApi.Models;
using OgrenciPortalApi.Utils;
using Shared.DTO;
using Shared.Enums;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Mvc;

namespace OgrenciPortalApi.Controllers
{
    [System.Web.Http.RoutePrefix("api/user")]
    public class UserController : BaseApiController
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(UserController));

        /// <summary>
        /// Kullanıcı girişi yapar ve JWT token döner.
        /// </summary>
        /// <param name="model">Kullanıcının e-posta ve şifre bilgilerini içeren model.</param>
        /// <returns>Giriş başarılıysa JWT token, değilse hata mesajı döner.</returns>
        [System.Web.Http.HttpPost]
        [System.Web.Http.AllowAnonymous]
        [System.Web.Http.Route("login")]
        [ResponseType(typeof(object))]
        public async Task<IHttpActionResult> Login([FromBody] LoginRequestDTO model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var user =
                    await _db.Users.FirstOrDefaultAsync(u => u.Email == model.Email && !u.IsDeleted && u.IsActive);
                if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.Password))
                {
                    return Unauthorized();
                }

                var token = TokenManager.GenerateJwtToken(user);
                return Ok(new
                {
                    Token = token,
                    Message = "Giriş başarılı.",
                });
            }
            catch (Exception ex)
            {
                Logger.Error("Giriş yapılırken hata oluştu.", ex);
                return InternalServerError(new Exception("Giriş yapılırken bir sunucu hatası oluştu."));
            }
        }

        /// <summary>
        /// Yeni kullanıcı kayıt sayfası için gerekli verileri (rol, departman, danışman listeleri vb.) getirir.
        /// </summary>
        /// <returns>Gerekli listeleri içeren bir HTTP yanıtı döner.</returns>
        [System.Web.Http.HttpGet]
        [JwtAuth]
        [System.Web.Http.Route("register")]
        [ResponseType(typeof(RegisterDataDto))]
        public IHttpActionResult GetRegisterData()
        {
            try
            {
                var registerDataDto = new RegisterDataDto();
                FillRegisterModel(registerDataDto);
                return Ok(registerDataDto);
            }
            catch (Exception ex)
            {
                Logger.Error("Kayıt sayfası için veriler alınırken hata oluştu.", ex);
                return InternalServerError(new Exception("Kayıt sayfası için veriler alınırken bir hata oluştu."));
            }
        }

        /// <summary>
        /// Sisteme yeni bir kullanıcı kaydeder.
        /// </summary>
        /// <param name="model">Kaydedilecek kullanıcının bilgilerini içeren model.</param>
        /// <returns>Kayıt durumunu bildiren bir HTTP yanıtı döner.</returns>
        [System.Web.Http.HttpPost]
        [JwtAuth]
        [System.Web.Http.Route("register")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Register([FromBody] RegisterDataDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                if (await _db.Users.AnyAsync(u => u.Email == model.Email && !u.IsDeleted))
                    return BadRequest("Bu e-posta adresi zaten kayıtlı.");

                if (model.Role == (int)Roles.Öğrenci && !string.IsNullOrEmpty(model.StudentNo) &&
                    await _db.Users.AnyAsync(u => u.StudentNo == model.StudentNo && !u.IsDeleted))
                    return BadRequest("Bu öğrenci numarası zaten kayıtlı.");

                var newUser = new Users
                {
                    UserId = Guid.NewGuid(),
                    Name = model.Name,
                    Surname = model.Surname,
                    Email = model.Email,
                    Password = BCrypt.Net.BCrypt.HashPassword(model.Password),
                    Role = (int)model.Role,
                    IsActive = true,
                    IsFirstLogin = true,
                    StudentNo = model.StudentNo,
                    DepartmentId = model.DepartmentId,
                    AdvisorId = model.AdvisorId,
                    CreatedAt = DateTime.Now,
                    CreatedBy = GetActiveUserIdString(),
                    UpdatedAt = DateTime.Now,
                    UpdatedBy = GetActiveUserIdString(),
                    IsDeleted = false
                };

                _db.Users.Add(newUser);
                await _db.SaveChangesAsync();
                return Ok(new { Message = "Kullanıcı başarıyla kaydedildi." });
            }
            catch (Exception ex)
            {
                Logger.Error("Kullanıcı kaydı sırasında hata oluştu.", ex);
                return InternalServerError(new Exception("Kullanıcı kaydedilirken bir hata oluştu."));
            }
        }

        /// <summary>
        /// Sistemdeki tüm kullanıcıları listeler.
        /// </summary>
        /// <returns>Kullanıcıların listesini içeren bir HTTP yanıtı döner.</returns>
        [System.Web.Http.HttpGet]
        [JwtAuth]
        [System.Web.Http.Route("list")]
        [ResponseType(typeof(object))]
        public async Task<IHttpActionResult> GetUsers()
        {
            try
            {
                var users = await _db.Users
                    .Where(u => !u.IsDeleted)
                    .Include(u => u.Departments)
                    .Select(u => new UserDTO
                    {
                        UserId = u.UserId,
                        CreatedAt = u.CreatedAt,
                        DepartmentName = u.Departments.Name,
                        Email = u.Email,
                        FullName = u.Name + " " + u.Surname,
                        IsFirstLogin = u.IsFirstLogin,
                        Role = (Roles)u.Role,
                        StudentNo = u.StudentNo
                    })
                    .ToListAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                Logger.Error("Kullanıcı listesi alınırken hata oluştu.", ex);
                return InternalServerError(new Exception("Kullanıcı listesi alınırken bir hata oluştu."));
            }
        }

        /// <summary>
        /// Mevcut kullanıcının şifresini değiştirir.
        /// </summary>
        /// <param name="model">Mevcut ve yeni şifre bilgilerini içeren model.</param>
        /// <returns>İşlem sonucunu bildiren bir HTTP yanıtı döner.</returns>
        [System.Web.Http.HttpPost]
        [JwtAuth]
        [System.Web.Http.Route("ChangePassword")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> ChangePassword([FromBody] ChangePasswordDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var userId = GetActiveUserId();
                var user = await _db.Users.FindAsync(userId);

                if (user == null || user.IsDeleted)
                {
                    return NotFound();
                }

                if (!BCrypt.Net.BCrypt.Verify(model.CurrentPassword, user.Password))
                {
                    return BadRequest("Mevcut şifreniz yanlış.");
                }

                user.Password = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
                user.UpdatedAt = DateTime.Now;
                user.UpdatedBy = userId.ToString();
                if (user.IsFirstLogin) user.IsFirstLogin = false;

                await _db.SaveChangesAsync();

                return Ok(new { Message = "Şifre başarıyla değiştirildi." });
            }
            catch (Exception ex)
            {
                Logger.Error("Şifre değiştirilirken hata oluştu.", ex);
                return InternalServerError(new Exception("Şifre değiştirilirken bir hata oluştu."));
            }
        }

        /// <summary>
        /// ID'si belirtilen kullanıcının düzenleme bilgilerini getirir.
        /// </summary>
        /// <param name="id">Düzenlenecek kullanıcının ID'si.</param>
        /// <returns>Kullanıcının düzenleme bilgilerini içeren bir HTTP yanıtı döner.</returns>
        [System.Web.Http.HttpGet]
        [JwtAuth]
        [System.Web.Http.Route("edit/{id:guid}")]
        [ResponseType(typeof(EditUserDTO))]
        public async Task<IHttpActionResult> GetUserForEdit(Guid id)
        {
            try
            {
                var userInDb = await _db.Users.FindAsync(id);
                if (userInDb == null || userInDb.IsDeleted)
                {
                    return NotFound();
                }

                var dto = new EditUserDTO()
                {
                    UserId = userInDb.UserId,
                    Name = userInDb.Name,
                    Surname = userInDb.Surname,
                    Email = userInDb.Email,
                    Role = userInDb.Role,
                    DepartmentId = userInDb.DepartmentId,
                    AdvisorId = userInDb.AdvisorId,
                    StudentNo = userInDb.StudentNo,
                    
                };
                FillEditModel(dto);

                return Ok(dto);
            }
            catch (Exception ex)
            {
                Logger.Error($"ID'si {id} olan kullanıcı bilgileri alınırken hata oluştu.", ex);
                return InternalServerError(new Exception("Kullanıcı bilgileri alınırken bir hata oluştu."));
            }
        }

        /// <summary>
        /// Mevcut bir kullanıcının bilgilerini günceller.
        /// </summary>
        /// <param name="model">Güncellenecek kullanıcının yeni bilgilerini içeren model.</param>
        /// <returns>Güncelleme durumunu bildiren bir HTTP yanıtı döner.</returns>
        [System.Web.Http.HttpPut]
        [JwtAuth]
        [System.Web.Http.Route("edit")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> EditUser(EditUserDTO model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                if (await _db.Users.AnyAsync(u => u.Email == model.Email && u.UserId != model.UserId && !u.IsDeleted))
                    return Conflict();

                var userInDb = await _db.Users.FindAsync(model.UserId);
                if (userInDb == null || userInDb.IsDeleted)
                    return NotFound();

                userInDb.Name = model.Name;
                userInDb.Surname = model.Surname;
                userInDb.Email = model.Email;
                userInDb.AdvisorId = model.AdvisorId;
                userInDb.DepartmentId = model.DepartmentId;
                userInDb.Role = (int)model.Role;
                userInDb.StudentNo = model.StudentNo;
                userInDb.UpdatedAt = DateTime.Now;
                userInDb.UpdatedBy = GetActiveUserIdString();

                await _db.SaveChangesAsync();
                return Ok(new { Message = "Kullanıcı başarıyla güncellendi." });
            }
            catch (Exception ex)
            {
                Logger.Error($"ID'si {model.UserId} olan kullanıcı güncellenirken hata oluştu.", ex);
                return InternalServerError(new Exception("Kullanıcı güncellenirken bir hata oluştu."));
            }
        }

        /// <summary>
        /// Belirtilen ID'ye sahip kullanıcıyı siler (soft delete).
        /// </summary>
        /// <param name="id">Silinecek kullanıcının ID'si.</param>
        /// <returns>Silme işleminin durumunu bildiren bir HTTP yanıtı döner.</returns>
        [System.Web.Http.HttpDelete]
        [JwtAuth]
        [System.Web.Http.Route("delete/{id:guid}")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> DeleteUser(Guid id)
        {
            try
            {
                var userInDb = await _db.Users.FindAsync(id);
                if (userInDb == null)
                {
                    return NotFound();
                }

                userInDb.IsDeleted = true;
                userInDb.DeletedAt = DateTime.Now;
                userInDb.DeletedBy = GetActiveUserIdString();
                userInDb.UpdatedAt = DateTime.Now;
                userInDb.UpdatedBy = GetActiveUserIdString();
                await _db.SaveChangesAsync();
                return Ok(new { Message = $"Kullanıcı ({userInDb.Name} {userInDb.Surname}) başarıyla silindi." });
            }
            catch (Exception ex)
            {
                Logger.Error($"ID'si {id} olan kullanıcı silinirken hata oluştu.", ex);
                return InternalServerError(new Exception("Kullanıcı silinirken bir hata oluştu."));
            }
        }

        private void FillEditModel(EditUserDTO model)
        {
            model.RolesList = Enum.GetValues(typeof(Roles))
                .Cast<Roles>()
                .Select(r => new SelectListItem { Text = r.ToString(), Value = ((int)r).ToString() });
            model.DepartmentsList = _db.Departments
                .Where(d => !d.IsDeleted && d.IsActive)
                .Select(d => new SelectListItem { Value = d.DepartmentId.ToString(), Text = d.Name });
            model.AdvisorsList = _db.Users
                .Where(u => u.Role == (int)Roles.Danışman && !u.IsDeleted && u.IsActive)
                .Select(u => new SelectListItem { Value = u.UserId.ToString(), Text = u.Name + " " + u.Surname });
        }

        private void FillRegisterModel(RegisterDataDto model)
        {
            model.RolesList = Enum.GetValues(typeof(Roles))
                .Cast<Roles>()
                .Select(r => new SelectListItem { Text = r.ToString(), Value = ((int)r).ToString() });
            model.DepartmentsList = _db.Departments
                .Where(d => !d.IsDeleted && d.IsActive)
                .Select(d => new SelectListItem { Value = d.DepartmentId.ToString(), Text = d.Name });
            model.AdvisorsList = _db.Users
                .Where(u => u.Role == (int)Roles.Danışman && !u.IsDeleted && u.IsActive)
                .Select(u => new SelectListItem { Value = u.UserId.ToString(), Text = $@"{u.Name} {u.Surname}" });
        }
    }
}