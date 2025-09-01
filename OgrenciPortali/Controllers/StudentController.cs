using log4net;
using OgrenciPortali.Attributes;
using OgrenciPortali.ViewModels;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using Newtonsoft.Json;
using OgrenciPortali.Utils;
using Shared.DTO;
using Shared.Enums;

namespace OgrenciPortali.Controllers
{
    [CustomAuth(Roles.Öğrenci)]
    public class StudentController : Controller
    {
        private readonly ApiClient _apiClient;
        private readonly IMapper _mapper;

        public StudentController(ApiClient apiClient, IMapper mapper)
        {
            _apiClient = apiClient;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult> Enroll()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "api/student/enroll");
            var response = await _apiClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var dto = await response.Content.ReadAsAsync<EnrollPageDTO>();
                var viewModel = _mapper.Map<EnrollmentPageViewModel>(dto);
                return View(dto);
            }

            ModelState.AddModelError("", @"Seçilebilen dersler çekilirken hata oluştu.");
            return View(new EnrollPageDTO());
        }

        /// <summary>
        /// Öğrencinin seçtiği dersleri görüntüler
        /// </summary>
        public async Task<ActionResult> MyCourses()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "api/student/my-courses");
            var response = await _apiClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var dto = JsonConvert.DeserializeObject<MyCoursesDTO>(json);
                return View(dto);
            }


            return View(new MyCoursesDTO(){Courses = new List<MyCourseDto>()});
        }
    }
}