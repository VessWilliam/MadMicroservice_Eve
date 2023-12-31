﻿using AutoMapper;
using MadMicro.MessageBus;
using MadMicro.Services.ShoppingCartAPI.DataContext;
using MadMicro.Services.ShoppingCartAPI.Models;
using MadMicro.Services.ShoppingCartAPI.Models.DTO;
using MadMicro.Services.ShoppingCartAPI.RabbitMQSender.IService;
using MadMicro.Services.ShoppingCartAPI.Services.IService;
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
    private readonly IProductService _productService;
    private readonly ICouponService _couponService;
    private readonly IConfiguration _configuration;
    private readonly AppDbContext _context;
    private readonly IRabbitMQShoppingCartMessageSender _rabbitMQSender;
    private readonly IMessageBus _messageBus;

    public CartApiController(AppDbContext context, IMapper mapper, 
        IProductService productService, ICouponService couponService, 
        IRabbitMQShoppingCartMessageSender rabbitMQSender, 
        IConfiguration configuration)
    {
        _context = context;
        _mapper = mapper;
        _productService = productService;
        _couponService = couponService;
        _rabbitMQSender = rabbitMQSender;
        _configuration = configuration;
        _response = new();

    }

    [HttpPost("ApplyCoupon")]
    public async Task<object> ApplyCoupon([FromBody] CartDTO cartDTO)
    {
        try
        {

            var cart = await _context.CartHeaders.FirstAsync(u => 
                   u.UserId ==  cartDTO.CartHeaders.UserId);
            cart.CouponCode = cartDTO.CartHeaders.CouponCode;
            _context.CartHeaders.Update(cart);
            await _context.SaveChangesAsync();
            _response.Result = true;
        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.Message = ex.Message; 
            throw;
        }

        return _response;
    }

    [HttpPost("EmailCartRequest")]
    public async Task<object> EmailCartRequest([FromBody] CartDTO cartDTO)
    {
        try
        {
           _rabbitMQSender.SendMessage(cartDTO, 
                _configuration.GetValue<string>("TopicAndQueueNames:EmailShoppingCartQueue") ?? string.Empty);
            _response.Result = true;
        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.Message = ex.Message; 
            throw;
        }

        return _response;
    }


    [HttpPost("RemoveCoupon")]
    public async Task<object> RemoveCoupon([FromBody] CartDTO cartDTO)
    {
        try
        {

            var cart = await _context.CartHeaders.FirstAsync(u =>
                   u.UserId == cartDTO.CartHeaders.UserId);
            cart.CouponCode = "";
            _context.CartHeaders.Update(cart);
            await _context.SaveChangesAsync();
            _response.Result = true;
        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.Message = ex.Message;
            throw;
        }

        return _response;
    }


    [HttpGet("GetCart/{userId}")]
    public async Task<ResponseDTO> GetCart(string userId)
    {
        try
        {
            CartDTO cart = new()
            {
                CartHeaders = _mapper.Map<CartHeadersDTO>(_context.CartHeaders.First(u => u.UserId == userId))
            };
            cart.CartDetails = _mapper.Map<IEnumerable<CartDetailsDTO>>(
                _context.CartDetails.Where(u => u.CartHeader.CartHeaderId == cart.CartHeaders.CartHeaderId));

            IEnumerable<ProductDTO> productDTO = await _productService.GetProducts();

            foreach (var item in cart.CartDetails)
            {
                item.Product = productDTO.FirstOrDefault(u => u.ProductId == item.ProductId);
                cart.CartHeaders.CartTotal += item.Count * item.Product.Price;
            }

            //apply coupon if any 
            if (!string.IsNullOrEmpty(cart.CartHeaders.CouponCode))
            {
                CouponDTO coupon = await _couponService.GetCoupon(cart.CartHeaders.CouponCode);
                if (coupon != null && cart.CartHeaders.CartTotal > coupon.MinAmount)
                {
                    cart.CartHeaders.CartTotal -= coupon.DiscountAmount;
                    cart.CartHeaders.Discount = coupon.DiscountAmount;
                }
            }

            _response.Result = cart;
        }
        catch (Exception ex)
        {

            _response.IsSuccess = false;
            _response.Message = ex.Message;
        }
        return _response;
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
