using log4net;
using OgrenciPortalApi.Models;
using OgrenciPortalApi.Utils;
using Shared.DTO;
using Shared.Enums;
using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Hosting;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.Results;
using System.Web.Mvc;
using MailKit.Net.Smtp;
using MimeKit;
using OgrenciPortalApi.Services;
using Org.BouncyCastle.Ocsp;
using ModelStateDictionary = System.Web.Http.ModelBinding.ModelStateDictionary;

namespace OgrenciPortalApi.Controllers
{
    [System.Web.Http.RoutePrefix("api/user")]
    [System.Web.Http.Authorize]
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
        public async Task<HttpResponseMessage> Login([FromBody] LoginUserDTO model)
        {
            try
            {
                if (model == null || !ModelState.IsValid)
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { Message = "Form verisi hatalı." });

                var user =
                    await _db.Users.Include(users => users.Departments)
                        .FirstOrDefaultAsync(u => u.Email == model.Email);
                if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.Password))
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized,
                        new { Message = "E-Posta adresiniz ya da parolanız hatalı." });
                }

                if (user.IsDeleted || !user.IsActive)
                    return Request.CreateResponse(HttpStatusCode.Unauthorized,
                        new { Message = "Hesabınız pasif hale getirilmiş ya da silinmiş." });

                var claims = TokenManager.GetClaimsFromUser(user);

                var accessToken = TokenManager.GenerateAccessToken(claims);
                var refreshToken = TokenManager.GenerateRefreshToken();
                user.RefreshTokenExpTime = DateTime.UtcNow.AddDays(Convert.ToInt32(AppSettings.RefreshTokenExpDays));
                user.RefreshToken = refreshToken;
                await _db.SaveChangesAsync();


                var cookie = new CookieHeaderValue("RefreshToken", refreshToken)
                {
                    Expires = DateTimeOffset.UtcNow.AddDays(Convert.ToInt32(AppSettings.RefreshTokenExpDays)),
                    Domain = Request.RequestUri.Host,
                    Path = "/",
                    HttpOnly = true,
                    Secure = true,
                };
                var res = Request.CreateResponse(HttpStatusCode.OK,
                    new LoginSuccessResponse()
                        { RefreshToken = refreshToken, AccessToken = accessToken, Message = "Giriş Başarılı" });
                res.Headers.AddCookies(new[] { cookie });

                return res;
            }
            catch (Exception ex)
            {
                Logger.Error("Giriş yapılırken hata oluştu.", ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError,
                    new Exception("Giriş yapılırken bir sunucu hatası oluştu."));
            }
        }

        /// <summary>
        /// Yeni kullanıcı kayıt sayfası için gerekli verileri (rol, departman, danışman listeleri vb.) getirir.
        /// </summary>
        /// <returns>Gerekli listeleri içeren bir HTTP yanıtı döner.</returns>
        [System.Web.Http.HttpGet]
        [System.Web.Http.Authorize(Roles = nameof(Roles.Admin))]
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
        [System.Web.Http.Authorize(Roles = nameof(Roles.Admin))]
        [System.Web.Http.Route("register")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Register([FromBody] RegisterDataDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            using (var transaction = _db.Database.BeginTransaction())
            {
                try
                {
                    var userClaim = User.Identity as ClaimsIdentity;
                    var userId = userClaim.FindFirst(ClaimTypes.NameIdentifier).Value;
                    string generatedStudentNo = await CreateStudentNo(model);
                    if (generatedStudentNo == null && model.Role == (int)Roles.Öğrenci)
                        return BadRequest("Öğrenci Numarası Hatalı");


                    if (model.Role == (int)Roles.Öğrenci && await CheckUserData.CheckStudentNoAsync(generatedStudentNo))
                        return BadRequest("Bu öğrenci numarası zaten kayıtlı.");

                    if (await _db.Users.AnyAsync(u => u.Email == model.Email && !u.IsDeleted))
                        return BadRequest("Bu e-posta adresi kullanılıyor.");
                    var newUser = new Users
                    {
                        UserId = Guid.NewGuid(),
                        Name = model.Name,
                        Surname = model.Surname,
                        Email = model.Email,
                        Password = BCrypt.Net.BCrypt.HashPassword(model.Password),
                        Role = model.Role,
                        StudentYear = model.StudentYear,
                        IsActive = true,
                        IsFirstLogin = true,
                        StudentNo = generatedStudentNo,
                        DepartmentId = model.DepartmentId,
                        AdvisorId = model.AdvisorId,
                        CreatedAt = DateTime.Now,
                        CreatedBy = userId,
                        UpdatedAt = DateTime.Now,
                        UpdatedBy = userId,
                        IsDeleted = false
                    };


                    _db.Users.Add(newUser);
                    await _db.SaveChangesAsync();
                    transaction.Commit();
                    var htmlPath = HostingEnvironment.MapPath("~/Views/account-created-template.html");
                    var htmlTemplate = File.ReadAllText(htmlPath);
                    htmlTemplate = htmlTemplate.Replace("[Öğrenci Adı Soyadı]", newUser.Name + " " + newUser.Surname);
                    htmlTemplate = htmlTemplate.Replace("[E-Posta Adresi]", newUser.Email);
                    htmlTemplate = htmlTemplate.Replace("[Geçici Şifre]", model.Password);
                    htmlTemplate = htmlTemplate.Replace("[Giris_Linki]", AppSettings.JwtAudience + "/User/Login");
                    await MailService.Instance.SendEmailAsync(newUser.Email, "Öğrenci Portala Hoş Geldiniz", htmlTemplate);
                    return Ok(new { Message = "Kullanıcı başarıyla kaydedildi." });
                }
                catch (Exception ex)
                {
                    Logger.Error("Kullanıcı kaydı sırasında hata oluştu.", ex);
                    return InternalServerError(new Exception("Kullanıcı kaydedilirken bir hata oluştu."));
                }
            }
        }

        /// <summary>
        /// Sistemdeki tüm kullanıcıları listeler.
        /// </summary>
        /// <returns>Kullanıcıların listesini içeren bir HTTP yanıtı döner.</returns>
        [System.Web.Http.HttpGet]
        [System.Web.Http.Authorize(Roles = nameof(Roles.Admin))]
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
        [System.Web.Http.Authorize]
        [System.Web.Http.Route("change-password")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> ChangePassword([FromBody] ChangePasswordDTO model)
        {
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
        [System.Web.Http.Authorize(Roles = nameof(Roles.Admin))]
        [System.Web.Http.Route("edit/{id:guid}")]
        [ResponseType(typeof(EditUserDTO))]
        public async Task<IHttpActionResult> GetUserForEdit([FromUri] Guid id)
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
                    StudentYear = userInDb.StudentYear,
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
        [System.Web.Http.HttpPost]
        [System.Web.Http.Authorize(Roles = nameof(Roles.Admin))]
        [System.Web.Http.Route("edit")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> EditUser([FromBody] EditUserDTO model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                if (await _db.Users.AnyAsync(u =>
                        u.Email.ToLower() == model.Email.ToLower() && u.UserId != model.UserId))
                    return BadRequest("Bu e-posta adresine sahip bir kullanıcı bulunmakta.");
                if (!string.IsNullOrEmpty(model.StudentNo) && await _db.Users.AnyAsync(u =>
                        u.StudentNo.ToLower() == model.StudentNo.ToLower() && u.UserId != model.UserId &&
                        u.Role == model.Role))
                    return BadRequest("Bu öğrenci numarasına sahip bir öğrenci bulunmakta.");

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
        [System.Web.Http.HttpPost]
        [System.Web.Http.Authorize(Roles = nameof(Roles.Admin))]
        [System.Web.Http.Route("delete/{id:guid}")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> DeleteUser([FromUri] Guid id)
        {
            try
            {
                var userInDb = await _db.Users.FindAsync(id);
                if (userInDb == null || userInDb.IsDeleted)
                {
                    return NotFound();
                }

                var advisorsCourses = await _db.OfferedCourses.Where(oc => oc.TeacherId == id).ToListAsync();

                if (userInDb.Role == (int)Roles.Danışman &&
                    await _db.OfferedCourses.AnyAsync(oc => oc.TeacherId == id && oc.Semesters.IsActive))
                    return BadRequest("Danışmanın ders verdiği dönem aktif olduğu için danışman silinemez.");

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

        [System.Web.Http.HttpPost]
        [System.Web.Http.AllowAnonymous]
        [System.Web.Http.Route("forgot-password")]
        public async Task<IHttpActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDto model)
        {
            try
            {
                var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == model.Email && !u.IsDeleted);
                if (user != null)
                {
                    var token = Guid.NewGuid().ToString("N");
                    user.PasswordResetToken = token;
                    user.ResetTokenExpiry = DateTime.UtcNow.AddMinutes(15);
                    await _db.SaveChangesAsync();
                    var resetLink = AppSettings.JwtAudience + "/User/ResetPassword?token=" + token;
                    await SendPasswordResetEmail(user.Email, resetLink);
                }

                return Ok(new
                {
                    Message =
                        "Eğer E-Postanız sistemimizde kayıtlıysa 15 dakika geçerli bir sıfırlama bağlantısı gönderilecektir. Spam kutunuzu kontrol etmeyi unutmayın."
                });
            }
            catch (Exception ex)
            {
                Logger.Error("Parola sıfırlama isteği sırasında hata oluştu.", ex);
                return Ok(new
                {
                    Message =
                        "Eğer E-Postanız sistemimizde kayıtlıysa 15 dakika geçerli bir sıfırlama bağlantısı gönderilecektir. Spam kutunuzu kontrol etmeyi unutmayın."
                });
            }
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("reset-password")]
        [System.Web.Http.AllowAnonymous]
        public async Task<IHttpActionResult> ResetPassword([FromBody] ResetPasswordRequestDto model)
        {
            if (!string.Equals(model.NewPassword, model.ConfirmPassword))
                return BadRequest("Şifreler uyuşmuyor.");
            var user = await _db.Users.FirstOrDefaultAsync(u => u.PasswordResetToken == model.Token && !u.IsDeleted);

            if (user == null || user.ResetTokenExpiry <= DateTime.UtcNow)
                return BadRequest("Geçersiz veya süresi dolmuş bir token kullandınız.");

            user.Password = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);

            user.PasswordResetToken = null;
            user.ResetTokenExpiry = null;
            await _db.SaveChangesAsync();

            return Ok(new { Message = "Şifreniz başarıyla güncellendi." });
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("increase-student-year/{id:guid}")]
        [System.Web.Http.Authorize(Roles = nameof(Roles.Admin))]
        public async Task<IHttpActionResult> IncreaseStudentYear(Guid id)
        {
            var student = await _db.Users.FindAsync();
            if (student == null || !student.IsActive)
                return NotFound();
            if (student.Role != (int)Roles.Öğrenci)
                return BadRequest("Kullanıcı bir öğrenci değil.");
            student.StudentYear++;
            await _db.SaveChangesAsync();
            return Ok(new { Message = "Öğrenci sınıfı güncellendi" });
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("test-email")]
        public async Task<IHttpActionResult> TestEmail([FromBody] TestEmailDto dto)
        {
            if (string.IsNullOrEmpty(dto.email.Trim()))
                return Ok(new { status = true });

            bool status = await CheckUserData.CheckEmailAddressAsync(dto.email.Trim());
            return Ok(new { status });
        }


        private async Task SendPasswordResetEmail(string toEmail, string resetLink)
        {
            try
            {
                var templatePath =
                    System.Web.Hosting.HostingEnvironment.MapPath("~/Views/password-reset-template.html");
                var htmlTemplate = File.ReadAllText(templatePath);
                htmlTemplate = htmlTemplate.Replace("[Sifirlama_Linki]", resetLink);
                htmlTemplate = htmlTemplate.Replace("[Geçerlilik Süresi]", "15");
                await MailService.Instance.SendEmailAsync(toEmail, "Parola Sıfırlama Talebi", htmlTemplate);
            }
            catch (Exception ex)
            {
                Logger.Error("Mail Gönderimi Sırasında Hata: ", ex);
                throw;
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
                .Select(u => new SelectListItem { Value = u.UserId.ToString(), Text = u.Name + " " + u.Surname });
        }

        private async Task<string> CreateStudentNo(RegisterDataDto model)
        {
            if (model.Role == (int)Roles.Öğrenci)
            {
                var currentYear = DateTime.Now.ToString("yy");
                var department = await _db.Departments.FindAsync(model.DepartmentId);
                if (department == null || string.IsNullOrEmpty(department.DepartmentIdGen))
                    return null;
                var depIdGen = department.DepartmentIdGen;
                var numPrefix = $"{currentYear}{depIdGen}";

                var lastStudentOnDep = await _db.Users.Where(u => u.StudentNo.StartsWith(numPrefix))
                    .OrderByDescending(u => u.StudentNo).FirstOrDefaultAsync();

                int nextSeq = 1;
                if (lastStudentOnDep != null)
                {
                    var lastSequenceStr = lastStudentOnDep.StudentNo.Substring(numPrefix.Length);
                    nextSeq = int.Parse(lastSequenceStr) + 1;
                }

                return $"{numPrefix}{nextSeq:D3}";
            }
            else
                return null;
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("get-by-department/{departmentId:guid}")] // Route'u daha açıklayıcı hale getirdik
        public async Task<IHttpActionResult> GetByDepartment(Guid departmentId)
        {
            if (departmentId == Guid.Empty)
            {
                return BadRequest("Bölüm ID'si geçersiz.");
            }

            var advisors = await _db.Users
                .Where(u => u.DepartmentId == departmentId && u.Role == (int)Roles.Danışman)
                .Select(a => new SelectListItem()
                {
                    Text = a.Name + " " + a.Surname,
                    Value = a.UserId.ToString(),
                })
                .ToListAsync();

            return Ok(advisors);
        }
    }
}