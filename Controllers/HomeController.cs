using AnimalsApp.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;

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

        public  IActionResult DefaultPage()
        {
            return View();
        }

        public async Task<IActionResult> AnimalsPage(int page = 1)
        {
            HttpResponseMessage response = await _httpClient.GetAsync(_BaseUrl);
            List < Animal > animalsShowed = new List< Animal >();

            if (response.IsSuccessStatusCode)
            {
                int pageSize = 10; 
                string responseData = await response.Content.ReadAsStringAsync();
                List<Animal> animals = JsonConvert.DeserializeObject<List<Animal>>(responseData) ?? new List<Animal>();
                int totalPages = calculateNumberOfPages(animals.Count());
                animalsShowed = animals.Skip((page - 1) * 10).Take(pageSize).ToList();

                ViewBag.Page = page;
                ViewBag.TotalPages = totalPages;
                return View(animalsShowed);
            }
            else
            {

            return RedirectToAction("Error");
            }

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