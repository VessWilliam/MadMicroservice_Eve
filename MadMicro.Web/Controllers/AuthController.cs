using MadMicro.Web.Models;
using MadMicro.Web.Services.IService;
using MadMicro.Web.Utility;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace MadMicro.Web.Controllers;

public class AuthController : Controller
{
    private readonly IAuthService _authService;
    private readonly ITokenProvider _tokenProvider;
    public AuthController(IAuthService authService, ITokenProvider tokenProvider)
    {
        _authService = authService;
        _tokenProvider = tokenProvider;
    }

    [HttpGet]
    public IActionResult Login()
    {
        UserLoginDTO userLoginDTO = new();
        return View(userLoginDTO);
    }


    [HttpPost]
    public async Task<IActionResult> Login(UserLoginDTO obj)
    {
        ResponseDTO responseDto = await _authService.LoginAsync(obj);

        if (responseDto != null && responseDto.IsSuccess)
        {
            LoginResponseDto loginResponseDto =
               JsonConvert.DeserializeObject<LoginResponseDto>(Convert.ToString(responseDto.Result));

            await SignInUser(loginResponseDto);
            _tokenProvider.SetToken(loginResponseDto.Token);
            return RedirectToAction("Index", "Home");
        }
        TempData["error"] = responseDto?.Message;

        return View(obj);
    }

    [HttpGet]
    public IActionResult Register()
    {
        var role = new List<SelectListItem>()
        {
          new SelectListItem{Text=StaticDetail.RoleAdmin, Value=StaticDetail.RoleAdmin},
          new SelectListItem{Text=StaticDetail.RoleCustomer, Value=StaticDetail.RoleCustomer},
        };

        ViewBag.RoleList = role;
        return View();
    }


    [HttpPost]
    public async Task<IActionResult> Register(UserRegisterDTO obj)
    {
        ResponseDTO result = await _authService.RegisterAsync(obj);
        ResponseDTO assignRole;

        if (result != null && result.IsSuccess)
        {
            if (string.IsNullOrEmpty(obj.Role))
                obj.Role = StaticDetail.RoleCustomer;

            assignRole = await _authService.AssignRoleLoginAsync(obj);

            if (assignRole != null && assignRole.IsSuccess)
            {
                TempData["success"] = "Registration Successful";
                return RedirectToAction(nameof(Login));
            }
        }

        TempData["error"] = result?.Message;

        var role = new List<SelectListItem>()
        {
          new SelectListItem{Text=StaticDetail.RoleAdmin, Value=StaticDetail.RoleAdmin},
          new SelectListItem{Text=StaticDetail.RoleCustomer, Value=StaticDetail.RoleCustomer},
        };


        ViewBag.RoleList = role;
        return View(obj);
    }

    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync();
        _tokenProvider.ClearToken();
        return RedirectToAction("index", "Home");
    }

    private async Task SignInUser(LoginResponseDto model)
    {
        var handler = new JwtSecurityTokenHandler();

        var jwt = handler.ReadJwtToken(model.Token);

        var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
        identity.AddClaim(new Claim(JwtRegisteredClaimNames.Email,
              jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Email).Value));
        identity.AddClaim(new Claim(JwtRegisteredClaimNames.Name,
              jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Name).Value));
        identity.AddClaim(new Claim(JwtRegisteredClaimNames.Sub,
              jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Sub).Value));

        identity.AddClaim(new Claim(ClaimTypes.Name,
             jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Email).Value));
        identity.AddClaim(new Claim(ClaimTypes.Role,
             jwt.Claims.FirstOrDefault(u => u.Type == "role").Value));


        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

    }
}
