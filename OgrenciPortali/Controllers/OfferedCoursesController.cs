using log4net;
using OgrenciPortali.Attributes;
using OgrenciPortali.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using Newtonsoft.Json;
using OgrenciPortali.Utils;
using Shared.DTO;
using Shared.Enums;

namespace OgrenciPortali.Controllers
{
    [CustomAuth(Roles.Admin)]
    public class OfferedCoursesController : Controller
    {
        private static ApiClient _apiClient;
        private static IMapper _mapper;

        public OfferedCoursesController(ApiClient apiClient, IMapper mapper)
        {
            _apiClient = apiClient;
            _mapper = mapper;
        }

        public async Task<ActionResult> List()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "api/offered");
            var response = await _apiClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var dto = await response.Content.ReadAsAsync<List<ListOfferedCoursesDTO>>();
                var viewModel = new ListOfferedCoursesViewModel { OfferedCoursesList = dto };
                return View(viewModel);
            }

            return View(new ListOfferedCoursesViewModel() { OfferedCoursesList = new List<ListOfferedCoursesDTO>() });
        }

        // GET: OfferedCourses/Add - Return data for modal or redirect to List
        public async Task<ActionResult> Add()
        {
            // If it's an AJAX request, return JSON data for the modal
            if (Request.IsAjaxRequest() || Request.Headers["Content-Type"] == "application/json" || Request.Headers["Accept"].Contains("application/json"))
            {
                var request = new HttpRequestMessage(HttpMethod.Get, "api/offered/add");
                var response = await _apiClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var dto = await response.Content.ReadAsAsync<AddOfferedCourseDTO>();
                    return Json(dto, JsonRequestBehavior.AllowGet);
                }
                return Json(new { error = "Unable to load form data" }, JsonRequestBehavior.AllowGet);
            }
            
            // Regular request - redirect to List page where modal will be available
            return RedirectToAction("List", "OfferedCourses");
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Add(AddOfferedCourseViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Model verisi geçersiz.");
                return View(await FillModel(viewModel));
            }

            var dto = _mapper.Map<AddOfferedCourseDTO>(viewModel);
            var request = new HttpRequestMessage(HttpMethod.Post, "api/offered/add")
            {
                Content = new StringContent(JsonConvert.SerializeObject(dto), Encoding.UTF8, "application/json")
            };

            var response = await _apiClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var message = await response.Content.ReadAsAsync<dynamic>();

                TempData["SuccessMessage"] = message.Message;

                return RedirectToAction("Add");
            }
            else
            {
                var errorContent = await response.Content.ReadAsAsync<dynamic>();
                string errorMessage = errorContent.Message;

                ModelState.AddModelError("", errorMessage);
            }

            return View(await FillModel(viewModel));
        }

        // GET: OfferedCourses/Edit/5 
        [CustomAuth(Roles.Admin)]
        public async Task<ActionResult> Edit(Guid id)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"api/offered/edit/{id}");
            var response = await _apiClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var dto = await response.Content.ReadAsAsync<EditOfferedCoursesDTO>();
                var viewModel = _mapper.Map<EditOfferedCourseViewModel>(dto);
                return View(viewModel);
            }

            ModelState.AddModelError("", @"Sayfa yüklenirken bir hata oluştu. Daha sonra tekrar deneyiniz.");
            var model = new EditOfferedCourseViewModel
            {
                CourseList = new SelectList(Enumerable.Empty<SelectListItem>()),
                SemesterList = new SelectList(Enumerable.Empty<SelectListItem>()),
                AdvisorList = new SelectList(Enumerable.Empty<SelectListItem>()),
                DaysList = new SelectList(Enumerable.Empty<SelectListItem>())
            };
            return View(model);
        }

        // POST: OfferedCourses/Edit
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(EditOfferedCourseViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Model verisi geçersiz.");
                return View(viewModel);
            }


            var dto = _mapper.Map<EditOfferedCoursesDTO>(viewModel);
            var request = new HttpRequestMessage(HttpMethod.Post, "api/offered/edit")
            {
                Content = new StringContent(JsonConvert.SerializeObject(dto), Encoding.UTF8, "application/json")
            };
            var response = await _apiClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                TempData.Add("SuccessMessage", "Dönem dersi başarıyla güncellendi.");
            }
            else
            {
                ModelState.AddModelError("", "Dönem dersi güncellenirken bir hata oluştu.");
            }


            return RedirectToAction("List", "OfferedCourses");
        }


        private async Task<AddOfferedCourseViewModel> FillModel(AddOfferedCourseViewModel viewModel)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "api/offered/add");
            var response = await _apiClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var dto = await response.Content.ReadAsAsync<AddOfferedCourseDTO>();
                viewModel.DaysList = new SelectList(dto.DaysOfWeek, "Value", "Text");
                viewModel.AdvisorList = new SelectList(dto.AdvisorList, "Value", "Text");
                viewModel.CourseList = new SelectList(dto.CourseList, "Value", "Text");
                viewModel.SemesterList = new SelectList(dto.SemesterList, "Value", "Text");
            }
            else
                ModelState.AddModelError("", @"Veri çekilirken hata oluştu. Tekrar deneyiniz.");

            return viewModel;
        }
    }
}