﻿using IdentityModel;
using MadMicro.Web.Models;
using MadMicro.Web.Services.IService;
using MadMicro.Web.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;

namespace MadMicro.Web.Controllers
{
    public class OrderController : Controller
    {

        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService) =>  _orderService = orderService;

        [Authorize]
        public IActionResult OrderIndex() => View();


        [Authorize]
        public async Task<IActionResult> OrderDetail(int orderId)
        {

            var orderHeaderDto = new OrderHeaderDTO();

            string userId = User.Claims.FirstOrDefault(u => u.Type is JwtRegisteredClaimNames.Sub).Value;

            var res = await _orderService.GetOrder(orderId);

            if (res is not null && res.IsSuccess)
                orderHeaderDto = JsonConvert.DeserializeObject<OrderHeaderDTO>(res.Result!.ToString()!);

            if(!User.IsInRole(StaticDetail.RoleAdmin) && userId != orderHeaderDto.UserId)
                return NotFound();

            return View(orderHeaderDto);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(string status) 
        {
            IEnumerable<OrderHeaderDTO> orderHeaderList;

            string userId = string.Empty;

          
            userId = User.Claims.FirstOrDefault(u => u.Type is JwtRegisteredClaimNames.Sub).Value;
            

            var res = await _orderService.GetOrders(userId);


            orderHeaderList = (res != null && res.IsSuccess) ? orderHeaderList = 
                orderHeaderList = JsonConvert.DeserializeObject<List<OrderHeaderDTO>>(res.Result!.ToString()!)! : new List<OrderHeaderDTO>();

            switch (status)
            {
                case StaticDetail.Status_Approved:
                    orderHeaderList = orderHeaderList.Where(u => u.Status is StaticDetail.Status_Approved);
                    break;
                case StaticDetail.Status_ReadyToPickup:
                      orderHeaderList = orderHeaderList.Where(u => u.Status is StaticDetail.Status_ReadyToPickup);
                    break;
                case StaticDetail.Status_Cancelled:
                    orderHeaderList = orderHeaderList.Where(u => u.Status is StaticDetail.Status_Cancelled);
                    break;

                default:
                    break;
            }

            return  Json(new { data = orderHeaderList });
        
        }

        [HttpPost("OrderReadyForPickup")]
        public async Task<IActionResult> OrderReadyForPickup(int orderId)
        {
            var res = await _orderService.UpdateOrderStatus(orderId, StaticDetail.Status_ReadyToPickup);
            if (res is null && !res.IsSuccess) return View();

            
            TempData["success"] = "Status Updated Successful";
            return RedirectToAction(nameof(OrderDetail), new { orderId });
        } 
        
        [HttpPost("CompleteOrder")]
        public async Task<IActionResult> CompleteOrder(int orderId)
        {
            var res = await _orderService.UpdateOrderStatus(orderId, StaticDetail.Status_Completed);
            if (res is null && !res.IsSuccess) return View();

            
            TempData["success"] = "Status Updated Successful";
            return RedirectToAction(nameof(OrderDetail), new { orderId });
        }


        [HttpPost("CancelOrder")]
        public async Task<IActionResult> CancelOrder(int orderId)
        {
            var res = await _orderService.UpdateOrderStatus(orderId, StaticDetail.Status_Cancelled);
            if (res is null && !res.IsSuccess) return View();


            TempData["success"] = "Status Updated Successful";
            return RedirectToAction(nameof(OrderDetail), new { orderId });
        }

    }
}
