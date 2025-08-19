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
using System.Threading.Tasks;
using System.Web.Mvc;
using Newtonsoft.Json;
using Shared.Enums;

namespace OgrenciPortali.Controllers
{
    [CustomAuth(Roles.Admin)]
    public class CoursesController : Controller
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(CoursesController));
        private readonly string _apiBaseAddress = AppSettings.ApiBaseAddress;


        // GET: Courses/List
        public async Task<ActionResult> List()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_apiBaseAddress);
                var token = GetCurrentUserToken();
                if (!string.IsNullOrEmpty(token))
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await client.GetAsync("api/courses/list");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var dto = JsonConvert.DeserializeObject<List<ListCoursesDTO>>(json);
                    return View(dto);
                }
                else
                {
                    return null;
                }
            }
        }

        // GET: Courses/Add
        public async Task<ActionResult> Add()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_apiBaseAddress);
                var token = GetCurrentUserToken();
                if (!string.IsNullOrEmpty(token))
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await client.GetAsync("api/courses/add");
                if (response.IsSuccessStatusCode)
                {
                    var json = response.Content.ReadAsStringAsync().Result;
                    var courseDataDto = JsonConvert.DeserializeObject<AddCourseDTO>(json);
                    var viewModel = new AddCourseViewModel
                    {
                        DepartmentList = new SelectList(courseDataDto.DepartmentsList, "DepartmentId", "DepartmentName")
                    };
                    return View(viewModel);
                }
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
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_apiBaseAddress);
                    var token = GetCurrentUserToken();
                    if (!string.IsNullOrEmpty(token))
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                    viewModel.DepartmentList = await GetDepListFromApi(client);
                }

                return View(viewModel);
            }

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_apiBaseAddress);
                var token = GetCurrentUserToken();
                if (!string.IsNullOrEmpty(token))
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var dto = new AddCourseDTO
                {
                    CourseCode = viewModel.CourseCode,
                    CourseName = viewModel.CourseName,
                    Credits = viewModel.Credits,
                    DepartmentId = viewModel.DepartmentId
                };

                var response = await client.PostAsJsonAsync("api/courses/add", dto);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Ders başarıyla eklendi.";
                    return RedirectToAction("List", "Courses");
                }
                else
                {
                    ModelState.AddModelError("", @"Ders eklenirken bir hata oluştu.");

                    viewModel.DepartmentList = await GetDepListFromApi(client);
                    return View(viewModel);
                }
            }
        }

        // GET: Courses/Edit/5
        [CustomAuth(Roles.Admin)]
        public async Task<ActionResult> Edit(Guid id)
        {
            if (id == Guid.Empty)
                return RedirectToAction("List", "Courses");
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_apiBaseAddress);
                var token = GetCurrentUserToken();
                if (!string.IsNullOrEmpty(token))
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var res = await client.GetAsync($"/api/courses/edit/{id}");

                if (res.IsSuccessStatusCode)
                {
                    var json = await res.Content.ReadAsStringAsync();
                    var dto = JsonConvert.DeserializeObject<EditCourseDTO>(json);
                    var viewModel = new EditCourseViewModel()
                    {
                        CourseId = dto.CourseId,
                        CourseCode = dto.CourseCode,
                        CourseName = dto.CourseName,
                        Credits = dto.Credits,
                        DepartmentId = dto.DepartmentId,
                        DepartmentList = await GetDepListFromApi(client),
                    };
                    return View(viewModel);
                }

                if (res.StatusCode == HttpStatusCode.BadRequest)
                    ModelState.AddModelError("", "Gönderilen ID Geçersiz");
                if (res.StatusCode == HttpStatusCode.NotFound)
                    ModelState.AddModelError("", "Ders Bulunamadı");

                return RedirectToAction("List", "Courses");
            }
        }

        // POST: Courses/Edit
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(EditCourseViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", @"Model Geçerli Değil");
                //viewModel.DepartmentList =
                //    await GetDepListFromApi(new HttpClient().BaseAddress = new Uri(_apiBaseAddress));
                //TODO Burada Kaldın
                return View(viewModel);
            }

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_apiBaseAddress);
                var token = GetCurrentUserToken();
                if (!string.IsNullOrEmpty(token))
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var dto = new EditCourseDTO
                {
                    CourseName = viewModel.CourseName,
                    CourseCode = viewModel.CourseCode,
                    CourseId = viewModel.CourseId,
                    DepartmentId = viewModel.DepartmentId,
                    Credits = viewModel.Credits,
                };

                var res = await client.PostAsJsonAsync("/api/courses/edit", dto);

                if (res.IsSuccessStatusCode)
                {
                }
                else
                {
                    ModelState.AddModelError("", @"Ders Eklenirken Hata Oluştu");
                }

                viewModel.DepartmentList = await GetDepListFromApi(client);
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

        private async Task<SelectList> GetDepListFromApi(HttpClient client)
        {
            var res = await client.GetAsync("/api/courses/add");
            if (!res.IsSuccessStatusCode) return null;
            var json = await res.Content.ReadAsStringAsync();
            var courseDataDto = JsonConvert.DeserializeObject<AddCourseDTO>(json);
            return new SelectList(courseDataDto.DepartmentsList, "DepartmentId", "DepartmentName");
        }
    }
}