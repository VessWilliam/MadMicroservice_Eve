using MadMicro.Web.Models;
using MadMicro.Web.Services.IService;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;

namespace MadMicro.Web.Controllers
{
    public class HomeController : Controller
    {
        
        private readonly IProductService _productService;

        public HomeController(IProductService productService)
        {
          _productService = productService;
        }

        public async Task<IActionResult> Index()
        {
            List<ProductDTO?> list = new();

            ResponseDTO? response = await  _productService.GetAllProductsAsync();

            if (response != null && response.IsSuccess)
                list = JsonConvert.DeserializeObject<List<ProductDTO?>>(response.Result.ToString());
            

            if (!string.IsNullOrEmpty(response?.Message))
                TempData["error"] = response?.Message;

            return View(list);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}