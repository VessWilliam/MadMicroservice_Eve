using IdentityModel;
using MadMicro.Web.Models;
using MadMicro.Web.Services.IService;
using MadMicro.Web.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;

namespace MadMicro.Web.Controllers;

public class ShopCartController : Controller
{
    private readonly IShopCartService _cartService;
    private readonly IOrderService _orderService;
    public ShopCartController(IShopCartService cartService, IOrderService orderService)
    {
        _cartService = cartService;
        _orderService = orderService;
    }

    [Authorize]
    public async Task<IActionResult> CartIndex()
    {
        return View(await LoadCartBaseOnLoginId());
    }

    [Authorize]
    public async Task<IActionResult> CheckOut()
    {
        return View(await LoadCartBaseOnLoginId());
    }

    [HttpPost, Authorize, ActionName("CheckOut")]
    public async Task<IActionResult> CheckOut(CartDTO cartDTO)
    {
       var cart =  await LoadCartBaseOnLoginId();
       cart.CartHeaders.Phone = cartDTO.CartHeaders.Phone;
       cart.CartHeaders.Email = cartDTO.CartHeaders.Email;  
       cart.CartHeaders.Name = cartDTO.CartHeaders.Name;    
       var res = await _orderService.CreateOrderAsync(cart);
       var orderHeaderDTO = JsonConvert.DeserializeObject<OrderHeaderDTO?>(res.Result.ToString());


        if ( res != null && res.IsSuccess)
        {

            //get stripes session and redirect to stripes to place order

            var domain = $"{Request.Scheme}://{Request.Host.Value}/";

            StripeRequestDTO stripeRequestDTO = new()
            {
                ApprovedURL = $"{domain}ShopCart/Confirmation?OrderId={orderHeaderDTO.OrderHeaderId}",
                CancelURL = $"{domain}cart/CheckOut",
                OrderHeader = orderHeaderDTO,
                StripeSessionID = orderHeaderDTO.StripeSessionId
            };

            var stripeResponse = await _orderService.CreateStripeSessionAsync(stripeRequestDTO);
            var stripeResult = JsonConvert.DeserializeObject<StripeRequestDTO>(stripeResponse.Result.ToString());
            Response.Headers.Add("Location", stripeResult.StripeSessionURL);
            return new StatusCodeResult(303);
        }
        return View();
    }


    [Authorize]
    public async Task<IActionResult> Confirmation(int orderId)
    {
        ResponseDTO? res = await _orderService.ValidateStripeSessionAsync(orderId);

        var orderHeader =  JsonConvert.DeserializeObject<OrderHeaderDTO>(res.Result.ToString());
        if(orderHeader.Status == StaticDetail.Status_Approved)
        {
            return View(orderId);
        }

        // can redirect to some error page base on status 
        //return RedirectToAction(nameof(CartIndex));;
        return View(orderId);
    }


    public async Task<IActionResult> Remove(int cartDetailsId)
    {
        var userId = User.Claims.Where(u => u.Type is JwtClaimTypes.Subject)?.FirstOrDefault()?.Value;
        ResponseDTO? res = await _cartService.RemoveFromCartAsync(cartDetailsId);

        if (res is null || !res.IsSuccess) return View();

        TempData["success"] = "Remove Cart Succeed";
        return RedirectToAction(nameof(CartIndex));
    }

    [HttpPost]
    public async Task<IActionResult> ApplyCoupon(CartDTO cartDTO)
    {
        ResponseDTO? res = await _cartService.ApplyCouponAsync(cartDTO);

        if (res is null || !res.IsSuccess) return View();

        TempData["success"] = "Cart Updated Succeed";
        return RedirectToAction(nameof(CartIndex));
    }

    [HttpPost]
    public async Task<IActionResult> RemoveCoupon(CartDTO cartDTO)
    {
        cartDTO.CartHeaders.CouponCode = "";
        ResponseDTO? res = await _cartService.ApplyCouponAsync(cartDTO);

        if (res is null || !res.IsSuccess) return View();

        TempData["success"] = "Apply Coupon Succeed";
        return RedirectToAction(nameof(CartIndex));
    }


    [HttpPost]
    public async Task<IActionResult> EmailCart(CartDTO cartDTO)
    {

        var cart = await LoadCartBaseOnLoginId();
        cart.CartHeaders.Email = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Email).FirstOrDefault()?.Value;

        ResponseDTO res = await _cartService.EmailCart(cart);
        if(res is null || !res.IsSuccess)  return View();

        TempData["success"] = "Email Will Be Sent Shortly";
        return RedirectToAction(nameof(CartIndex));
    }

    private async Task<CartDTO?> LoadCartBaseOnLoginId()
    {
        var userId = User.Claims.Where(
            u => u.Type is JwtRegisteredClaimNames.Sub).FirstOrDefault()?.Value;

        ResponseDTO? res = await _cartService.GetCartByUserIdAsync(userId);
        if (res is not null && !res.IsSuccess) return new CartDTO();

        var cart = JsonConvert.DeserializeObject<CartDTO>(res.Result.ToString());
        return cart;
    }





}
