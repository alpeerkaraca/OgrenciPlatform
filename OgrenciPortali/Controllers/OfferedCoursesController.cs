using log4net;
using OgrenciPortali.Attributes;
using OgrenciPortali.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Mvc;
using Newtonsoft.Json;
using Shared.DTO;
using Shared.Enums;

namespace OgrenciPortali.Controllers
{
    [CustomAuth(Roles.Admin)]
    public class OfferedCoursesController : Controller
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(CoursesController));
        private readonly string _apiBaseAddress = Utils.AppSettings.ApiBaseAddress;

        public async Task<ActionResult> List()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_apiBaseAddress);
                var token = GetCurrentUserToken();
                if (!string.IsNullOrEmpty(token))
                    client.DefaultRequestHeaders.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                var response = await client.GetAsync("api/offered");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    List<ListOfferedCoursesDTO> dto = JsonConvert.DeserializeObject<List<ListOfferedCoursesDTO>>(json);
                    var viewModel = new ListOfferedCoursesViewModel
                    {
                        OfferedCoursesList = dto
                    };
                    return View(viewModel);
                }
            }

            return View();
        }

        public async Task<ActionResult> Add()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_apiBaseAddress);
                var token = GetCurrentUserToken();
                if (!string.IsNullOrEmpty(token))
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await client.GetAsync("api/offered/add");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var dto = JsonConvert.DeserializeObject<AddOfferedCourseDTO>(json);
                    var viewModel = new AddOfferedCourseViewModel()
                    {
                        DaysList = new SelectList(dto.DaysOfWeek, "Value", "Text"),
                        AdvisorList = new SelectList(dto.AdvisorList, "Value", "Text"),
                        CourseList = new SelectList(dto.CourseList, "Value", "Text"),
                        SemesterList = new SelectList(dto.SemesterList, "Value", "Text")
                    };
                    return View(viewModel);
                }

                ModelState.AddModelError("", @"Sayfa yüklenirken bir hata oluştu. Daha sonra tekrar deneyiniz.");

                return RedirectToAction("List", "OfferedCourses");
            }
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Add(AddOfferedCourseViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(await FillModel(viewModel));
            }

            var dto = new AddOfferedCourseDTO
            {
                CourseId = viewModel.CourseId,
                DayOfWeek = viewModel.DayOfWeek,
                AdvisorId = viewModel.TeacherId,
                EndTime = viewModel.EndTime,
                StartTime = viewModel.StartTime,
                Quota = viewModel.Quota,
                SemesterId = viewModel.SemesterId,
            };

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_apiBaseAddress);
                var token = GetCurrentUserToken();
                if (!string.IsNullOrEmpty(token))
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await client.PostAsJsonAsync("api/offered/add", dto);

                if (response.IsSuccessStatusCode)
                {
                    // DÜZELTME 1: Başarılı yanıtı doğru şekilde işleme
                    var json = await response.Content.ReadAsStringAsync();
                    var responseObject = new { Message = "" }; // Anonim bir tip şablonu
                    var parsedData = JsonConvert.DeserializeAnonymousType(json, responseObject);

                    TempData["SuccessMessage"] = parsedData.Message;

                    return RedirectToAction("Add");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    string errorMessage;

                    try
                    {
                        var modelStateError = JsonConvert.DeserializeObject<Dictionary<string, object>>(errorContent);
                        if (modelStateError.ContainsKey("ModelState"))
                        {
                            errorMessage = "Lütfen formdaki tüm alanları doğru doldurduğunuzdan emin olun.";
                        }
                        else
                        {
                            var simpleError = new { Message = "" };
                            errorMessage = JsonConvert.DeserializeAnonymousType(errorContent, simpleError)?.Message ??
                                           errorContent;
                        }
                    }
                    catch (JsonReaderException)
                    {
                        // JSON değilse, düz metindir.
                        errorMessage = errorContent;
                    }

                    ModelState.AddModelError("", errorMessage);
                }
            }


            return View(await FillModel(viewModel));
        }

        // GET: OfferedCourses/Edit/5 
        [CustomAuth(Roles.Admin)]
        public async Task<ActionResult> Edit(Guid id)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_apiBaseAddress);
                var token = GetCurrentUserToken();
                if (!string.IsNullOrEmpty(token))
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await client.GetAsync($"api/offered/edit/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var dto = JsonConvert.DeserializeObject<EditOfferedCoursesDTO>(json);
                    var viewModel = new EditOfferedCourseViewModel
                    {
                        OfferedCourseId = dto.OfferedCourseId,
                        StartTime = dto.StartTime,
                        EndTime = dto.EndTime,
                        CourseId = dto.CourseId,
                        SemesterId = dto.SemesterId,
                        AdvisorId = dto.AdvisorId,
                        Quota = dto.Quota,
                        DayOfWeek = dto.DayOfWeek,
                        CourseList = new SelectList(dto.CourseList, "Value", "Text"),
                        SemesterList = new SelectList(dto.SemesterList, "Value", "Text"),
                        AdvisorList = new SelectList(dto.AdvisorList, "Value", "Text"),
                        DaysList = new SelectList(dto.DaysOfWeek, "Value", "Text")
                    };
                    return View(viewModel);
                }

                ModelState.AddModelError("", @"Sayfa yüklenirken bir hata oluştu. Daha sonra tekrar deneyiniz.");
            }

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
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_apiBaseAddress);
                var token = GetCurrentUserToken();
                if (!string.IsNullOrEmpty(token))
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var dto = new EditOfferedCoursesDTO
                {
                    OfferedCourseId = viewModel.OfferedCourseId,
                    StartTime = viewModel.StartTime,
                    EndTime = viewModel.EndTime,
                    CourseId = viewModel.CourseId,
                    SemesterId = viewModel.SemesterId,
                    AdvisorId = viewModel.AdvisorId,
                    Quota = viewModel.Quota,
                    DayOfWeek = viewModel.DayOfWeek
                };
                var response = await client.PostAsJsonAsync("api/offered/edit", dto);
                if (response.IsSuccessStatusCode)
                {
                    TempData.Add("SuccessMessage", "Dönem dersi başarıyla güncellendi.");
                }
                else
                {
                    ModelState.AddModelError("", "Dönem dersi güncellenirken bir hata oluştu.");
                }
            }


            return View(viewModel);
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



        private async Task<AddOfferedCourseViewModel> FillModel(AddOfferedCourseViewModel viewModel)
        {
            var token = GetCurrentUserToken();
            var client = new HttpClient();
            client.BaseAddress = new Uri(_apiBaseAddress);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await client.GetAsync("api/offered/add");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var dto = JsonConvert.DeserializeObject<AddOfferedCourseDTO>(json);

                viewModel.DaysList = new SelectList(dto.DaysOfWeek, "Value", "Text");
                viewModel.AdvisorList = new SelectList(dto.AdvisorList, "Value", "Text");
                viewModel.CourseList = new SelectList(dto.CourseList, "Value", "Text");
                viewModel.SemesterList = new SelectList(dto.SemesterList, "Value", "Text");
            }
            else
                ModelState.AddModelError("", @"Veri çekilirken hata oluştu. Tekrar deneyiniz.");

            client.Dispose();
            return viewModel;
        }
    }
}