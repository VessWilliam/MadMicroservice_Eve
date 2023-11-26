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
using MadMicro.MessageBus;

namespace MadMicro.Services.OrderAPI.Controllers
{
    [Route("api/order")]
    [ApiController]
    public class OrderAPIController : ControllerBase
    {

        protected ResponseDTO _response;
        private IMapper _mapper;
        private readonly AppDbContext _context;
        private IProductService _productService;
        private readonly IConfiguration _config;
        private readonly IMessageBus _messageBus;

        public OrderAPIController(AppDbContext context, IMapper mapper, IProductService productService, 
            IConfiguration config, IMessageBus messageBus)
        {
            _context = context;
            _mapper = mapper;
            _response = new ResponseDTO();
            _productService = productService;
            _config = config;
            _messageBus = messageBus;
        }



        [HttpGet("GetOrders"), Authorize]

        public async Task<ResponseDTO> GetOrders(string? userId = "")
        {

            try
            {
                IEnumerable<OrderHeader> orderHeadersList = await _context.OrderHeaders
                 .Include(u => u.OrderDetails)
                 .OrderByDescending(u => u.OrderHeaderId)
                 .ToListAsync();

                orderHeadersList = User.IsInRole(StaticDetails.RoleAdmin) ? orderHeadersList : orderHeadersList.Where(u => u.UserId == userId);

                _response.Result = _mapper.Map<IEnumerable<OrderHeaderDTO>>(orderHeadersList);

                return _response;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
                return _response;   
                throw;
            }
        }


        [HttpGet("GetOrder/{id:int}"), Authorize]

        public async Task<ResponseDTO> GetOrder(int id)
        {

            try
            {

                var orderHeader = await _context.OrderHeaders.Include(u => u.OrderDetails).FirstAsync(u => u.OrderHeaderId == id);
                _response.Result = _mapper.Map<OrderHeaderDTO>(orderHeader);
                _response.IsSuccess = true;

                return _response;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
                return _response;
                throw;
            }
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

                var options = new SessionCreateOptions
                {
                    SuccessUrl = stripeRequestDTO.ApprovedURL,
                    CancelUrl = stripeRequestDTO.CancelURL,
                    LineItems = new List<SessionLineItemOptions>(),
                    Mode = "payment",
                };

                var DiscountCoupon = new List<SessionDiscountOptions>()
                {

                    new SessionDiscountOptions()
                    {
                        Coupon = stripeRequestDTO.OrderHeader.CouponCode
                    }

                };

                foreach (var item in stripeRequestDTO.OrderHeader.OrderDetails)
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

                if (stripeRequestDTO.OrderHeader.Discount > 0)
                {
                    options.Discounts = DiscountCoupon;
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


        [HttpPost("ValidateStripeSession") , Authorize]
        public async Task<ResponseDTO> ValidateStripeSession([FromBody] int orderHeaderId)
        {
            try
            {

                var orderHeader = await _context.OrderHeaders
                   .FirstOrDefaultAsync(u => u.OrderHeaderId == orderHeaderId);

                var service = new SessionService();
                var stripeSession = service.Get(orderHeader.StripeSessionId);

                var paymentIntentService = new PaymentIntentService();
                var paymentIntent = paymentIntentService.Get(stripeSession.PaymentIntentId);

                if (paymentIntent.Status is "succeeded")
                {
                    //then Payment success
                    orderHeader.PaymentIntentId = paymentIntent.Id;
                    orderHeader.Status = StaticDetails.Status_Approved;
                    await _context.SaveChangesAsync();

                    RewardsDTO rewardDTO = new()
                    {
                        OrderId = orderHeader.OrderHeaderId,
                        RewardsActivity = Convert.ToInt32(orderHeader.OrderTotal),
                        UserId = orderHeader.UserId
                    };

                    string topicName = _config.GetValue<string>("TopicAndQueueNames:OrderCreatedTopic")!;
                    await _messageBus.PublishMessage(rewardDTO, topicName);
                    _response.Result = _mapper.Map<OrderHeaderDTO>(orderHeader);
                    _response.IsSuccess = true;
                }

            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }

            return _response;
        }


        [HttpPost("UpdateOrderStatus/{orderId:int}") , Authorize]
        public async Task<ResponseDTO> UpdateOrderStatus(int orderId, [FromBody] string newStatus)
        {
            try
            {
                var orderHeader = await _context.OrderHeaders.FirstAsync(u => u.OrderHeaderId == orderId);

                if (orderHeader == null) return _response;


                if(newStatus == StaticDetails.Status_Cancelled)
                {

                    var options = new RefundCreateOptions
                    {
                        Reason = RefundReasons.RequestedByCustomer,
                        PaymentIntent = orderHeader.PaymentIntentId
                    };

                    var service = new RefundService();  
                    var refund = service.Create(options);

                }
                orderHeader.Status = newStatus;
                await _context.SaveChangesAsync();
                _response.IsSuccess = true;
                return _response;   
            }
            catch (Exception ex)
            {

                 _response.IsSuccess = false;
                 _response.Message = ex.Message; 
                return  _response;  
                throw;
            }
        }

    }
}

