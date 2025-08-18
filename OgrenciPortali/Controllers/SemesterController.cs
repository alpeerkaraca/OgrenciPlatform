using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using OgrenciPortali.Attributes;
using Shared.DTO;
using OgrenciPortali.ViewModels;
using Shared.Enums;

namespace OgrenciPortali.Controllers
{
    [CustomAuth(Roles.Admin)]
    public class SemesterController : Controller
    {
        private readonly string _apiBaseAddress = Utils.AppSettings.ApiBaseAddress;


        // GET: Semester/List
        public async Task<ActionResult> List()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_apiBaseAddress);
                var token = GetCurrentUserToken();
                if (!string.IsNullOrEmpty(token))
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await client.GetAsync("api/semesters");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var dto = JsonConvert.DeserializeObject<IEnumerable<ListSemestersDTO>>(json);
                    var semesters = dto.Select(s => new SemesterListViewModel
                    {
                        SemesterId = s.SemesterId,
                        SemesterName = s.SemesterName,
                        StartDate = s.StartDate,
                        EndDate = s.EndDate,
                        IsActive = s.IsActive
                    }).ToList();
                    return View(semesters);
                }
                else
                {
                    ModelState.AddModelError("", "Dönemler yüklenirken bir hata oluştu.");
                    return View(new List<SemesterListViewModel>());
                }
            }
        }

        // GET: Semester/Add
        public ActionResult Add()
        {
            return View(new SemesterAddViewModel());
        }

        // POST: Semester/Add
        [HttpPost]
        //[ValidateAntiForgeryToken]
        [CustomAuth]
        public async Task<ActionResult> Add(SemesterAddViewModel viewModel)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_apiBaseAddress);
                var token = GetCurrentUserToken();
                if (!string.IsNullOrEmpty(token))
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                if (!ModelState.IsValid)
                {
                    ModelState.AddModelError("", @"Form verilerinde hata bulundu. Tekrar deneyiniz.");
                    return View(viewModel);
                }

                var dto = new AddSemesterDTO
                {
                    SemesterName = viewModel.SemesterName,
                    StartDate = viewModel.StartDate,
                    EndDate = viewModel.EndDate,
                    IsActive = viewModel.IsActive
                };
                var response = await client.PostAsJsonAsync("api/semesters/add", dto);
                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Dönem başarıyla eklendi.";
                    return RedirectToAction("List", "Semester");
                }
                else
                {
                    ModelState.AddModelError("", @"Dönem eklenirken bir hata oluştu.");
                    return View(viewModel);
                }
            }
        }

        // GET: Semester/Edit/5
        public ActionResult Edit(Guid id)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_apiBaseAddress);
                var token = GetCurrentUserToken();
                if (!string.IsNullOrEmpty(token))
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = client.GetAsync($"api/semesters/edit/{id}").Result;
                if (response.IsSuccessStatusCode)
                {
                    var json = response.Content.ReadAsStringAsync().Result;
                    var dto = JsonConvert.DeserializeObject<EditSemesterDTO>(json);
                    var viewModel = new SemesterUpdateviewModel
                    {
                        SemesterId = dto.SemesterId,
                        SemesterName = dto.SemesterName,
                        StartDate = dto.StartDate,
                        EndDate = dto.EndDate,
                        IsActive = dto.IsActive
                    };
                    return View(viewModel);
                }
                else
                {
                    ModelState.AddModelError("", "Dönem bilgileri yüklenirken bir hata oluştu.");
                    return RedirectToAction("List");
                }
            }
        }

        // POST: Semester/Edit/5
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(SemesterUpdateviewModel viewModel)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_apiBaseAddress);
                var token = GetCurrentUserToken();
                if (!string.IsNullOrEmpty(token))
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                if (!ModelState.IsValid)
                {
                    ModelState.AddModelError("", @"Form verilerinde hata bulundu. Tekrar deneyiniz.");
                    return View(viewModel);
                }

                var dto = new EditSemesterDTO
                {
                    SemesterId = viewModel.SemesterId,
                    SemesterName = viewModel.SemesterName,
                    StartDate = viewModel.StartDate,
                    EndDate = viewModel.EndDate,
                    IsActive = viewModel.IsActive
                };
                var response = await client.PostAsJsonAsync("api/semesters/edit", dto);
                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Dönem başarıyla güncellendi.";
                    return RedirectToAction("List", "Semester");
                }
                else
                {
                    ModelState.AddModelError("", @"Dönem güncellenirken bir hata oluştu.");
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