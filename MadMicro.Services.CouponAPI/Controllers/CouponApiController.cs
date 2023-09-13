using AutoMapper;
using MadMicro.Services.CouponAPI.DataContext;
using MadMicro.Services.CouponAPI.Models;
using MadMicro.Services.CouponAPI.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MadMicro.Services.CouponAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CouponApiController : ControllerBase
{
    private readonly AppDbContext _context;
    private ResponseDto _response;
    private IMapper _mapper;

    public CouponApiController(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _response = new ResponseDto();
        _mapper = mapper;    
    }

    [HttpGet]

    public async Task<ResponseDto> Get()
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
    public async Task<ResponseDto> Get(int id)
    {
        try
        {
            var obj = await _context.Coupons.FirstOrDefaultAsync(c => c.CouponId == id);
            if (obj != null)
            {
                _response.Result = _mapper.Map<CouponDTO>(obj);
                return _response;
            }
            _response.IsSuccess = false;
            _response.Message = "No data Valid";
           
        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.Message = ex.Message;
        }
        return _response; 
    }




}
