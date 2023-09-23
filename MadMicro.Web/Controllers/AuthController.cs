using MadMicro.Web.Models;
using MadMicro.Web.Services.IService;
using MadMicro.Web.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

namespace MadMicro.Web.Controllers;

public class AuthController : Controller
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
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
            
            return RedirectToAction("Index", "Home");   
        }

        ModelState.AddModelError("CustomError", responseDto.Message);
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

        var role = new List<SelectListItem>()
        {
          new SelectListItem{Text=StaticDetail.RoleAdmin, Value=StaticDetail.RoleAdmin},
          new SelectListItem{Text=StaticDetail.RoleCustomer, Value=StaticDetail.RoleCustomer},
        };


        ViewBag.RoleList = role;
        return View(obj);
    }



    public IActionResult Logout()
    {
        return View();
    }
}
