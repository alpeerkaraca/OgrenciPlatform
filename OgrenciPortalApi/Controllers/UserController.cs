using log4net;
using OgrenciPortalApi.Attributes;
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

namespace OgrenciPortalApi.Controllers
{
    [System.Web.Http.RoutePrefix("api/user")]
    public class UserController : ApiController
    {
        private readonly ogrenci_portalEntities _db = new ogrenci_portalEntities();
        private static readonly ILog Logger = LogManager.GetLogger(typeof(UserController));

        [System.Web.Http.HttpPost]
        [System.Web.Http.AllowAnonymous]
        [System.Web.Http.Route("login")]
        public async Task<IHttpActionResult> Login([FromBody] LoginRequestDTO model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
                if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.Password) || user.IsDeleted)
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
                Logger.Error("Giriş Yapılırken Hata Oluştu.", ex);
                return InternalServerError(ex);
            }
        }


        [System.Web.Http.HttpGet]
        [JwtAuth]
        [System.Web.Http.Route("register")]
        public async Task<IHttpActionResult> GetRegister()
        {
            try
            {
                var registerDataDto = new RegisterDataDto();
                FillModel(registerDataDto);
                return Ok(registerDataDto);
            }
            catch (Exception ex)
            {
                Logger.Error("Kullanıcı Bilgileri Alınırken Hata Oluştu.", ex);
                return InternalServerError(ex);
            }
        }

        [System.Web.Http.HttpPost]
        [JwtAuth]
        [System.Web.Http.Route("register")]
        public async Task<IHttpActionResult> Register([FromBody] RegisterDataDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            try
            {
                if (await _db.Users.AnyAsync(u => u.Email == model.Email))
                    return BadRequest("Bu e-posta adresi zaten kayıtlı.");
                if (await _db.Users.AnyAsync(u => u.StudentNo == model.StudentNo) &&
                    model.Role == (int)Roles.Öğrenci)
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
                    CreatedBy = GetActiveUserId(),
                    UpdatedAt = DateTime.Now,
                    UpdatedBy = GetActiveUserId(),
                    IsDeleted = false
                };

                _db.Users.Add(newUser);
                await _db.SaveChangesAsync();
                return Ok(new
                {
                    Message = "Kullanıcı başarıyla kaydedildi.",
                });
            }
            catch (Exception ex)
            {
                Logger.Error("Kullanıcı Kaydedilirken Hata Oluştu.", ex);
                return InternalServerError(ex);
            }
        }

        [System.Web.Http.HttpGet]
        [JwtAuth]
        [System.Web.Http.Route("list")]
        public async Task<IHttpActionResult> GetUsers()
        {
            try
            {
                var users = await _db.Users
                    .Where(u => !u.IsDeleted)
                    .Include(u => u.Departments)
                    .ToListAsync();
                if (!users.Any())
                    return NotFound();
                return Ok(users);
            }
            catch (Exception ex)
            {
                Logger.Error("Kullanıcı Listesi Alınırken Hata Oluştu.", ex);
                return InternalServerError(ex);
            }
        }


        [System.Web.Http.HttpPost]
        [JwtAuth]
        [System.Web.Http.Route("ChangePassword")]
        public async Task<IHttpActionResult> ChangePassword([FromBody] ChangePasswordDTO model)
        {
            if (!ModelState.IsValid)
            {
                Logger.Error("Modelstate sorunu:");
                return BadRequest();
            }

            try
            {
                var user = await _db.Users.FindAsync(model.UserId);
                if (user == null)
                {
                    Logger.Error("Kullanıcı bulunamadı.");
                    return NotFound();
                }

                if (!BCrypt.Net.BCrypt.Verify(model.CurrentPassword, user.Password))
                {
                    return BadRequest("Eski şifre yanlış.");
                }

                if (user.IsDeleted)
                    return Unauthorized();
                user.Password = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
                user.UpdatedAt = DateTime.Now;
                user.UpdatedBy = GetActiveUserId();
                await _db.SaveChangesAsync();

                return Ok(new
                {
                    Message = "Şifre başarıyla değiştirildi.",
                });
            }
            catch (Exception ex)
            {
                Logger.Error("Şifre Değiştirilirken Hata Oluştu.", ex);
                return InternalServerError(ex);
            }
        }

        [System.Web.Http.HttpGet]
        [JwtAuth]
        [System.Web.Http.Route("edit/{id:guid}")]
        public async Task<IHttpActionResult> Edit(Guid id)
        {
            var userInDb = await _db.Users.FindAsync(id);
            if (userInDb == null)
            {
                return NotFound();
            }

            var roleList = Enum.GetValues(typeof(Roles))
                .Cast<Roles>()
                .Select(r => new SelectListItem { Text = r.ToString(), Value = ((int)r).ToString() });
            var departmentsList = (await _db.Departments.ToListAsync())
                .Select(d => new SelectListItem { Value = d.DepartmentId.ToString(), Text = d.Name });
            var advisorsList = (await _db.Users.Where(u => u.Role == (int)Roles.Danışman).ToListAsync())
                .Select(u => new SelectListItem { Value = u.UserId.ToString(), Text = $@"{u.Name} {u.Surname}" });

            var dto = new EditUserDTO()
            {
                UserId = userInDb.UserId,
                Name = userInDb.Name,
                Surname = userInDb.Surname,
                Email = userInDb.Email,
                Role = (Roles)userInDb.Role,
                DepartmentId = userInDb.DepartmentId,
                AdvisorId = userInDb.AdvisorId,
                StudentNo = userInDb.StudentNo,
                AdvisorsList = advisorsList,
                DepartmentsList = departmentsList,
                RolesList = roleList
            };

            return Ok(dto);
        }

        [System.Web.Http.HttpPost]
        [JwtAuth]
        [System.Web.Http.Route("edit")]
        public async Task<IHttpActionResult> Edit(EditUserDTO model)
        {
            if (string.IsNullOrEmpty(model.UserId.ToString()))
                return BadRequest();
            if (await IsUserNotExist(model.Email))
                return Conflict();
            var userInDb = await _db.Users.FindAsync(model.UserId);
            if (userInDb == null)
                return NotFound();
            userInDb.Name = model.Name;
            userInDb.Surname = model.Surname;
            userInDb.AdvisorId = model.AdvisorId;
            userInDb.DepartmentId = model.DepartmentId;
            userInDb.Role = (int)model.Role;
            userInDb.StudentNo = model.StudentNo;
            userInDb.Email = model.Email;
            userInDb.UpdatedAt = DateTime.Now;
            userInDb.UpdatedBy = GetActiveUserId();
            FillModel(model);

            await _db.SaveChangesAsync();
            return Ok(new { Message = "Kullanıcı Başarıyla Eklendi.", content = model });
        }


        [System.Web.Http.HttpDelete]
        [JwtAuth]
        [System.Web.Http.Route("delete/{id:guid}")]
        public async Task<IHttpActionResult> Delete(Guid id)
        {
            var userInDb = await _db.Users.FindAsync(id);
            if (userInDb == null)
            {
                return NotFound();
            }

            userInDb.IsDeleted = true;
            userInDb.DeletedAt = DateTime.Now;
            userInDb.DeletedBy = GetActiveUserId();
            userInDb.UpdatedAt = DateTime.Now;
            userInDb.UpdatedBy = GetActiveUserId();
            await _db.SaveChangesAsync();
            return Ok($"Kullanıcı ({userInDb.Name} {userInDb.Surname}) silindi.");
        }


        public async Task<bool> IsUserNotExist(string email)
        {
            return !await _db.Users.AnyAsync(users => users.Email == email);
        }

        public string GetActiveUserId()
        {
            return TokenManager.GetPrincipal(Request.Headers.Authorization.Parameter)
                .FindFirst(ClaimTypes.NameIdentifier).Value;
        }

        private void FillModel(EditUserDTO model)
        {
            model.RolesList = Enum.GetValues(typeof(Roles))
                .Cast<Roles>()
                .Select(r => new SelectListItem { Text = r.ToString(), Value = ((int)r).ToString() });
            model.DepartmentsList = _db.Departments
                .Select(d => new SelectListItem { Value = d.DepartmentId.ToString(), Text = d.Name });
            model.AdvisorsList = _db.Users
                .Where(u => u.Role == (int)Roles.Danışman)
                .Select(u => new SelectListItem { Value = u.UserId.ToString(), Text = $@"{u.Name} {u.Surname}" });
        }

        private void FillModel(RegisterDataDto model)
        {
            model.RolesList = Enum.GetValues(typeof(Roles))
                .Cast<Roles>()
                .Select(r => new SelectListItem { Text = r.ToString(), Value = ((int)r).ToString() });
            model.DepartmentsList = _db.Departments
                .Select(d => new SelectListItem { Value = d.DepartmentId.ToString(), Text = d.Name });
            model.AdvisorsList = _db.Users
                .Where(u => u.Role == (int)Roles.Danışman)
                .Select(u => new SelectListItem { Value = u.UserId.ToString(), Text = $@"{u.Name} {u.Surname}" });
        }
    }
}