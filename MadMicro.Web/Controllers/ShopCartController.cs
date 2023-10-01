using MadMicro.Web.Models;
using MadMicro.Web.Services.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;

namespace MadMicro.Web.Controllers;

public class ShopCartController : Controller
{
    private readonly IShopCartService _cartService;

    public ShopCartController(IShopCartService cartService)
    {
        _cartService = cartService;
    }

    [Authorize]
    public async Task<IActionResult> CartIndex()
    {
        return View(await LoadCartBaseOnLoginId());
    }


    private async Task<CartDTO?> LoadCartBaseOnLoginId()
    {
        var userId = User.Claims.Where(
            u => u.Type == JwtRegisteredClaimNames.Sub).FirstOrDefault()?.Value;

        ResponseDTO? res = await _cartService.GetCartByUserIdAsync(userId);
        if (!(res != null & res.IsSuccess)) return new CartDTO();

        var cart = JsonConvert.DeserializeObject<CartDTO>(res.Result.ToString());
        return cart;
    }

}
