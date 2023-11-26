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

        [HttpGet]
        public async Task<IActionResult> GetAll() 
        {
            IEnumerable<OrderHeaderDTO> orderHeaderList;

            string userId = string.Empty;

            if(!User.IsInRole(StaticDetail.RoleAdmin))
            {
                userId = User.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Sub).Value;
            }

            var res = await _orderService.GetOrders(userId);


            orderHeaderList = (res != null && res.IsSuccess) ? orderHeaderList = 
                orderHeaderList = JsonConvert.DeserializeObject<List<OrderHeaderDTO>>(res.Result!.ToString()!)! : new List<OrderHeaderDTO>();

            return  Json(new { data = orderHeaderList });
        
        }

    }
}
