using MadMicro.Web.Models;
using MadMicro.Web.Services.IService;
using Microsoft.AspNetCore.Mvc;

namespace MadMicro.Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<IActionResult> ProductIndex()
        {
            List<ProductDTO> listOfProduct = new();
            ResponseDTO response = await _productService.GetAllProductsAsync();

            if(response != null)
            {



            }

            return View(listOfProduct);
        }
    }
}
