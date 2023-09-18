using MadMicro.Web.Models;
using MadMicro.Web.Services.IService;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace MadMicro.Web.Controllers
{
    public class CouponController : Controller
    {
        private readonly ICouponService _couponService;

        public CouponController(ICouponService couponService)
        {
            _couponService = couponService;
        }
        public async Task<IActionResult> CouponIndex()
        {
            List<CouponDTO>? listOfCoupon = new();

            ResponseDTO? response = await _couponService.GetAllCouponAsync();

            if (response != null && response.IsSuccess)
            {
                listOfCoupon = JsonConvert.DeserializeObject<List<CouponDTO>>(Convert.ToString(response?.Result));

            }
            else
            {
                TempData["error"] = response?.Message; 
            }

            return View(listOfCoupon);
        }

        public async Task<IActionResult> CouponCreate()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CouponCreate(CouponDTO model)
        {
            if(ModelState.IsValid) 
            {
                ResponseDTO? response = await _couponService.CreateCouponAsync(model);

                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = "Coupon Created Successful !";    
                    return RedirectToAction(nameof(CouponIndex));
                }
                else
                {
                    TempData["error"] = response?.Message;
                }
            }
            return View(model);
        }

        public async Task<IActionResult> CouponDelete(int couponID)
        {

			ResponseDTO? response = await _couponService.GetCouponByIdAsync(couponID);

			if (response != null && response.IsSuccess)
			{
               
                var model = JsonConvert.DeserializeObject<CouponDTO>(Convert.ToString(response.Result));
                return View(model); 

			}
            else
            {
                TempData["error"] = response?.Message;
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> CouponDelete(CouponDTO couponDTO)
        {

			ResponseDTO? response = await _couponService.DeleteCouponAsync(couponDTO.CouponId);

			if (response != null && response.IsSuccess)
			{
                TempData["success"] = "Coupon Deleted Successful !";
                return RedirectToAction(nameof(CouponIndex));
			}
            else
            {
                TempData["error"] = response?.Message;
            }
            return View(couponDTO);
        }

    }
}
