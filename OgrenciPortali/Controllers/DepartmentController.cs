using OgrenciPortali.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Mvc;
using log4net;
using Newtonsoft.Json;
using OgrenciPortali.Attributes;
using OgrenciPortali.DTOs;
using OgrenciPortali.ViewModels;

namespace OgrenciPortali.Controllers
{
    [CustomAuth(Roles.Admin)]
    public class DepartmentController : Controller
    {
        private readonly string _apiBaseAddress = Utils.AppSettings.ApiBaseAddress;
        private readonly ILog Logger = LogManager.GetLogger(typeof(DepartmentController));

        private readonly OgrenciPortalContext _db = new OgrenciPortalContext();

        // GET: Department/List
        public async Task<ActionResult> List()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_apiBaseAddress);

                var token = GetCurrentUserToken();
                if (!string.IsNullOrEmpty(token))
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await client.GetAsync("api/departments");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var deps = JsonConvert.DeserializeObject<List<ListDepartmentsDTO>>(json);
                    var viewModel = new DepartmentListViewModel
                    {
                        Departments = deps
                    };
                    return View(viewModel);
                }

                return View(new DepartmentListViewModel());
            }
        }

        // GET: Department/Add
        public ActionResult Add()
        {
            return View(new DepartmentAddViewModel());
        }

        //POST: Department/Add
        [HttpPost]
        public async Task<ActionResult> Add(DepartmentAddViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Gönderilen formda hata mevcut. Tekrar deneyiniz.");
                return View(viewModel);
            }

            var dto = new AddDepartmentDTO
            {
                DepartmentCode = viewModel.DepartmentCode,
                DepartmentName = viewModel.DepartmentName
            };

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_apiBaseAddress);
                var token = GetCurrentUserToken();
                if (!string.IsNullOrEmpty(token))
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await client.PostAsJsonAsync("api/departments/add", dto);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("List", "Department");
                }
                else
                {
                    ModelState.AddModelError("", "Bölüm eklenirken bir hata oluştu.");
                    return View(viewModel);
                }
            }
        }

        //GET: Department/Edit/{id}

        public async Task<ActionResult> Edit(Guid id)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_apiBaseAddress);
                var token = GetCurrentUserToken();
                if (!string.IsNullOrEmpty(token))
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await client.GetAsync($"api/departments/edit/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var dto = JsonConvert.DeserializeObject<EditDepartmentDTO>(json);
                    if (dto == null)
                    {
                        ModelState.AddModelError("", "Bölüm bulunamadı.");
                        return RedirectToAction("List", "Department");
                    }

                    var viewModel = new DepartmentEditViewModel
                    {
                        DepartmentCode = dto.DepartmentCode,
                        DepartmentId = dto.DepartmentId,
                        IsActive = dto.IsActive,
                        DepartmentName = dto.DepartmentName
                    };
                    return View(viewModel);
                }

                ModelState.AddModelError("",
                    response.StatusCode == HttpStatusCode.NotFound
                        ? "Bölüm bulunamadı."
                        : "Bölüm düzenlenirken bir hata oluştu.");

                return RedirectToAction("List", "Department");
            }
        }

        //POST: Department/Edit/
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(DepartmentEditViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Gönderilen formda hata mevcut. Tekrar deneyiniz.");
                return View(viewModel);
            }

            var dto = new EditDepartmentDTO
            {
                DepartmentId = viewModel.DepartmentId,
                DepartmentCode = viewModel.DepartmentCode,
                DepartmentName = viewModel.DepartmentName,
                IsActive = viewModel.IsActive
            };

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_apiBaseAddress);
                var token = GetCurrentUserToken();
                if (!string.IsNullOrEmpty(token))
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await client.PostAsJsonAsync("api/departments/edit", dto);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("List", "Department");
                }
                else
                {
                    ModelState.AddModelError("", "Bölüm güncellenirken bir hata oluştu.");
                    return View(viewModel);
                }
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
    }
}