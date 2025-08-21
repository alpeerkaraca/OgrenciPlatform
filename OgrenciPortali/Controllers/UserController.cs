using Newtonsoft.Json;
using OgrenciPortali.Attributes;
using OgrenciPortali.ViewModels;
using Shared.DTO;
using Shared.Enums;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using log4net;
using OgrenciPortali.Utils;

namespace OgrenciPortali.Controllers
{
    /// <summary>
    /// Kullanıcı yönetimi işlemlerini gerçekleştiren controller sınıfı
    /// </summary>
    public class UserController : BaseController
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(UserController));
        private readonly ApiClient _apiClient;
        private readonly IMapper _mapper;

        public UserController(ApiClient apiClient, IMapper mapper)
        {
            _apiClient = apiClient;
            _mapper = mapper;
        }


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
            var loginUserDto = _mapper.Map<LoginUserDTO>(viewModel);
            var request = new HttpRequestMessage(HttpMethod.Post, "api/user/login")
            {
                Content = new StringContent(JsonConvert.SerializeObject(loginUserDto), System.Text.Encoding.UTF8,
                    "application/json"),
            };

            var response = await _apiClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsAsync<LoginSuccessResponse>();
                if (result == null || string.IsNullOrEmpty(result.Token)) return RedirectToAction("Index", "Home");
                var userCookie = new HttpCookie("AuthToken", result.Token)
                {
                    HttpOnly = true,
                    Secure = Request.IsSecureConnection,
                    Expires = DateTime.Now.AddMinutes(15),
                    Path = "/",
                    SameSite = SameSiteMode.Strict,
                };

                Response.Cookies.Add(userCookie);

                return RedirectToAction("Index", "Home");
            }

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                ModelState.AddModelError("", "E-posta veya şifre hatalı.");
                return View(viewModel);
            }

            ModelState.AddModelError("",
                "Giriş işlemi sırasında bir hata oluştu. Lütfen daha sonra tekrar deneyin.");
            return View(viewModel);
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

            var currentUser = User.Identity as ClaimsIdentity;
            var userIdClaim = currentUser?.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (userIdClaim == null)
                return View(model);

            var changePasswordDto = _mapper.Map<ChangePasswordDTO>(model);
            changePasswordDto.UserId = Guid.Parse(userIdClaim);

            var request = new HttpRequestMessage(HttpMethod.Post, "api/user/change-password")
            {
                Content = new StringContent(JsonConvert.SerializeObject(changePasswordDto), Encoding.UTF8,
                    "application/json")
            };

            var response = await _apiClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Şifreniz başarıyla değiştirildi.";
                return RedirectToAction("Index", "Home");
            }

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                ModelState.AddModelError("", "Mevcut şifre yanlış.");
                return View(model);
            }

            ModelState.AddModelError("",
                "Şifre değiştirme işlemi sırasında bir hata oluştu. Lütfen daha sonra tekrar deneyin.");
            return View(model);
        }

        /// <summary>
        /// Şifre değiştirme sayfasını görüntüler
        /// </summary>
        [CustomAuth]
        public ActionResult ChangePassword()
        {
            return View(new ChangePasswordViewModel());
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
            var viewModel = new RegisterViewModel();
            var request = new HttpRequestMessage(HttpMethod.Get, "api/user/register");
            var response =
                await _apiClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var registerDto = await response.Content.ReadAsAsync<RegisterDataDto>();
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
            {
                viewModel = await FillModel(viewModel);

                return View(viewModel);
            }

            var registerDataDto = _mapper.Map<RegisterDataDto>(viewModel);
            var request = new HttpRequestMessage(HttpMethod.Post, "api/user/register")
            {
                Content = new StringContent(JsonConvert.SerializeObject(registerDataDto), Encoding.UTF8,
                    "application/json")
            };

            var response = await _apiClient.SendAsync(request);


            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Kullanıcı başarıyla kaydedildi.";
                return RedirectToAction("List", "User");
            }

            ModelState.AddModelError("",
                "Kullanıcı kaydı sırasında bir hata oluştu. Lütfen daha sonra tekrar deneyin.");
            viewModel = await FillModel(viewModel);
            return View(viewModel);
        }

        /// <summary>
        /// Kullanıcı listesini görüntüler (Admin yetkisi gerekir)
        /// </summary>
        [CustomAuth(Roles.Admin)]
        public async Task<ActionResult> List()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/user/list");
            var response = await _apiClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var userList = await response.Content.ReadAsAsync<List<UserDTO>>();
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
                var request = new HttpRequestMessage(HttpMethod.Post, "api/auth/logout")
                {
                    Content = null
                };

                var response = await _apiClient.SendAsync(request);

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
        [HttpGet]
        [CustomAuth(Roles.Admin)]
        public async Task<ActionResult> Edit(Guid id)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"api/user/edit/{id}");
            var response = await _apiClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var editUserDto = await response.Content.ReadAsAsync<EditUserDTO>();
                var viewModel = _mapper.Map<UpdateUserViewModel>(editUserDto);
                return View(viewModel);
            }

            return View("Error");
        }

        /// <summary>
        /// Kullanıcı düzenleme işlemini gerçekleştirir
        /// </summary>
        [CustomAuth(Roles.Admin)]
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(UpdateUserViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Model verileri geçersiz.");
                viewModel = await FillModel(viewModel);
                return View(viewModel);
            }

            var dto = _mapper.Map<EditUserDTO>(viewModel);
            var request = new HttpRequestMessage(HttpMethod.Post, "api/user/edit")
            {
                Content = new StringContent(JsonConvert.SerializeObject(dto), Encoding.UTF8, "application/json"),
            };

            var response = await _apiClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Edit", "User");
            }

            if (response.StatusCode == HttpStatusCode.BadRequest)
                ModelState.AddModelError("", "Bu bilgilere ait bir kullanıcı sistemde mevcut.");
            if (response.StatusCode == HttpStatusCode.InternalServerError)
                ModelState.AddModelError("", "Kullanıcı güncellenirken bir hata oluştu. Tekrar deneyin.");
            else
            {
                var msg = await response.Content.ReadAsAsync<dynamic>();
                var message = msg.Message;
                ModelState.AddModelError("", message.ToString());
            }

            viewModel = await FillModel(viewModel);
            return View(viewModel);
        }

        private async Task<RegisterViewModel> FillModel(RegisterViewModel viewModel)
        {
            var request_back = new HttpRequestMessage(HttpMethod.Get, "api/user/register");
            var response_back =
                await _apiClient.SendAsync(request_back);

            if (response_back.IsSuccessStatusCode)
            {
                var registerDto = await response_back.Content.ReadAsAsync<RegisterDataDto>();
                viewModel.RolesList = new SelectList(registerDto.RolesList, "Value", "Text");
                viewModel.DepartmentsList = new SelectList(registerDto.DepartmentsList, "Value", "Text");
                viewModel.AdvisorsList = new SelectList(registerDto.AdvisorsList, "Value", "Text");
            }

            return viewModel;
        }

        private async Task<UpdateUserViewModel> FillModel(UpdateUserViewModel viewModel)
        {
            var request_back = new HttpRequestMessage(HttpMethod.Get, "api/user/register");
            var response_back =
                await _apiClient.SendAsync(request_back);

            if (response_back.IsSuccessStatusCode)
            {
                var registerDto = await response_back.Content.ReadAsAsync<RegisterDataDto>();
                viewModel.RolesList = new SelectList(registerDto.RolesList, "Value", "Text");
                viewModel.DepartmentsList = new SelectList(registerDto.DepartmentsList, "Value", "Text");
                viewModel.AdvisorsList = new SelectList(registerDto.AdvisorsList, "Value", "Text");
            }

            return viewModel;
        }
    }
}