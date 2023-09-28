using MadMicro.Web.Models;
using MadMicro.Web.Services.IService;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace MadMicro.Web.Controllers;

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
		ResponseDTO? response = await _productService.GetAllProductsAsync();

		if (response != null && response.IsSuccess)
			listOfProduct = JsonConvert.DeserializeObject<List<ProductDTO>>(response.Result.ToString());

		if (!string.IsNullOrEmpty(response?.Message))
			TempData["error"] = response?.Message;

		return View(listOfProduct);
	}

	public async Task<IActionResult> ProductCreate()
	{
		return View();
	}

	[HttpPost]
	public async Task<IActionResult> ProductCreate(ProductDTO productDTO)
	{
		if (ModelState.IsValid)
		{
			ResponseDTO response = await _productService.CreateProductAsync(productDTO);
			if (response != null && response.IsSuccess)
			{
				TempData["success"] = "Product Created Successful";
				return RedirectToAction(nameof(ProductIndex));
			}
			TempData["error"] = response?.Message;
		}
		return View(productDTO);
	}



	public async Task<IActionResult> ProductDelete(int productId)
	{
		ResponseDTO response = await _productService.GetAllProductsByIdAsync(productId);
		if (response != null && response.IsSuccess)
		{
			var model = JsonConvert.DeserializeObject<ProductDTO>(response.Result.ToString());
			return View(model);
		}
		TempData["error"] = response?.Message;
		return NotFound();
	}



	[HttpPost]
	public async Task<IActionResult> ProductDelete(ProductDTO productDTO)
	{
		ResponseDTO? response = await _productService.DeleteProductAsync(productDTO.ProductId);

		if (response != null && response.IsSuccess)
		{
			TempData["success"] = "Product Deleted Successful";
			return RedirectToAction(nameof(ProductIndex));
		}
		TempData["error"] = response?.Message;
		return View(productDTO);
	}
	
	public async Task<IActionResult> ProductEdit(int productId)
	{
		ResponseDTO? response = await _productService.GetAllProductsByIdAsync(productId);
		if (response != null && response.IsSuccess)
		{
			var model = JsonConvert.DeserializeObject<ProductDTO>(response.Result.ToString());
			return View(model);
		}
		TempData["error"] = response?.Message;
		return NotFound();
	}



	[HttpPost]
	public async Task<IActionResult> ProductEdit(ProductDTO productDTO)
	{
		ResponseDTO? response = await _productService.UpdateProductAsync(productDTO);

		if (response != null && response.IsSuccess)
		{
			TempData["success"] = "Product Updated Successful";
			return RedirectToAction(nameof(ProductIndex));
		}
		TempData["error"] = response?.Message;
		return View(productDTO);
	}
}
