using MadMicro.Services.ShoppingCartAPI.Models.DTO;
using MadMicro.Services.ShoppingCartAPI.Services.IService;
using Newtonsoft.Json;

namespace MadMicro.Services.ShoppingCartAPI.Services.Service;

public class CouponService : ICouponService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public CouponService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<CouponDTO> GetCoupon(string couponCode)
    {
        var client = _httpClientFactory.CreateClient("Coupon");
        var response = await client.GetAsync($"/api/coupon/GetByCode/{couponCode}");
        var apiContent = await response.Content.ReadAsStringAsync();
        var responseContent = JsonConvert.DeserializeObject<ResponseDTO>(apiContent);
        if (responseContent == null || !responseContent.IsSuccess)
             return new CouponDTO();
        
      
       return JsonConvert.DeserializeObject<CouponDTO>(responseContent.Result.ToString());
    }
}
