using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using log4net;
using Newtonsoft.Json;
using OgrenciPortali.Attributes;
using OgrenciPortali.Utils;
using Shared.DTO;
using OgrenciPortali.ViewModels;
using Shared.Enums;

namespace OgrenciPortali.Controllers
{
    [CustomAuth(Roles.Admin)]
    public class DepartmentController : BaseController
    {
        private static ApiClient _apiClient;

        private static IMapper _mapper;

        public DepartmentController(ApiClient apiClient, IMapper mapper)
        {
            _apiClient = apiClient;
            _mapper = mapper;
        }

        // GET: Department/List
        public async Task<ActionResult> List()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "api/departments");
            var response = await _apiClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var deps = await response.Content.ReadAsAsync<List<ListDepartmentsDTO>>();
                var viewModel = new DepartmentListViewModel
                {
                    Departments = deps
                };
                return View(viewModel);
            }

            return View(new DepartmentListViewModel());
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
                ModelState.AddModelError("", "Model geçersiz.");
                return View(viewModel);
            }

            var dto = _mapper.Map<AddDepartmentDTO>(viewModel);
            var request = new HttpRequestMessage(HttpMethod.Post, "api/departments/add")
            {
                Content = new StringContent(JsonConvert.SerializeObject(dto), Encoding.UTF8, "application/json")
            };

            var response = await _apiClient.SendAsync(request);
            ;
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

        //GET: Department/Edit/{id}
        public async Task<ActionResult> Edit(Guid id)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"api/departments/add/{id}");
            var response = await _apiClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var dto = await response.Content.ReadAsAsync<EditDepartmentDTO>();
                var viewModel = _mapper.Map<DepartmentEditViewModel>(dto);
                return View(viewModel);
            }

            ModelState.AddModelError("",
                response.StatusCode == HttpStatusCode.NotFound
                    ? "Bölüm bulunamadı."
                    : "Bölüm düzenlenirken bir hata oluştu.");

            return RedirectToAction("List", "Department");
        }

        //POST: Department/Edit/
        [HttpPost]
        //[ValidateAntiForgeryToken]

        // TODO: Burada kaldın, _apiClient'e entegre etmeye devam et.
        public async Task<ActionResult> Edit(DepartmentEditViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Gönderilen formda hata mevcut. Tekrar deneyiniz.");
                return View(viewModel);
            }

            var dto = _mapper.Map<EditDepartmentDTO>(viewModel);
            var request = new HttpRequestMessage(HttpMethod.Post, "api/departments/edit")
            {
                Content = new StringContent(JsonConvert.SerializeObject(dto), Encoding.UTF8, "application/json")
            };

            var response = await _apiClient.SendAsync(request);
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
}