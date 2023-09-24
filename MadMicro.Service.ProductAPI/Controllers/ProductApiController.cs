using AutoMapper;
using MadMicro.Service.ProductAPI.Models.DTO;
using MadMicro.Services.ProductAPI.DataContext;
using MadMicro.Services.ProductAPI.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MadMicro.Service.ProductAPI.Controllers;

[Route("api/product")]
[ApiController]
public class ProductApiController : ControllerBase
{
    private readonly AppDbContext _context;
    private ResponseDTO _response;
    private IMapper _mapper;

    public ProductApiController(AppDbContext context, IMapper mapper)
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
            var objList = await _context.Products.ToListAsync();
            _response.Result = _mapper.Map<IEnumerable<ProductDTO>>(objList);
        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.Message = ex.Message;
        }
        return _response;
    }
}
