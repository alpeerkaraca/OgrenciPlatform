using log4net;
using OgrenciPortali.Attributes;
using OgrenciPortali.ViewModels;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Mvc;
using Newtonsoft.Json;
using Shared.DTO;
using Shared.Enums;

namespace OgrenciPortali.Controllers
{
    [CustomAuth(Roles.Öğrenci)]
    public class StudentController : Controller
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(StudentController));
        private readonly string _apiBaseAddress = Utils.AppSettings.ApiBaseAddress;

        [HttpGet]
        [CustomAuth(Roles.Öğrenci)]
        public async Task<ActionResult> Enroll()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_apiBaseAddress);
                var token = GetCurrentUserToken();
                if (!string.IsNullOrEmpty(token))
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await client.GetAsync("api/students/enroll");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var dto = JsonConvert.DeserializeObject<EnrollPageDTO>(json);
                    var viewModel = new EnrollmentPageViewModel()
                    {
                        EnrollableList = dto.EnrollableList,
                        PendingCourses = dto.PendingCourses,
                    };
                    return View(viewModel);
                }

                ModelState.AddModelError("", @"Seçilebilen dersler çekilirken hata oluştu.");
                return View(new EnrollmentPageViewModel());
            }
        }

        /// <summary>
        /// Öğrencinin seçtiği dersleri görüntüler
        /// </summary>
        [CustomAuth(Roles.Öğrenci)]
        public async Task<ActionResult> MyCourses()
        {

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_apiBaseAddress);
                var token = GetCurrentUserToken();
                if (!string.IsNullOrEmpty(token))
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await client.GetAsync("/api/students/my-courses");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var dto = JsonConvert.DeserializeObject<MyCoursesDTO>(json);
                    return View(dto);
                }
            }


            return View(new MyCoursesDTO());
        }


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
    }
}