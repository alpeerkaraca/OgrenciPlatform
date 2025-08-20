using Azure;
using Newtonsoft.Json;
using OgrenciPortali.Attributes;
using OgrenciPortali.Utils;
using OgrenciPortali.ViewModels;
using Shared.DTO;
using Shared.Enums;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using log4net;

namespace OgrenciPortali.Controllers
{
    /// <summary>
    /// Kullanıcı yönetimi işlemlerini gerçekleştiren controller sınıfı
    /// </summary>
    public class UserController : BaseController
    {
        private static readonly HttpClient client = new HttpClient()
        {
            BaseAddress = new Uri(AppSettings.ApiBaseAddress)
        };

        private static readonly ILog Logger = LogManager.GetLogger(typeof(UserController));

        /// <summary>
        /// Kullanıcı giriş sayfasını görüntüler
        /// </summary>
        [AllowAnonymous]
        public ActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            var viewModel = new LoginUserViewModel();
            return View(viewModel);
        }

        /// <summary>
        /// Kullanıcı giriş işlemini gerçekleştirir
        /// </summary>
        /// 
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Login(LoginUserViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return View(viewModel);
            var response = await client.PostAsJsonAsync("api/user/login",
                new LoginRequestDTO { Email = viewModel.Email, Password = viewModel.Password });

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsAsync<LoginSuccessResponse>();
                if (result != null && !string.IsNullOrEmpty(result.Token))
                {
                    var userCookie = new HttpCookie("AuthToken", result.Token)
                    {
                        HttpOnly = true,
                        Secure = Request.IsSecureConnection,
                        Expires = DateTime.Now.AddMinutes(15),
                        Path = "/",
                        SameSite = SameSiteMode.Strict,
                    };

                    Response.Cookies.Add(userCookie);
                }

                //return View("LoginSuccess");
                return RedirectToAction("Index", "Home");
            }
            else if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                ModelState.AddModelError("", "E-posta veya şifre hatalı.");
                return View(viewModel);
            }
            else
            {
                ModelState.AddModelError("",
                    "Giriş işlemi sırasında bir hata oluştu. Lütfen daha sonra tekrar deneyin.");
                return View(viewModel);
            }
        }

        /// <summary>
        /// Şifre değiştirme işlemini gerçekleştirir
        /// </summary>
        /// 
        [HttpPost]
        //[ValidateAntiForgeryToken]
        [CustomAuth]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var authenticatedClient = await GetAuthenticatedHttpClient();

            var currentUser = User.Identity as ClaimsIdentity;
            var userIdClaim = currentUser?.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (userIdClaim == null)
                return View(model);

            Guid userId = Guid.Parse(userIdClaim);
            var response = await authenticatedClient.PostAsJsonAsync("api/user/ChangePassword",
                new ChangePasswordRequestDTO()
                {
                    UserId = userId,
                    CurrentPassword = model.CurrentPassword,
                    NewPassword = model.NewPassword,
                });

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Şifreniz başarıyla değiştirildi.";
            }
            else if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                ModelState.AddModelError("", "Mevcut şifre yanlış.");
                return View(model);
            }
            else
            {
                ModelState.AddModelError("",
                    "Şifre değiştirme işlemi sırasında bir hata oluştu. Lütfen daha sonra tekrar deneyin.");
                return View(model);
            }

            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// Şifre değiştirme sayfasını görüntüler
        /// </summary>
        [CustomAuth]
        public ActionResult ChangePassword()
        {
            return View();
        }

        /// <summary>
        /// Şifre değiştirme uyarısını geçici olarak kapatır
        /// </summary>
        [HttpPost]
        [CustomAuth]
        public ActionResult DismissPasswordReminder()
        {
            TempData["PasswordChangeReminder"] = "dismissed";
            return Json(new { success = true });
        }

        /// <summary>
        /// Şifre değiştirme modal'ının gösterildiğini işaretler
        /// </summary>
        [HttpPost]
        [CustomAuth]
        public ActionResult MarkPasswordModalShown()
        {
            Session["PasswordChangeModalShown"] = true;
            return Json(new { success = true });
        }

        [CustomAuth(Roles.Admin)]
        public async Task<ActionResult> Register()
        {
            RegisterViewModel viewModel = new RegisterViewModel();
            var authenticatedClient = await GetAuthenticatedHttpClient();
            var response =
                await authenticatedClient.GetAsync("api/user/register");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var registerDto = JsonConvert.DeserializeObject<RegisterDataDto>(json);
                viewModel.RolesList = new SelectList(registerDto.RolesList, "Value", "Text");
                viewModel.DepartmentsList = new SelectList(registerDto.DepartmentsList, "Value", "Text");
                viewModel.AdvisorsList = new SelectList(registerDto.AdvisorsList, "Value", "Text");
            }
            else
                ModelState.AddModelError("", $"API'den veri alınamadı. Hata Kodu: {response.StatusCode}");

            return View(viewModel);
        }

        /// <summary>
        /// Yeni kullanıcı kayıt işlemini gerçekleştirir
        /// </summary>
        [HttpPost]
        //[ValidateAntiForgeryToken]
        [CustomAuth(Roles.Admin)]
        public async Task<ActionResult> Register(RegisterViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return View(viewModel);

            var authenticatedClient = await GetAuthenticatedHttpClient();
            var registerDataDto = new RegisterDataDto()
            {
                AdvisorId = viewModel.AdvisorId,
                DepartmentId = viewModel.DepartmentId,
                Email = viewModel.Email,
                Name = viewModel.Name,
                Password = viewModel.Password,
                Role = viewModel.Role,
                Surname = viewModel.Surname,
                StudentNo = viewModel.StudentNo,
            };
            var response = await authenticatedClient.PostAsJsonAsync("/api/user/register",
                registerDataDto);


            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Kullanıcı başarıyla kaydedildi.";
                return RedirectToAction("List", "User");
            }
            else
            {
                ModelState.AddModelError("",
                    "Kullanıcı kaydı sırasında bir hata oluştu. Lütfen daha sonra tekrar deneyin.");
                return View(viewModel);
            }
        }

        /// <summary>
        /// Kullanıcı listesini görüntüler (Admin yetkisi gerekir)
        /// </summary>
        [CustomAuth(Roles.Admin)]
        public async Task<ActionResult> List()
        {
            var authenticatedClient = await GetAuthenticatedHttpClient();
            var response = await authenticatedClient.GetAsync("/api/user/list");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var userList = JsonConvert.DeserializeObject<List<UserDTO>>(json);
                return View(userList);
            }

            return View(new List<UserDTO>());
        }

        /// <summary>
        /// Kullanıcı çıkış işlemini gerçekleştirir
        /// </summary>
        [CustomAuth]
        public async Task<ActionResult> Logout()
        {
            try
            {
                var authenticatedClient = await GetAuthenticatedHttpClient();
                await authenticatedClient.PostAsync("api/auth/logout", null);

                // Tarayıcıdaki cookie'leri temizle
                if (Request.Cookies["AuthToken"] != null)
                {
                    Response.Cookies.Add(new HttpCookie("AuthToken", "") { Expires = DateTime.Now.AddDays(-1) });
                }

                if (Request.Cookies["RefreshToken"] != null)
                {
                    Response.Cookies.Add(new HttpCookie("RefreshToken", "") { Expires = DateTime.Now.AddDays(-1) });
                }

                Session.Clear();
                TempData.Clear();
                if (Request.Cookies["AuthToken"] != null)
                {
                    var cookie = new HttpCookie("AuthToken", "")
                    {
                        Expires = DateTime.Now.AddDays(-1),
                        HttpOnly = true,
                        Path = "/"
                    };
                    Response.Cookies.Add(cookie);
                }

                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
                Response.Cache.SetNoStore();

                return RedirectToAction("Login", "User");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Login", "User");
            }
        }

        /// <summary>
        /// Kullanıcı düzenleme sayfasını görüntüler
        /// </summary>
        [CustomAuth(Roles.Admin)]
        public async Task<ActionResult> Edit(Guid id)
        {
            var authenticatedClient = await GetAuthenticatedHttpClient();
            var response = await authenticatedClient.GetAsync($"api/user/edit/{id}");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<EditUserDTO>(json);
                var vm = new UpdateUserViewModel
                {
                    UserId = result.UserId,
                    Name = result.Name,
                    Surname = result.Surname,
                    Email = result.Email,
                    StudentNo = result.StudentNo,
                    Role = result.Role,
                    AdvisorId = result.AdvisorId,
                    AdvisorsList = result.AdvisorsList,
                    DepartmentsList = result.DepartmentsList,
                    RolesList = result.RolesList,
                    DepartmentId = result.DepartmentId,
                };
                return View(vm);
            }

            return View(new UpdateUserViewModel());
        }

        /// <summary>
        /// Kullanıcı düzenleme işlemini gerçekleştirir
        /// </summary>
        [CustomAuth(Roles.Admin)]
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(UpdateUserViewModel viewModel)
        {
            var authenticatedClient = await GetAuthenticatedHttpClient();
            var dto = new EditUserDTO()
            {
                Name = viewModel.Name,
                Email = viewModel.Email,
                Role = viewModel.Role,
                DepartmentId = viewModel.DepartmentId,
                StudentNo = viewModel.StudentNo,
                AdvisorId = viewModel.AdvisorId,
                Surname = viewModel.Surname,
                UserId = viewModel.UserId,
            };
            var response = await authenticatedClient.PostAsJsonAsync("api/user/edit", dto);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Edit", "User");
            }

            if (response.StatusCode == HttpStatusCode.BadRequest)
                ModelState.AddModelError("", "Bu bilgilere ait bir kullanıcı sistemde mevcut.");
            if (response.StatusCode == HttpStatusCode.InternalServerError)
                ModelState.AddModelError("", "Kullanıcı güncellenirken bir hata oluştu. Tekrar deneyin.");
            return View(viewModel);
        }

        /// <summary>
        /// Kullanıcı şifresinin doğruluğunu kontrol eder
        /// </summary>
        public bool IsUserPasswordValid(string password, string hashInDb)
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(hashInDb))
            {
                return false;
            }

            return BCrypt.Net.BCrypt.Verify(password, hashInDb);
        }

        /// <summary>
        /// Mevcut kullanıcının token'ını okuyarak API istekleri için yetkilendirilmiş bir HttpClient nesnesi döner.
        /// </summary>
        private async Task<HttpClient> GetAuthenticatedHttpClient(bool forceRefresh = false)
        {
            var tokenCookie = Request.Cookies["AuthToken"];

            if (forceRefresh || tokenCookie == null || string.IsNullOrEmpty(tokenCookie.Value))
            {
                var refreshSuccess = await RefreshAccessTokenAsync();
                if (!refreshSuccess)
                {
                    client.DefaultRequestHeaders.Authorization = null;
                    return client;
                }

                tokenCookie = Request.Cookies["AuthToken"];
            }

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenCookie.Value);
            return client;
        }

        /// <summary>
        /// Kullanıcının tokeninin süresi dolduysa refresh tokenini kullanarak yeni bir erişim tokeni döndürür.
        /// </summary>>
        private async Task<bool> RefreshAccessTokenAsync()
        {
            try
            {
                var response = await client.PostAsync("api/auth/refresh-token", null);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    var tokenData = JsonConvert.DeserializeObject<dynamic>(result);
                    var newAuthCookie = new HttpCookie("AuthToken", tokenData.accessToken)
                    {
                        HttpOnly = true,
                        Secure = Request.IsSecureConnection,
                        Expires = DateTime.UtcNow.AddMinutes(Convert.ToInt32(15)),
                        Path = "/"
                    };
                    Response.SetCookie(newAuthCookie);
                    return true;
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Token Yenilerken Hata: ", ex);
            }

            return false;
        }
    }
}