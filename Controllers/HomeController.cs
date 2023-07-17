﻿using AnimalsApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace AnimalsApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly HttpClient _httpClient;
        private readonly string _BaseUrl = "http://localhost:3000/animals";
        public HomeController(ILogger<HomeController> logger, HttpClient httpClient)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public IActionResult DefaultPage()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> filterTable(IFormCollection form)
        {

            var parameters = form.Where(item => !string.IsNullOrEmpty(item.Value))
                .Select(item => new KeyValuePair<string, string>(item.Key, 
                (item.Key == "IsActive" && item.Value.Contains("on")) ? "true" : item.Value))
                .ToList();

            return RedirectToAction("AnimalsPage",  new { page = 1, parameters = formatParams(parameters) });
        }

        [HttpPatch]
        [Consumes("application/json")]
        public async Task<IActionResult> updateAnimal([FromBody] Animal data)
        {
            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
            };
            var content = new StringContent(JsonConvert.SerializeObject(data,settings), Encoding.UTF8, "application/json");
            try
            {
            var response = await _httpClient.PatchAsync($"http://localhost:3000/animals/{data.id}", content);
            return response.IsSuccessStatusCode? Ok("Record Updated successful") : BadRequest("Failed to update");

            }catch (Exception ex)
            {
                return Ok("No esta permitido");
            }

        }

        [HttpPost]
        [Consumes("application/json")]
        public async Task<IActionResult> saveAnimal([FromBody] Animal data)
        {
            var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"http://localhost:3000/animals", content);
                return response.IsSuccessStatusCode ? Ok("Record Created successful") : BadRequest("Failed to create");

         
        }
        [HttpDelete]
        public async Task<IActionResult> deleteAnimal( int id)
        {

            var response = await _httpClient.DeleteAsync($"http://localhost:3000/animals/{id}");
                return response.IsSuccessStatusCode ? Ok("Record Deleted Successfully") : BadRequest("Failed to deleted");

         
        }

        private async Task<List<Animal>> getAnimalsService(string parms)
        {
            string urlRequest = (parms == null) ? _BaseUrl : $"{_BaseUrl}/?{parms}";
            HttpResponseMessage response = await _httpClient.GetAsync(urlRequest);

            List<Animal> animalsToShow = new List<Animal>();
            if (response.IsSuccessStatusCode)
            {
            string responseData = await response.Content.ReadAsStringAsync();
            animalsToShow = JsonConvert.DeserializeObject<List<Animal>>(responseData) ?? new List<Animal>();
            return animalsToShow;

            }
            else
            {
                RedirectToAction("Error");
            }
            return animalsToShow;

        }

        private string formatParams(List<KeyValuePair<string, string>> parms )
        {
            StringBuilder queryString = new StringBuilder();
            foreach (var param in parms)
            {
                queryString.Append(param.Key);
                queryString.Append("=");
                queryString.Append(param.Value);
                queryString.Append("&");
            }
            if (queryString.Length > 0)
            {
                queryString.Length--;
            }
            return queryString.ToString();
        }


        public async Task<IActionResult> AnimalsPage(int page = 1,string parameters = null, bool lastpage =false)
        {
            List<Animal> animals = await getAnimalsService(parameters);
            int pageSize = 10;
            int totalPages = calculateNumberOfPages(animals.Count());
            if (lastpage == true)
            {
                page =  totalPages ;
            }
                
                List<Animal> animalsShowed = animals.Skip((page - 1) * pageSize).Take(pageSize).ToList();

                ViewBag.Page = page;
                ViewBag.TotalPages = totalPages;
                ViewBag.Rows = animals.Count();
                ViewBag.Showed = animalsShowed.Count();
                ViewBag.SexOptions =  new List<SelectListItem>
{
    new SelectListItem { Value = "Male", Text = "Male" },
    new SelectListItem { Value = "Female", Text = "Female" }
};
            return View(animalsShowed);
    
        }

        private int calculateNumberOfPages(int registers = 0)
        {
            return (int)Math.Ceiling((double)registers / 10);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}