using MadMicro.Web.Models;
using MadMicro.Web.Services.IService;
using MadMicro.Web.Utility;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;

namespace MadMicro.Web.Controllers
{
    public class OrderController : Controller
    {

        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public IActionResult OrderIndex()
        {
            return View();
        }

        public async Task<IActionResult> OrderDetail(int orderId)
        {

            var orderHeaderDto = new OrderHeaderDTO();

            string userId = User.Claims.FirstOrDefault(u => u.Type is JwtRegisteredClaimNames.Sub).Value;

            var res = await _orderService.GetOrder(orderId);

            if (res is not null && res.IsSuccess)
                orderHeaderDto = JsonConvert.DeserializeObject<OrderHeaderDTO>(res.Result!.ToString()!);

            if(User.IsInRole(StaticDetail.RoleAdmin) && userId != orderHeaderDto.UserId)
                return NotFound();

            return View(orderHeaderDto);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() 
        {
            IEnumerable<OrderHeaderDTO> orderHeaderList;

            string userId = string.Empty;

            if(!User.IsInRole(StaticDetail.RoleAdmin))
            {
                userId = User.Claims.FirstOrDefault(u => u.Type is JwtRegisteredClaimNames.Sub).Value;
            }

            var res = await _orderService.GetOrders(userId);


            orderHeaderList = (res != null && res.IsSuccess) ? orderHeaderList = 
                orderHeaderList = JsonConvert.DeserializeObject<List<OrderHeaderDTO>>(res.Result!.ToString()!)! : new List<OrderHeaderDTO>();

            return  Json(new { data = orderHeaderList });
        
        }

    }
}
