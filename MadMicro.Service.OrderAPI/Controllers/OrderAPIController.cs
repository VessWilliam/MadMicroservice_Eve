using AutoMapper;
using MadMicro.Services.OrderAPI.DataContext;
using MadMicro.Services.OrderAPI.Models;
using MadMicro.Services.OrderAPI.Models.DTO;
using MadMicro.Services.OrderAPI.Services.IService;
using MadMicro.Services.OrderAPI.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using Stripe;
using Microsoft.EntityFrameworkCore;

namespace MadMicro.Services.OrderAPI.Controllers
{
    [Route("api/order")]
    [ApiController]
    public class OrderAPIController : ControllerBase
    {

        protected ResponseDTO _response;
        private IMapper _mapper;
        private readonly AppDbContext _context;
        private IProductsService _productService;

        public OrderAPIController(AppDbContext context, IMapper mapper, IProductsService productService)
        {
            _context = context;
            _mapper = mapper;
            _response = new ResponseDTO();
            _productService = productService;
        }

        [HttpPost("CreateOrder"), Authorize]
        public async Task<ResponseDTO> CreateOrder([FromBody] CartDTO cartDTO)
        {
            try
            {
                OrderHeaderDTO orderHeaderDTO = _mapper.Map<OrderHeaderDTO>(cartDTO.CartHeaders);
                orderHeaderDTO.OrderTime = DateTime.Now;
                orderHeaderDTO.Status = StaticDetails.Status_Pending;
                orderHeaderDTO.OrderDetails = _mapper.Map<IEnumerable<OrderDetailsDTO>>(cartDTO.CartDetails);

                OrderHeader orderCreate = (await _context.AddAsync(_mapper.Map<OrderHeader>(orderHeaderDTO))).Entity;
                await _context.SaveChangesAsync();

                orderHeaderDTO.OrderHeaderId = orderCreate.OrderHeaderId;
                _response.Result = orderHeaderDTO;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
                throw;
            }
            return _response;
        }

        [Authorize, HttpPost("CreateStripeSession")]
        public async Task<ResponseDTO> CreateStripeSession([FromBody] StripeRequestDTO stripeRequestDTO)
        {
            try
            {
                StripeConfiguration.ApiKey = "sk_test_4eC39HqLyjWDarjtT1zdp7dc";

                var options = new SessionCreateOptions
                {
                    SuccessUrl = stripeRequestDTO.ApprovedURL,
                    CancelUrl = stripeRequestDTO.CancelURL,
                    LineItems = new List<SessionLineItemOptions>(),
                    Mode = "payment",
                };

                foreach(var item in stripeRequestDTO.OrderHeader.OrderDetails)
                {

                    var sessionItem = new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(item.Price * 100),// 20.99 -> 2099
                            Currency = "usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.Product.Name
                            }
                        },

                        Quantity = item.Count
                    };

                    options.LineItems.Add(sessionItem); 
                }

                var service = new SessionService();
                var stripeSession = service.Create(options);
                stripeRequestDTO.StripeSessionURL = stripeSession.Url;
                var orderHeader = await _context.OrderHeaders
                    .FirstOrDefaultAsync(u => u.OrderHeaderId == stripeRequestDTO.OrderHeader.OrderHeaderId);

                if (orderHeader is null) 
                {
                    _response.IsSuccess = false;
                    _response.Result = null;
                    return _response;
                };

                orderHeader.StripeSessionId = stripeSession.Id;
                await _context.SaveChangesAsync();
                _response.Result = stripeRequestDTO;
                _response.IsSuccess = true;
             
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message; 
            }

            return _response;
        }

    }
}

