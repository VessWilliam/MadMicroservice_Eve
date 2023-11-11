using AutoMapper;
using MadMicro.Services.OrderAPI.DataContext;
using MadMicro.Services.OrderAPI.Models;
using MadMicro.Services.OrderAPI.Models.DTO;
using MadMicro.Services.OrderAPI.Services.IService;
using MadMicro.Services.OrderAPI.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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

        public OrderAPIController(AppDbContext context, IMapper mapper, IProductService productService)
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
                OrderHeaderDTO orderHeaderDTO =  _mapper.Map<OrderHeaderDTO>(cartDTO.CartHeaders);   
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




    }
}
