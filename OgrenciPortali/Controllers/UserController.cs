using OgrenciPortali.Attributes;
using OgrenciPortali.ViewModels;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using Shared.DTO;
using Shared.Enums;

namespace OgrenciPortali.Controllers
{
    /// <summary>
    /// Kullanıcı yönetimi işlemlerini gerçekleştiren controller sınıfı
    /// </summary>
    public class UserController : BaseController
    {
        private readonly string _apiBaseAddress = Utils.AppSettings.ApiBaseAddress;

        /// <summary>
        /// Kullanıcı giriş sayfasını görüntüler
        /// </summary>
        [AllowAnonymous]
        public ActionResult Login()
        {
            // Check if user is already authenticated via Bearer token
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
        //[ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<ActionResult> Login(LoginUserViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return View(viewModel);

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_apiBaseAddress);
                var response = await client.PostAsJsonAsync("api/user/login",
                    new LoginRequestDTO { Email = viewModel.Email, Password = viewModel.Password });

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsAsync<LoginSuccessResponse>();

                    Session["bearerToken"] = result.Token;
                    TempData["BearerToken"] = result.Token;

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

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_apiBaseAddress);

                var token = GetCurrentUserToken();
                if (!string.IsNullOrEmpty(token))
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }

                var currentUser = User.Identity as ClaimsIdentity;
                var userIdClaim = currentUser?.FindFirst(ClaimTypes.NameIdentifier).Value;
                if (userIdClaim == null)
                    return View(model);

                Guid userId = Guid.Parse(userIdClaim);
                var response = await client.PostAsJsonAsync("api/user/ChangePassword",
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
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
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

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_apiBaseAddress);
                var token = GetCurrentUserToken();
                if (!string.IsNullOrEmpty(token))
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response =
                    await client.GetAsync("api/user/register");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();

                    var registerDto = JsonConvert.DeserializeObject<RegisterDataDto>(json);
                    viewModel.RolesList = new SelectList(registerDto.RolesList, "Value", "Text");
                    viewModel.DepartmentsList = new SelectList(registerDto.DepartmentsList, "Value", "Text");
                    viewModel.AdvisorsList = new SelectList(registerDto.AdvisorsList, "Value", "Text");
                }
                else
                    ModelState.AddModelError("Hata", $"API'den veri alınamadı. Hata Kodu: {response.StatusCode}");
            }

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

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_apiBaseAddress);
                var token = GetCurrentUserToken();
                if (!string.IsNullOrEmpty(token))
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
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
                var response = await client.PostAsJsonAsync("/api/user/register",
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
        }

        /// <summary>
        /// Kullanıcı listesini görüntüler (Admin yetkisi gerekir)
        /// </summary>
        [CustomAuth(Roles.Admin)]
        public async Task<ActionResult> List()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(ApiBaseAddress);
                var token = GetCurrentUserToken();
                if (!string.IsNullOrEmpty(token))
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await client.GetAsync("/api/user/list");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    //TODO:DTO ile yolla
                    var userList = JsonConvert.DeserializeObject<List<UserDTO>>(json);
                    return View(userList);
                }
            }

            return View(new List<UserDTO>());
        }

        /// <summary>
        /// Kullanıcı çıkış işlemini gerçekleştirir
        /// </summary>
        [CustomAuth]
        public ActionResult Logout()
        {
            try
            {
                Session.Clear();
                TempData.Clear();

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
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_apiBaseAddress);
                var token = GetCurrentUserToken();
                if (!string.IsNullOrEmpty(token))
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await client.GetAsync($"api/user/edit/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<UpdateUserViewModel>(json);
                    return View(result);
                }

                return View(new UpdateUserViewModel());
            }
        }

        /// <summary>
        /// Kullanıcı düzenleme işlemini gerçekleştirir
        /// </summary>
        [CustomAuth(Roles.Admin)]
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(UpdateUserViewModel viewModel)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_apiBaseAddress);
                var token = GetCurrentUserToken();
                if (!string.IsNullOrEmpty(token))
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
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
                var response = await client.PostAsJsonAsync("api/user/edit", dto);
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
        }


        /// <summary>
        /// Gets the current user's Bearer token from session/TempData
        /// </summary>
        private string GetCurrentUserToken()
        {
            var token = TempData["BearerToken"] as string ?? Session["bearerToken"] as string;

            // Keep token in TempData for next request
            if (!string.IsNullOrEmpty(token))
            {
                TempData.Keep("BearerToken");
            }

            return token;
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
    }
}