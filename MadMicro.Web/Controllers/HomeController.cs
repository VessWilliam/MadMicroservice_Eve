using IdentityModel;
using MadMicro.Web.Models;
using MadMicro.Web.Services.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;

namespace MadMicro.Web.Controllers
{
    public class HomeController : Controller
    {
        
        private readonly IProductService _productService;
        private readonly IShopCartService _shopCartService;

        public HomeController(IProductService productService, IShopCartService shopCartService)
        {
            _productService = productService;
            _shopCartService = shopCartService;
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

        [Authorize]
        public async Task<IActionResult> ProductDetails(int productId)
        {
            ProductDTO? product = new();

            ResponseDTO? response = await _productService.GetAllProductsByIdAsync(productId);

            if (response != null && response.IsSuccess)
                product = JsonConvert.DeserializeObject<ProductDTO?>(response.Result.ToString());


            if (!string.IsNullOrEmpty(response?.Message))
                TempData["error"] = response?.Message;

            return View(product);
        }  
        
        
        [Authorize]
        [HttpPost]
        [ActionName("ProductDetails")]
        public async Task<IActionResult> ProductDetails(ProductDTO productDTO)
        {

            CartDTO cart = new()
            {
                CartHeaders = new()
                {
                    UserId = User.Claims.Where(u => u.Type == JwtClaimTypes.Subject)?.FirstOrDefault()?.Value
                }
            };

            CartDetailsDTO cartDetails = new()
            {
                Count = productDTO.Count,
                ProductId = productDTO.ProductId,   

            };

            List<CartDetailsDTO> cartDetailsList = new()
            {
                cartDetails
            };
            cart.CartDetails = cartDetailsList;


            ResponseDTO? response = await _shopCartService.UpsertCartAsync(cart);

            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Item Has Added To Cart";
                return RedirectToAction(nameof(Index));
            }

            if (!string.IsNullOrEmpty(response?.Message))
                   TempData["error"] = response?.Message;

            return View(productDTO);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}