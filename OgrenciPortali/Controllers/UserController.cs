using AutoMapper;
using Azure;
using log4net;
using Newtonsoft.Json;
using OgrenciPortali.Attributes;
using OgrenciPortali.Utils;
using OgrenciPortali.ViewModels;
using Shared.DTO;
using Shared.Enums;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Remoting.Messaging;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;

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
                Content = new StringContent(JsonConvert.SerializeObject(loginUserDto), Encoding.UTF8,
                    "application/json"),
            };

            var response = await _apiClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsAsync<LoginSuccessResponse>();
                if (result == null || string.IsNullOrEmpty(result.AccessToken))
                {
                    ModelState.AddModelError("",
                        "Giriş işlemi başarısız oldu. Lütfen bilgilerinizi kontrol edin ve tekrar deneyin.");
                    return View(viewModel);
                }

                var userCookie = new HttpCookie("AuthToken", result.AccessToken)
                {
                    HttpOnly = true,
                    Secure = Request.IsSecureConnection,
                    Expires = DateTime.Now.AddMinutes(15),
                    Path = "/",
                    SameSite = SameSiteMode.Strict,
                };

                var refreshUserCookie = new HttpCookie("RefreshToken", result.RefreshToken)
                {
                    HttpOnly = true,
                    Secure = Request.IsSecureConnection,
                    Expires = DateTime.Now.AddDays(7),
                    Path = "/",
                    SameSite = SameSiteMode.Strict,
                };

                Response.Cookies.Add(userCookie);
                Response.Cookies.Add(refreshUserCookie);

                return RedirectToAction("Index", "Home");
            }

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                var json = await response.Content.ReadAsAsync<dynamic>();
                ModelState.AddModelError("", json.Message.ToString());
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
                return Json(new { success = true, message = "Şifreniz başarıyla değiştirildi." });
            }

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                return Json(new { success = false, message = "Mevcut şifre yanlış." });
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
            // Password reminder dismissed
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
        [ValidateAntiForgeryToken]
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
                return Json(new { success = true, message = "Kullanıcı başarıyla kaydedildi." });
            }

            return Json(new
            {
                success = false,
                message = "Kullanıcı kaydı sırasında bir hata oluştu. Lütfen daha sonra tekrar deneyin."
            });
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
                var claims = User.Identity as ClaimsIdentity;
                var isSsoUser = Request.Cookies[".AspNet.Cookies"] != null;
                if (isSsoUser)
                    HttpContext.GetOwinContext().Authentication.SignOut(
                        CookieAuthenticationDefaults.AuthenticationType,
                        OpenIdConnectAuthenticationDefaults.AuthenticationType);

                var request = new HttpRequestMessage(HttpMethod.Post, "api/auth/logout");
                var response = await _apiClient.SendAsync(request);
                if (!response.IsSuccessStatusCode)
                    Logger.Warn(
                        $"API'den çıkış yapılırken beklenmedik bir durum oluştu. Status: {response.StatusCode}");
            }
            catch (Exception ex)
            {
                Logger.Error("Logout sırasında API isteği başarısız oldu.", ex);
            }
            finally
            {
                if (Request.Cookies["AuthToken"] != null)
                {
                    // Cookie'yi silmenin en doğru yolu, süresi geçmiş ve içi boş bir cookie basmaktır.
                    var authTokenCookie = new HttpCookie("AuthToken", "")
                    {
                        Expires = DateTime.Now.AddDays(-1),
                        HttpOnly = true, // Güvenlik için önemli
                        Path = "/"
                    };
                    Response.Cookies.Set(authTokenCookie);
                }

                if (Request.Cookies["RefreshToken"] != null)
                {
                    var refreshTokenCookie = new HttpCookie("RefreshToken", "")
                    {
                        Expires = DateTime.Now.AddDays(-1),
                        HttpOnly = true, // Güvenlik için önemli
                        Path = "/"
                    };
                    Response.Cookies.Set(refreshTokenCookie);
                }

                Session.Clear();
                Session.Abandon();
            }

            return RedirectToAction("Login", "User");
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

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult> ResetPassword(string token)
        {
            var model = new ResetPasswordViewModel();
            if (string.IsNullOrEmpty(token))
            {
                model.ErrorMessage = "Geçersiz şifre sıfırlama bağlantısı. Anasayfaya yönlendiriliyorsunuz.";
                return RedirectToAction("Login");
            }

            model.Token = token;
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateJsonAntiForgeryToken]
        public async Task<JsonResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errorMessages = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                Response.StatusCode = 400;
                return Json(new { success = false, message = string.Join(" ", errorMessages) });
            }

            try
            {
                var request = new HttpRequestMessage(HttpMethod.Post, "api/user/reset-password")
                {
                    Content = new StringContent(
                        JsonConvert.SerializeObject(new ResetPasswordRequestDto
                            { NewPassword = model.NewPassword, Token = model.Token, ConfirmPassword = model.ConfirmPassword}), Encoding.UTF8,
                        "application/json"),
                };

                var res = await _apiClient.SendAsync(request);
                if (res.IsSuccessStatusCode)
                {
                    return Json(new { success = true });
                }
                else
                {
                    var errorResponse = await res.Content.ReadAsAsync<dynamic>();
                    Response.StatusCode = (int)res.StatusCode;
                    return Json(new
                    {
                        success = false, message = errorResponse?.Message ?? "Şifre sıfırlanırken bir hata oluştu."
                    });
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Parola sıfırlarken hata: ", ex);
                Response.StatusCode = 500;
                return Json(new { success = false, message = "Sunucuda beklenmedik bir hata oluştu." });
            }
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