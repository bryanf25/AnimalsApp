using AnimalsApp.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
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

        public async Task<IActionResult> AnimalsPage()
        {
            HttpResponseMessage response = await _httpClient.GetAsync(_BaseUrl);

            if(response.IsSuccessStatusCode)
            {
                string responseData = await response.Content.ReadAsStringAsync();
                List<Animal> animals = JsonConvert.DeserializeObject<List<Animal>>(responseData);
                return View(animals);
            }
            else
            {

            return RedirectToAction("Error");
            }

        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}