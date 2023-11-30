﻿using AutoMapper;
using MadMicro.Services.ProductAPI.Models;
using MadMicro.Services.ProductAPI.Models.DTO;
using MadMicro.Services.ProductAPI.DataContext;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace MadMicro.Services.ProductAPI.Controllers;

[ApiController,Route("api/product")]
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

    [HttpGet ,Route("{id:int}")]
    public async Task<ResponseDTO> Get(int id)
    {

        try
        {
            var obj = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == id);
            if (obj is null)
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


    [HttpPost ,Authorize(Roles = "ADMIN")]
    public async Task<ResponseDTO> CreateNewProduct([FromBody] ProductDTO productDTO)
    {
        try
        {
            var product = _mapper.Map<Product>(productDTO);
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            if (productDTO.Image is not null)
            {
                var filename = $"{product.ProductId}{Path.GetExtension(productDTO.Image.FileName)}";
                var filePath = $@"wwwroot\ProductImages\{filename}";
                var filePathDirectory = Path.Combine(Directory.GetCurrentDirectory(), filename);
                using var fileStream = new FileStream(filePathDirectory, FileMode.Create);

                var baseURL = $@"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value}
                                       {HttpContext.Request.PathBase.Value}";

                product.ImageUrl = $"{baseURL}/ProductImages/{filePath}";

                productDTO.Image.CopyTo(fileStream);
                product.ImageLocalPath = filePath;

            }
            else
            {
                product.ImageUrl = "https://placehold.co/600x400";
            }


            _context.Update(product);
            await _context.SaveChangesAsync();
            _response.Result = _mapper.Map<ProductDTO>(product);
            return _response;

        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.Message = ex.Message;
            throw;
        }
    }

    [HttpPut,Authorize(Roles = "ADMIN")]
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

    [HttpDelete ,Route("{id:int}") ,Authorize(Roles = "ADMIN")]
    public async Task<ResponseDTO> DeleteProduct(int id)
    {
        try
        {
            var obj = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == id);
            if (obj is null)
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
