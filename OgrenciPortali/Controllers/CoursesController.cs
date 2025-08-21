using log4net;
using OgrenciPortali.Attributes;
using Shared.DTO;
using OgrenciPortali.Utils;
using OgrenciPortali.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using Newtonsoft.Json;
using Shared.Enums;

namespace OgrenciPortali.Controllers
{
    [CustomAuth(Roles.Admin)]
    public class CoursesController : Controller
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(CoursesController));
        private static ApiClient _apiClient;
        private static IMapper _mapper;

        public CoursesController(ApiClient apiClient, IMapper mapper)
        {
            _apiClient = apiClient;
            _mapper = mapper;
        }

        // GET: Courses/List
        public async Task<ActionResult> List()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "api/courses/list");
            var response = await _apiClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var dto = await response.Content.ReadAsAsync<List<ListCoursesDTO>>();
                return View(dto);
            }
            else
            {
                return null;
            }
        }

        // GET: Courses/Add
        public async Task<ActionResult> Add()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "api/courses/add");
            var response = await _apiClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var courseDataDto = await response.Content.ReadAsAsync<AddCourseDTO>();
                var viewModel = _mapper.Map<AddCourseViewModel>(courseDataDto);
                return View(viewModel);
            }

            var emptyViewModel = new AddCourseViewModel
            {
                DepartmentList = new SelectList(Enumerable.Empty<SelectListItem>())
            };
            return View(emptyViewModel);
        }

        // POST: Courses/Add
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Add(AddCourseViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                viewModel.DepartmentList = await GetDepListFromApiForAddCourses();
                return View(viewModel);
            }

            var addCourseDto = _mapper.Map<AddCourseDTO>(viewModel);
            var request = new HttpRequestMessage(HttpMethod.Post, "api/courses/add")
            {
                Content = new StringContent(JsonConvert.SerializeObject(addCourseDto), Encoding.UTF8,
                    "application/json"),
            };

            var response = await _apiClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Ders başarıyla eklendi.";
                return RedirectToAction("List", "Courses");
            }
            else
            {
                ModelState.AddModelError("", "Ders eklenirken bir hata oluştu.");

                viewModel.DepartmentList = await GetDepListFromApiForAddCourses();
                return View(viewModel);
            }
        }

        // GET: Courses/Edit/5
        [CustomAuth(Roles.Admin)]
        public async Task<ActionResult> Edit(Guid id)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"api/courses/edit/{id}");
            var res = await _apiClient.SendAsync(request);

            if (res.IsSuccessStatusCode)
            {
                var dto = await res.Content.ReadAsAsync<EditCourseDTO>();
                var viewModel = _mapper.Map<EditCourseViewModel>(dto);
                viewModel.DepartmentList = await GetDepListFromApiForAddCourses();
                return View(viewModel);
            }

            if (res.StatusCode == HttpStatusCode.BadRequest)
                ModelState.AddModelError("", "Gönderilen ID Geçersiz");
            if (res.StatusCode == HttpStatusCode.NotFound)
                ModelState.AddModelError("", "Ders Bulunamadı");

            return RedirectToAction("List", "Courses");
        }

        // POST: Courses/Edit
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(EditCourseViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Model Geçerli Değil");
                viewModel.DepartmentList = await GetDepListFromApiForAddCourses();
                return View(viewModel);
            }


            var dto = _mapper.Map<EditCourseDTO>(viewModel);
            var request = new HttpRequestMessage(HttpMethod.Post, "api/courses/edit")
            {
                Content = new StringContent(JsonConvert.SerializeObject(dto), Encoding.UTF8, "application/json"),
            };

            var res = await _apiClient.SendAsync(request);
            if (res.IsSuccessStatusCode)
            {
                return RedirectToAction("List", "Courses");
            }
            else
            {
                ModelState.AddModelError("", @"Ders Eklenirken Hata Oluştu");
                viewModel.DepartmentList = await GetDepListFromApiForAddCourses();
                return View(viewModel);
            }
        }

        private async Task<SelectList> GetDepListFromApiForAddCourses()
        {
            var res = await _apiClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, "/api/courses/add"));
            if (!res.IsSuccessStatusCode) return null;
            var json = await res.Content.ReadAsStringAsync();
            var courseDataDto = await res.Content.ReadAsAsync<AddCourseDTO>();
            return new SelectList(courseDataDto.DepartmentsList, "DepartmentId", "DepartmentName");
        }
    }
}