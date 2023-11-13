using AutoMapper;
using MadMicro.Services.CouponAPI.DataContext;
using MadMicro.Services.CouponAPI.Models;
using MadMicro.Services.CouponAPI.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace MadMicro.Services.CouponAPI.Controllers;

[Route("api/coupon")]
[Authorize]
[ApiController]
public class CouponApiController : ControllerBase
{
    private readonly AppDbContext _context;
    private ResponseDTO _response;
    private IMapper _mapper;

    public CouponApiController(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _response = new ResponseDTO();
        _mapper = mapper;    
    }

    [HttpGet]

    public async Task<ResponseDTO> Get()
    {
        try
        {
            var objList = await _context.Coupons.ToListAsync();
             _response.Result = _mapper.Map<IEnumerable<CouponDTO>>(objList); 
        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.Message = ex.Message;
        }
        return _response;   
    }

    [HttpGet]
    [Route("{id:int}")]
    public async Task<ResponseDTO> Get(int id)
    {
        try
        {
            var obj = await _context.Coupons.FirstOrDefaultAsync(c => c.CouponId == id);
            if (obj == null)
            {
                _response.IsSuccess = false;
                _response.Message = "No data Valid";
                return _response;
            }
            _response.Result = _mapper.Map<CouponDTO>(obj);
           
        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.Message = ex.Message;
        }
        return _response; 
    }
    
    [HttpGet]
    [Route("GetByCode/{code}")]
    public async Task<ResponseDTO> GetByCouponCode(string code)
    {
        try
        {
            var obj = await _context.Coupons.FirstOrDefaultAsync(c => c.CouponCode.ToLower() == code);
            if (obj == null)
            {
                _response.IsSuccess = false;
                _response.Message = "No data Valid";
                return _response;
            }
            _response.Result = _mapper.Map<CouponDTO>(obj);
           
        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.Message = ex.Message;
        }
        return _response; 
    }
    
    
    [HttpPost]
    [Authorize(Roles = "ADMIN")]
    public async Task<ResponseDTO> CreateNewCoupon([FromBody] CouponDTO couponDTO)
    {
        try
        {
            var obj = _mapper.Map<Coupon>(couponDTO);           
            await _context.Coupons.AddAsync(obj);
            await _context.SaveChangesAsync();

            var options = new Stripe.CouponCreateOptions
            {
                AmountOff = (long)(couponDTO.DiscountAmount * 100),
                Name = couponDTO.CouponCode,
                Currency = "usd",
                Id = couponDTO.CouponCode
            };
            var service = new Stripe.CouponService();
            service.Create(options);



            _response.Result = _mapper.Map<CouponDTO>(obj);
        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.Message = ex.Message;
        }
        return _response; 
    }
    
    [HttpPut]
    [Authorize(Roles = "ADMIN")]
    public async Task<ResponseDTO> UpdateCoupon([FromBody] CouponDTO couponDTO)
    {
        try
        {
            var obj = _mapper.Map<Coupon>(couponDTO);           
             _context.Coupons.Update(obj);
            await _context.SaveChangesAsync();
            _response.Result = _mapper.Map<CouponDTO>(obj);

        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.Message = ex.Message;
        }
        return _response; 
    }
    
    [HttpDelete]
    [Route("{id:int}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<ResponseDTO> DeleteCoupon(int id)
    {
        try
        {
            var obj = await _context.Coupons.FirstOrDefaultAsync(c => c.CouponId == id);
            if (obj == null)
            {
                _response.IsSuccess = false;
                _response.Message = "No data Valid";
                return _response;
            }
            _context.Coupons.Remove(obj);
            await _context.SaveChangesAsync();

            var stripeService = new Stripe.CouponService();
            stripeService.Delete(obj.CouponCode);

        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.Message = ex.Message;
        }
        return _response; 
    }




}
