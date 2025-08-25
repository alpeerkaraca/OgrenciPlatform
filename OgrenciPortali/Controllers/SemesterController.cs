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
using OgrenciPortali.Attributes;
using OgrenciPortali.Utils;
using Shared.DTO;
using OgrenciPortali.ViewModels;
using Shared.Enums;

namespace OgrenciPortali.Controllers
{
    [CustomAuth(Roles.Admin)]
    public class SemesterController : Controller
    {
        private ApiClient _apiClient;
        private IMapper _mapper;

        public SemesterController(ApiClient apiClient, IMapper mapper)
        {
            _apiClient = apiClient;
            _mapper = mapper;
        }


        // GET: Semester/List
        public async Task<ActionResult> List()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "api/semesters");
            var response = await _apiClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var dto = await response.Content.ReadAsAsync<IEnumerable<ListSemestersDTO>>();
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

        // GET: Semester/Add - Return data for modal or redirect to List
        public ActionResult Add()
        {
            // If it's an AJAX request, return JSON data for the modal
            if (Request.IsAjaxRequest() || Request.Headers["Content-Type"] == "application/json" || Request.Headers["Accept"].Contains("application/json"))
            {
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            
            // Regular request - redirect to List page where modal will be available
            return RedirectToAction("List", "Semester");
        }

        // POST: Semester/Add
        [HttpPost]
        //[ValidateAntiForgeryToken]
        [CustomAuth]
        public async Task<ActionResult> Add(SemesterAddViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", @"Form verilerinde hata bulundu. Tekrar deneyiniz.");
                return View(viewModel);
            }

            var dto = _mapper.Map<AddSemesterDTO>(viewModel);
            var request = new HttpRequestMessage(HttpMethod.Post, "api/semesters/add")
            {
                Content = new StringContent(JsonConvert.SerializeObject(dto), Encoding.UTF8, "application/json"),
            };

            var response = await _apiClient.SendAsync(request);
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

        // GET: Semester/Edit/5
        public async Task<ActionResult> Edit(Guid id)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"api/semesters/edit/{id}");
            var response = await _apiClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var dto = await response.Content.ReadAsAsync<EditSemesterDTO>();
                var viewModel = _mapper.Map<SemesterUpdateviewModel>(dto);
                return View(viewModel);
            }
            else
            {
                ModelState.AddModelError("", "Dönem bilgileri yüklenirken bir hata oluştu.");
                return RedirectToAction("List");
            }
        }

        // POST: Semester/Edit/5
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(SemesterUpdateviewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", @"Form verilerinde hata bulundu. Tekrar deneyiniz.");
                return View(viewModel);
            }

            var dto = _mapper.Map<EditSemesterDTO>(viewModel);
            var request = new HttpRequestMessage(HttpMethod.Post, "api/semesters/edit")
            {
                Content = new StringContent(JsonConvert.SerializeObject(dto), Encoding.UTF8, "application/json"),
            };

            var response = await _apiClient.SendAsync(request);
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
}