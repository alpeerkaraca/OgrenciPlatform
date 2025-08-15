using log4net;
using OgrenciPortali.Attributes;
using OgrenciPortali.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Mvc;
using Newtonsoft.Json;
using OgrenciPortali.DTOs;

namespace OgrenciPortali.Controllers
{
    /// <summary>
    /// Danýþman iþlemlerini yöneten controller sýnýfý
    /// </summary>
    [CustomAuth(Roles.Danýþman)]
    public class AdvisorController : Controller
    {
        private readonly OgrenciPortalContext db = new OgrenciPortalContext();
        private static readonly ILog Logger = LogManager.GetLogger(typeof(CoursesController));
        private readonly string _apiBaseAddress = Utils.AppSettings.ApiBaseAddress;


        public async Task<ActionResult> CourseApprovals()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_apiBaseAddress);
                var token = GetCurrentUserToken();
                if (!string.IsNullOrEmpty(token))
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await client.GetAsync("api/advisor/approvals");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var dto = JsonConvert.DeserializeObject<AdvisorApprovalDTO>(json);
                    return View(dto);
                }

                return View(new AdvisorApprovalDTO());
            }
        }

        public async Task<ActionResult> AdvisedStudentList()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_apiBaseAddress);
                var token = GetCurrentUserToken();
                if (!string.IsNullOrEmpty(token))
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await client.GetAsync("api/advisor/students");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var dto = JsonConvert.DeserializeObject<AdvisedStudentsDTO>(json);
                    return View(dto);
                }
            }

            return View(new AdvisedStudentsDTO
            {
                AdvisedStudents = new List<StudentInfoDTO>()
            });
        }

        /// <summary>
        /// Öðrenci detay sayfasýný görüntüler
        /// </summary>
        [CustomAuth(Roles.Danýþman)]
        public async Task<ActionResult> StudentDetail(Guid id)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_apiBaseAddress);
                var token = GetCurrentUserToken();
                if (!string.IsNullOrEmpty(token))
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await client.GetAsync($"api/advisor/student/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var studentDto = JsonConvert.DeserializeObject<StudentDetailDto>(json);
                    return View(studentDto);
                }

                return View(new StudentDetailDto() { StudentCourses = new List<StudentCourseInfoDto>() });
            }
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

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}