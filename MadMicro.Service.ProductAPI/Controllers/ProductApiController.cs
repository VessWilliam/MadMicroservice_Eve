using AutoMapper;
using MadMicro.Service.ProductAPI.Models;
using MadMicro.Service.ProductAPI.Models.DTO;
using MadMicro.Services.ProductAPI.DataContext;
using MadMicro.Services.ProductAPI.Models.DTO;
using Microsoft.AspNetCore.Authorization;
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

    [HttpGet]
    [Route("{id:int}")]
    public async Task<ResponseDTO> Get(int id)
    {

        try
        {
            var obj = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == id);
            if (obj == null)
            {
                _response.IsSuccess = false;
                _response.Message = "No data Valid";
                return _response;
            }
            _response.Result = _mapper.Map<ProductDTO>(obj);
            return _response;

        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.Message = ex.Message;
            throw;
        }
    }


    [HttpPost]
    [Authorize(Roles = "ADMIN")]
    public async Task<ResponseDTO> CreateNewProduct([FromBody] ProductDTO productDTO)
    {
        try
        {
            var obj = _mapper.Map<Product>(productDTO);
            await _context.Products.AddAsync(obj);
            await _context.SaveChangesAsync();
            _response.Result = _mapper.Map<ProductDTO>(obj);

        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.Message = ex.Message;
            throw;
        }
        return _response;
    }

    [HttpPut]
    [Authorize(Roles = "ADMIN")]
    public async Task<ResponseDTO> UpdateProduct([FromBody] ProductDTO productDTO)
    {
        try
        {
            var obj = _mapper.Map<Product>(productDTO);
            _context.Products.Update(obj);
            await _context.SaveChangesAsync();
            _response.Result = _mapper.Map<ProductDTO>(obj);

        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.Message = ex.Message;
            throw;
        }
        return _response;
    }

    [HttpDelete]
    [Route("{id:int}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<ResponseDTO> DeleteProduct(int id)
    {
        try
        {
            var obj = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == id);
            if (obj == null)
            {
                _response.IsSuccess = false;
                _response.Message = "No data Valid";
                return _response;
            }
            _context.Products.Remove(obj);  
            await _context.SaveChangesAsync();  
        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.Message = ex.Message;
            throw;
        }
        return _response;
    }
}
