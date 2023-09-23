using MadMicro.Web.Models;
using MadMicro.Web.Services.IService;
using MadMicro.Web.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

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

    public IActionResult Logout()
    {
        return View();
    }
}
