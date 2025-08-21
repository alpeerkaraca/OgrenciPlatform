using log4net;
using OgrenciPortali.Attributes;
using System;
using Shared.Enums;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using Newtonsoft.Json;
using OgrenciPortali.Utils;
using Shared.DTO;

namespace OgrenciPortali.Controllers
{
    /// <summary>
    /// Danýþman iþlemlerini yöneten controller sýnýfý
    /// </summary>
    [CustomAuth(Roles.Danýþman)]
    public class AdvisorController : Controller
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(CoursesController));
        private static ApiClient _apiClient;
        private readonly ApiClient _apiClient;
        private readonly IMapper _mapper;

        public AdvisorController(ApiClient apiClient, IMapper mapper)
        {
            _apiClient = apiClient;
            _mapper = mapper;
        }


        public async Task<ActionResult> CourseApprovals()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "api/advisor/approvals");
            var response = await _apiClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var dto = await response.Content.ReadAsAsync<AdvisorApprovalDTO>();
                return View(dto);
            }

            return View(new AdvisorApprovalDTO());
        }

        public async Task<ActionResult> AdvisedStudentList()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "api/advisor/students");
            var response = await _apiClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var dto = await response.Content.ReadAsAsync<AdvisedStudentsDTO>();
                return View(dto);
            }

            return View(new AdvisedStudentsDTO
            {
                AdvisedStudents = new List<StudentInfoDTO>()
            });
        }

        /// <summary>
        /// Öðrenci detay sayfasýný görüntüler
        /// </summary>
        public async Task<ActionResult> StudentDetail(Guid id)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"api/advisor/student/{id}");
            var response = await _apiClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var studentDto = await response.Content.ReadAsAsync<StudentDetailDto>();
                return View(studentDto);
            }

            return View(new StudentDetailDto() { StudentCourses = new List<StudentCourseInfoDto>() });
        }
    }
}