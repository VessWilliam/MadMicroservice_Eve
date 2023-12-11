using MadMicro.MessageBus;
using MadMicro.Services.AuthAPI.Models.DTO;
using MadMicro.Services.AuthAPI.RabbitMQSender.IService;
using MadMicro.Services.AuthAPI.Service.IService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MadMicro.Services.AuthAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthApiController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IConfiguration _configuration;
        private readonly IRabbitMQAuthMessageSender _rabbitMQSender;
        protected ResponseDTO _response;

        public AuthApiController(IAuthService authService, 
            IRabbitMQAuthMessageSender rabbitMQSender, IConfiguration configuration)
        {
            _authService = authService;
            _rabbitMQSender = rabbitMQSender;
            _configuration = configuration;
            _response = new();
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterDTO model)
        {
            var errorMsg = await _authService.Register(model);
            if(!string.IsNullOrEmpty(errorMsg)) 
            {
               _response.IsSuccess = false;
               _response.Message = errorMsg;
               return BadRequest(_response);
            }
            _rabbitMQSender.SendMessage(model.Email, 
               _configuration.GetValue<string>("TopicAndQueueNames:RegisterUserQueue")!);
            return Ok(_response);
        } 
        
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDTO model)
        {
            var loginRes = await _authService.Login(model);
            if(loginRes.User == null)
            {
                _response.IsSuccess = false;
                _response.Message = "Username or password is incorrect";
                return BadRequest(_response);   
            }
            _response.Result = loginRes;
            return Ok(_response);
        }
        
        
        [HttpPost("AssignRole")]
        public async Task<IActionResult> AssignRole([FromBody] UserRegisterDTO model)
        {
            var assignSuccess = await _authService.AssignRole(model.Email,model.Role.ToUpper());
            if(!assignSuccess)
            {
                _response.IsSuccess = false;
                _response.Message = "Error !";
                return BadRequest(_response);   
            }
            return Ok(_response);
        }
    }
}
