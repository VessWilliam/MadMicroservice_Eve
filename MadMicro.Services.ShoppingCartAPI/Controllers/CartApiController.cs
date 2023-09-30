using AutoMapper;
using MadMicro.Services.ShoppingCartAPI.DataContext;
using MadMicro.Services.ShoppingCartAPI.Models;
using MadMicro.Services.ShoppingCartAPI.Models.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection.PortableExecutable;

namespace MadMicro.Services.ShoppingCartAPI.Controllers;

[Route("api/cart")]
[ApiController]
public class CartApiController : ControllerBase
{
    private ResponseDTO _response;
    private IMapper _mapper;
    private readonly AppDbContext _context;

    public CartApiController(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
        _response = new();
    }
    [HttpPost("CartUpsert")]
    public async Task<ResponseDTO> CartUpsert(CartDTO cartDTO)
    {
        try
        {
            // Check if the cart header exists.
            var cartHeaderFromDb = await _context.CartHeaders.AsNoTracking().
                 FirstOrDefaultAsync(u => u.UserId == cartDTO.CartHeaders.UserId);
            if (cartHeaderFromDb == null)
            {
                // Create a new cart header.
                CartHeaders cartHeader = _mapper.Map<CartHeaders>(cartDTO.CartHeaders);
                _context.CartHeaders.Add(cartHeader);
                await _context.SaveChangesAsync();

                // Set the cart header ID on the cart details object.
                cartDTO.CartDetails.First().CartHeaderId = cartHeader.CartHeaderId;
            }

            // Check if the cart detail exists.
            var cartDetailsFromDb = await _context.CartDetails.AsNoTracking().FirstOrDefaultAsync(p =>
                p.ProductId == cartDTO.CartDetails.First().ProductId &&
                p.CartHeaderId == cartHeaderFromDb.CartHeaderId);

            if (cartDetailsFromDb == null)
            {
                // Create a new cart detail.
                cartDTO.CartDetails.First().CartHeaderId = cartHeaderFromDb.CartHeaderId;
                _context.CartDetails.Add(_mapper.Map<CartDetails>(cartDTO.CartDetails.First()));
                await _context.SaveChangesAsync();
            }
            else
            {
                // Update the quantity of the cart detail.
                cartDTO.CartDetails.First().Count += cartDetailsFromDb.Count;
                cartDTO.CartDetails.First().CartHeaderId = cartDetailsFromDb.CartHeaderId;
                cartDTO.CartDetails.First().CartDetailsId = cartDetailsFromDb.CartDetailsId;
                _context.CartDetails.Update(_mapper.Map<CartDetails>(cartDTO.CartDetails.First()));
                await _context.SaveChangesAsync();
            }

            // Return the cartDTO object.
            _response.Result = cartDTO;
        }
        catch (Exception ex)
        {
            _response.Message = ex.Message.ToString();
            _response.IsSuccess = false;
        }
        return _response;
    }


    [HttpPost("RemoveCart")]
    public async Task<ResponseDTO> RemoveCart([FromBody] int cartDetailId)
    {

        try
        {
            CartDetails cartDetails = _context.CartDetails.First(u => u.CartDetailsId == cartDetailId);
            int totalCountCartItem = _context.CartDetails.Where(u => u.CartHeaderId == cartDetails.CartHeaderId).Count();
            _context.CartDetails.Remove(cartDetails);
            if (totalCountCartItem == 1)
            {
                var cartHeader = await _context.CartHeaders.FirstOrDefaultAsync(u => u.CartHeaderId == cartDetails.CartHeaderId);
                _context.CartHeaders.Remove(cartHeader);
            }
            await _context.SaveChangesAsync();
            _response.Result = true;
        }
        catch (Exception ex)
        {

            _response.Message = ex.Message.ToString();
            _response.IsSuccess = false;
        }
        return _response;
    }



}
