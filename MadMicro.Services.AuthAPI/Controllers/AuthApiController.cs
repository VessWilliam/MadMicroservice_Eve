using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MadMicro.Services.AuthAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthApiController : ControllerBase
    {

        [HttpPost("register")]
        public async Task<IActionResult> Register()
        {
            return Ok();
        } 
        
        [HttpPost("login")]
        public async Task<IActionResult> Login()
        {
            return Ok();
        }
    }
}
